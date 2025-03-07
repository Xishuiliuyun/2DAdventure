using System.Collections;
using System.Collections.Generic;
//using System.Media;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public PhysicsCheck physcisCheck;

    [Header("��������")]
    public float normalSpeed;//��ͨ�ٶ�
    public float chaseSpeed;//׷���ٶ�
    [HideInInspector] public float currentSpeed;//��ǰ�ٶ�
    public Vector3 faceDir;//�泯����
    public string enemyName;

    [HideInInspector] public Transform attacker;
    [Header("��ʱ��")]
    public bool wait;
    public float waitTime;
    public float waitTimeCounter;
    public float lostTime;
    public float lostTimeCounter;

    [Header("���˲���")]
    public bool hurt;
    public float hurtForce;
    public bool dead;

    [Header("��Ҽ��")]
    public bool foundPlayer;
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;

    protected RaycastHit2D blockHit;
    protected RaycastHit2D playerHit;
    public bool isBlock;//����Ƿ��ڵ�
    public LayerMask blockLayer;
    public Vector3 blockHitPoint;
    public Vector3 playerHitPoint;

    protected BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected BaseState showState;

    //��ǰ״̬����ǰ���ڴ浵��¼����״̬
    public NPCState curState;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physcisCheck = GetComponent<PhysicsCheck>();
        currentSpeed = normalSpeed;
        waitTimeCounter = waitTime;
    }

    private void OnEnable()
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }

    private void Update()
    {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);

        currentState.LogicUpdate();
        TimeCounter();
    }

    private void FixedUpdate()
    {
        if (wait || hurt || dead)
            return;
        Move();

        currentState.PhysicsUpdate();
    }

    private void OnDisable()
    {
        currentState.OnExit();
    }


    public virtual void Move()
    {
        rb.velocity = new Vector2(faceDir.x * currentSpeed * Time.deltaTime, rb.velocity.y);
        if(currentState == patrolState )
        {
            anim.SetBool("isWalk", true);
        }
        else if (currentState == chaseState )
        {
            anim.SetBool("isWalk", false);
        }
    }

    public void TimeCounter()//��ʱ��
    {
        foundPlayer = FoundPlayer();
        if (!foundPlayer&& lostTimeCounter > 0)
        {
            lostTimeCounter -= Time.deltaTime;
        }
        else if (foundPlayer)
        {
            lostTimeCounter = lostTime;
            wait = false;
            waitTimeCounter = waitTime;
        }

        if (wait)
        {
            waitTimeCounter -= Time.deltaTime;
        }
        if (waitTimeCounter <= 0 )
        {
            wait = false;
            waitTimeCounter = waitTime;
            transform.localScale = new Vector3(faceDir.x, 1, 1);
        }
    }

    public bool FoundPlayer()
    {
        blockHit = Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, faceDir, checkDistance, blockLayer);
        playerHit = Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, faceDir, checkDistance, attackLayer);
        //��Ҫ�ȼ�⵽�ڵ����ڵ�������ס��player���ж����߱��ڵ�
        if (!playerHit) return false;//û��⵽���ֱ�ӷ���false
        if (!blockHit) return true;//û��⵽�ϰ�������⵽��ң�ֱ�ӷ���true
        playerHitPoint = playerHit.point;
        blockHitPoint = blockHit.point;
        if (faceDir.x < 0)//�泯��
        {
            if (blockHit.point.x - playerHit.point.x > 0)
            {
                isBlock = true;
            }
            else isBlock = false;
        }
        else if (faceDir.x > 0)
        {
            if (blockHit.point.x - playerHit.point.x < 0)
            {
                isBlock = true;
            }
            else isBlock = false;
        }

        return !isBlock;
    }

    public void SwitchState(NPCState state)
    {
        curState = state;
        var newState = state switch
        {
            NPCState.Patrol => patrolState,
            NPCState.Chase => chaseState,
            NPCState.Show=>showState,
            NPCState.KeepHide => chaseState,
            _ => null
        };

        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }


    #region �¼�ִ�з���
    public void GetHurt(Vector3 hitPos,bool isSpike)//�ܻ�����Ӧ���¼�����
    {
        hurt = true;
        //attacker = attackTrans;
        //����������ת��
        if(isSpike)
        {
            if (hitPos.x - transform.position.x < 0)//�������࣬���÷�תԭͼ����
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            if (hitPos.x - transform.position.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            //ת���������
            if (hitPos.x - transform.position.x < 0)//�������࣬���÷�תԭͼ����
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            if (hitPos.x - transform.position.x > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        //�������˶���
        anim.SetTrigger("hurt");

        //����
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(transform.position.x - hitPos.x, 0).normalized;
        StartCoroutine( OnHurt(dir));
    }

    //Я�̷�ʽ����ִ�����̣�Ҳ������Animation�������˶�����Ӵ����ڶ���������ɺ�ִ��״̬����
    private IEnumerator OnHurt(Vector2 dir)
    {
        //ʩ����
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);

        //�ȴ�0.5��
        yield return new WaitForSeconds(0.5f);

        hurt = false;

    }

    public void OnDie()
    {
        rb.velocity = Vector2.zero;
        dead = true;
        gameObject.layer = 2;
        anim.SetBool("dead", true);
        DataManager.instance.SetEnemyDead(GetComponent<DataDefinition>().ID);
    }

    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }

    #endregion



    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset+new Vector3(checkDistance * (-transform.localScale.x), 0,0), checkSize.y);
    }


}
