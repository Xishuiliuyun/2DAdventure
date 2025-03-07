using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundControl : MonoBehaviour
{
    public bool manual;//�Ƿ��ֶ�����,���ֵ�ͼ�����������ʱ���ֶ�����
    public GameObject iCamera;//���������
    public float mapWidth;//������ͼ�Ŀ��
    public int mapNum;//��ͼ���ظ�����
    public float totalWidth;//�ܵ�ͼ���
    //public Vector2 centerPos;//���ڸ����������Ҹýű����ڸ��ű��ϣ����ĵ�λ�����������¼���

    private void Awake()
    {
        iCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if(!manual)
        {
            GetTotalWidth(transform.childCount, transform);
            totalWidth = mapWidth * mapNum;
        }
    }

    private void Update()
    {
        Vector3 temPos = transform.position;
        //Debug.Log(camera.transform.position.x);
        //Debug.Log(transform.position.x);
        if (iCamera.transform.position.x>transform.position.x+ totalWidth/2)
        {
            temPos.x += totalWidth;
            transform.position = temPos;
        }
        else if(iCamera.transform.position.x<transform.position.x - totalWidth / 2)
        {
            temPos.x -= totalWidth;
            transform.position = temPos;
        }

    }


    /*public void GetTotalWidth()
    {
        int childCount = transform.childCount;
        int count = 0;
        Transform child;
        while (childCount != 0)
        {
            count = 0;
            for (int i = 0; i < childCount; i++)
            {
                child = transform.GetChild(i);
                count = child.childCount;
                if(count>0) break;
            }
            childCount = count;
            //child = transform.GetChild();
        }


        if(childCount>0)
        {
            for (int i=0;i< childCount;i++)
            {
                child = transform.GetChild(i);
                if (child.childCount == 0) totalWidth += child.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
                else;
            }
        }
    }*/

    public void GetTotalWidth(int childCount,Transform tran)
    {
        if (childCount == 0) mapWidth = tran.GetComponent<SpriteRenderer>().sprite.bounds.size.x*transform.localScale.x;
        else
        {
            Transform child;
            int count = 0;
            for (int i = 0; i < childCount; i++)
            {
                child = tran.GetChild(i);
                count = child.childCount;
                if (count > 0) GetTotalWidth(count, child);
                else mapWidth += child.GetComponent<SpriteRenderer>().sprite.bounds.size.x * child.localScale.x;
            }
        }
    }
}
