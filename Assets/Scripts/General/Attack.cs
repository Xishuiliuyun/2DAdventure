using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("��������")]
    public int damage;//�˺�
    public float attackRange;//������Χ
    public float attackRate;//����Ƶ��
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
        //�������������������쳣�������ǻ��ƺ����ʱ����Tilemap�ķ�ʽ��ӵĵ��µ�
        //�������˺������ⷽʽ���ݷ���
        if (this.gameObject.CompareTag("Spike"))
        {
            Vector2 injurerPos = collision.transform.position;
            ContactPoint2D[] contactPoints = new ContactPoint2D[10];
            int contactCount = collision.GetContacts(contactPoints);
            Vector2 hitPosition = contactPoints[0].point;
            Debug.Log("����Pos" + injurerPos + "\n��ײ��" + hitPosition);
            hitPos = hitPosition;
            isSpike = true;
            collision.gameObject.GetComponent<Character>()?.TakeDamage(this);
            /*if ((injurerPos.x - hitPosition.x) > 0)
            {
                Debug.Log("���Һ���");
            }
            else
            {
                Debug.Log("�������");
            }*/
        }


    }


}
