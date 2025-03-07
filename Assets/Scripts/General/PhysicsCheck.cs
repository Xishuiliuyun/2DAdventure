using System.Collections;
using System.Collections.Generic;
//using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    [Header("状态")]
    public bool manual;//是否手动设置检测范围
    public bool isGround;
    public bool touchLeftWall;
    public bool touchRightWall;
    public Collider2D isThinGround;//是否踩到薄地板
    public bool isInThinGround;//用于检测Player身体部分是否在薄地板中
    //public Collider2D isHitThinGround;//是否撞到薄地板
    public bool isSpike;

    [Header("平台登顶检测参数")]
    public bool seeWall;//能否看到墙，用于平台登顶检测(当前只有Player有用)
    public RaycastHit2D hitTop;
    public Vector2 seeLineOffset1;//射线起点偏移量(用于检测射线)
    public Vector3 seeLineOffset2;//射线起点偏移量(用于绘制)
    //public Vector2 direction;//射线方向
    public float distance;//检测距离

    [Header("检测参数")]
    public float checkRaduis;//检测范围(圆形)
    public LayerMask groundLayer;//检测物理层
    public LayerMask spikeLayer;
    public Vector2 bottomOffset;//检测地面偏移量
    public Vector2 leftOffset;//检测左墙偏移量
    public Vector2 rightOffset;//检测右墙偏移量
    public Vector2 checkSize2;//检测范围(方形)用于检测地面
    //public Vector3 boundsCenter;//碰撞体中心点//测试用
    //下为薄地板检测用参数
    public LayerMask thinGroundLayer;//检测薄地板物理层
    public Vector2 checkSize;//检测范围(方形)用于检测是否卡薄地板
    public Vector2 centerOffset;//检测中心点
    //public Vector2 topOffset;//检测顶部墙体偏移量


    public CapsuleCollider2D coll;//用于计算物理检测偏移量的碰撞体

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
        Check();//当前可能会出现初始未检测到物体在地面的情况，在Awake执行一次可避免此情况
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
            //先检测是否薄地板，如果是则isGround也设置为true
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
        Gizmos.DrawCube((Vector2)transform.position+ centerOffset* transform.localScale, checkSize);//检测是否卡薄地板
        Gizmos.DrawCube((Vector2)transform.position + bottomOffset * transform.localScale, checkSize2);//检测是否地板
        Vector3 v3 = transform.position + seeLineOffset2 /** transform.localScale*/;
        Vector3 v3_1 = new Vector3(transform.position.x + seeLineOffset2.x * transform.localScale.x, v3.y, v3.z);
        Gizmos.DrawLine(v3_1, new Vector3(v3_1.x+ transform.localScale.x* distance, v3.y, v3.z) );
    }


}
