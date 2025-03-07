using System.Collections;
using System.Collections.Generic;
//using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    [Header("״̬")]
    public bool manual;//�Ƿ��ֶ����ü�ⷶΧ
    public bool isGround;
    public bool touchLeftWall;
    public bool touchRightWall;
    public Collider2D isThinGround;//�Ƿ�ȵ����ذ�
    public bool isInThinGround;//���ڼ��Player���岿���Ƿ��ڱ��ذ���
    //public Collider2D isHitThinGround;//�Ƿ�ײ�����ذ�
    public bool isSpike;

    [Header("ƽ̨�Ƕ�������")]
    public bool seeWall;//�ܷ񿴵�ǽ������ƽ̨�Ƕ����(��ǰֻ��Player����)
    public RaycastHit2D hitTop;
    public Vector2 seeLineOffset1;//�������ƫ����(���ڼ������)
    public Vector3 seeLineOffset2;//�������ƫ����(���ڻ���)
    //public Vector2 direction;//���߷���
    public float distance;//������

    [Header("������")]
    public float checkRaduis;//��ⷶΧ(Բ��)
    public LayerMask groundLayer;//��������
    public LayerMask spikeLayer;
    public Vector2 bottomOffset;//������ƫ����
    public Vector2 leftOffset;//�����ǽƫ����
    public Vector2 rightOffset;//�����ǽƫ����
    public Vector2 checkSize2;//��ⷶΧ(����)���ڼ�����
    //public Vector3 boundsCenter;//��ײ�����ĵ�//������
    //��Ϊ���ذ����ò���
    public LayerMask thinGroundLayer;//��ⱡ�ذ������
    public Vector2 checkSize;//��ⷶΧ(����)���ڼ���Ƿ񿨱��ذ�
    public Vector2 centerOffset;//������ĵ�
    //public Vector2 topOffset;//��ⶥ��ǽ��ƫ����


    public CapsuleCollider2D coll;//���ڼ���������ƫ��������ײ��

    private void Awake()
    {
        //coll = GetComponent<CapsuleCollider2D>();
        if (!manual)
        {
            leftOffset = new Vector2(-coll.bounds.size.x / 2 + coll.offset.x, coll.bounds.size.y / 2);
            rightOffset = new Vector2(coll.bounds.size.x / 2 + coll.offset.x, leftOffset.y);
            //topOffset = new Vector2(0, coll.bounds.size.y); 
            centerOffset = new Vector2(coll.offset.x, coll.bounds.size.y / 2);
            checkSize = new Vector2(coll.size.x, coll.size.y);
        }
        Check();//��ǰ���ܻ���ֳ�ʼδ��⵽�����ڵ�����������Awakeִ��һ�οɱ�������
    }

    private void Update()
    {
        Check();
        //boundsCenter = coll.bounds.center;
    }


    public void Check()
    {
        
        if (transform.localScale.x>0)
        {
            //�ȼ���Ƿ񱡵ذ壬�������isGroundҲ����Ϊtrue
            /*isThinGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset * transform.localScale, checkRaduis, thinGroundLayer);
            if (isThinGround) isGround = true;
            else isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset * transform.localScale, checkRaduis, groundLayer);*/
            isThinGround = Physics2D.OverlapBox((Vector2)transform.position + bottomOffset * transform.localScale, checkSize2, 0f, thinGroundLayer);
            if (isThinGround) isGround = true;
            else isGround = Physics2D.OverlapBox((Vector2)transform.position + bottomOffset * transform.localScale, checkSize2, 0f, groundLayer);
            isSpike = Physics2D.OverlapBox((Vector2)transform.position + bottomOffset * transform.localScale, checkSize2, 0f, spikeLayer);
            touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset * transform.localScale, checkRaduis, groundLayer);
            touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset * transform.localScale, checkRaduis, groundLayer);
            isInThinGround = Physics2D.OverlapBox((Vector2)transform.position + centerOffset * transform.localScale, checkSize, 0f, thinGroundLayer);
            //isHitThinGround = Physics2D.OverlapCircle((Vector2)transform.position + topOffset * transform.localScale, checkRaduis, thinGroundLayer);
            Vector2 v2 = (Vector2)transform.position + seeLineOffset1 * transform.localScale;
            hitTop = Physics2D.Raycast(v2, Vector2.right * transform.localScale, distance, groundLayer);
        }
        else if(transform.localScale.x < 0)
        {
            /*isThinGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset * transform.localScale, checkRaduis, thinGroundLayer);
            if (isThinGround) isGround = true;
            else isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset * transform.localScale, checkRaduis, groundLayer);*/
            isThinGround = Physics2D.OverlapBox((Vector2)transform.position + bottomOffset * transform.localScale, checkSize2,0f, thinGroundLayer);
            if (isThinGround) isGround = true;
            else isGround = Physics2D.OverlapBox((Vector2)transform.position + bottomOffset * transform.localScale, checkSize2, 0f, groundLayer);
            isSpike = Physics2D.OverlapBox((Vector2)transform.position + bottomOffset * transform.localScale, checkSize2, 0f, spikeLayer);
            touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset * transform.localScale, checkRaduis, groundLayer);
            touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset * transform.localScale, checkRaduis, groundLayer);
            isInThinGround = Physics2D.OverlapBox((Vector2)transform.position + centerOffset * transform.localScale, checkSize, 0f, thinGroundLayer);
            //isHitThinGround = Physics2D.OverlapCircle((Vector2)transform.position + topOffset * transform.localScale, checkRaduis, thinGroundLayer);
            Vector2 v2 = (Vector2)transform.position + seeLineOffset1 * transform.localScale;
            hitTop = Physics2D.Raycast(v2, Vector2.right * transform.localScale, distance, groundLayer);
        }
        seeWall = hitTop;
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset * transform.localScale, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset * transform.localScale, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset * transform.localScale, checkRaduis);
        //Gizmos.DrawWireSphere((Vector2)transform.position + topOffset * transform.localScale, checkRaduis);
        Gizmos.DrawCube((Vector2)transform.position+ centerOffset* transform.localScale, checkSize);//����Ƿ񿨱��ذ�
        Gizmos.DrawCube((Vector2)transform.position + bottomOffset * transform.localScale, checkSize2);//����Ƿ�ذ�
        Vector3 v3 = transform.position + seeLineOffset2 /** transform.localScale*/;
        Vector3 v3_1 = new Vector3(transform.position.x + seeLineOffset2.x * transform.localScale.x, v3.y, v3.z);
        Gizmos.DrawLine(v3_1, new Vector3(v3_1.x+ transform.localScale.x* distance, v3.y, v3.z) );
    }


}
