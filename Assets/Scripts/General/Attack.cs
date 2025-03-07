using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("基本参数")]
    public int damage;//伤害
    public float attackRange;//攻击范围
    public float attackRate;//攻击频率
    public Vector3 hitPos;
    public bool isSpike;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (this.gameObject.CompareTag("Spike"))
        {
            return;
        }
        else
        {
            //Debug.Log(collision.name);
            hitPos = this.transform.position;
            collision.GetComponent<Character>()?.TakeDamage(this);
        }
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        //荆棘的受伤受力方向异常，可能是绘制和添加时是以Tilemap的方式添加的导致的
        //荆棘的伤害以另外方式传递方向
        if (this.gameObject.CompareTag("Spike"))
        {
            Vector2 injurerPos = collision.transform.position;
            ContactPoint2D[] contactPoints = new ContactPoint2D[10];
            int contactCount = collision.GetContacts(contactPoints);
            Vector2 hitPosition = contactPoints[0].point;
            Debug.Log("受伤Pos" + injurerPos + "\n碰撞点" + hitPosition);
            hitPos = hitPosition;
            isSpike = true;
            collision.gameObject.GetComponent<Character>()?.TakeDamage(this);
            /*if ((injurerPos.x - hitPosition.x) > 0)
            {
                Debug.Log("向右后退");
            }
            else
            {
                Debug.Log("向左后退");
            }*/
        }


    }


}
