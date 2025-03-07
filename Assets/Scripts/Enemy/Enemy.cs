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

    [Header("基本参数")]
    public float normalSpeed;//普通速度
    public float chaseSpeed;//追击速度
    [HideInInspector] public float currentSpeed;//当前速度
    public Vector3 faceDir;//面朝方向
    public string enemyName;

    [HideInInspector] public Transform attacker;
    [Header("计时器")]
    public bool wait;
    public float waitTime;
    public float waitTimeCounter;
    public float lostTime;
    public float lostTimeCounter;

    [Header("受伤参数")]
    public bool hurt;
    public float hurtForce;
    public bool dead;

    [Header("玩家检测")]
    public bool foundPlayer;
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;

    protected RaycastHit2D blockHit;
    protected RaycastHit2D playerHit;
    public bool isBlock;//检测是否被遮挡
    public LayerMask blockLayer;
    public Vector3 blockHitPoint;
    public Vector3 playerHitPoint;

    protected BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected BaseState showState;

    //当前状态，当前用于存档记录怪物状态
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

    public void TimeCounter()//计时器
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
        //需要先检测到遮挡且遮挡基本挡住了player才判定视线被遮挡
        if (!playerHit) return false;//没检测到玩家直接返回false
        if (!blockHit) return true;//没检测到障碍，但检测到玩家，直接返回true
        playerHitPoint = playerHit.point;
        blockHitPoint = blockHit.point;
        if (faceDir.x < 0)//面朝左
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


    #region 事件执行方法
    public void GetHurt(Vector3 hitPos,bool isSpike)//受击后响应的事件函数
    {
        hurt = true;
        //attacker = attackTrans;
        //碰到荆棘，转身
        if(isSpike)
        {
            if (hitPos.x - transform.position.x < 0)//玩家在左侧，不用反转原图方向
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
            //转身，面向玩家
            if (hitPos.x - transform.position.x < 0)//玩家在左侧，不用反转原图方向
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            if (hitPos.x - transform.position.x > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        //播放受伤动画
        anim.SetTrigger("hurt");

        //击退
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(transform.position.x - hitPos.x, 0).normalized;
        StartCoroutine( OnHurt(dir));
    }

    //携程方式控制执行流程，也可以在Animation里在受伤动画添加代码在动画播放完成后执行状态更改
    private IEnumerator OnHurt(Vector2 dir)
    {
        //施加力
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);

        //等待0.5秒
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
