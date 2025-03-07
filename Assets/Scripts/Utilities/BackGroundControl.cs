using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundControl : MonoBehaviour
{
    public bool manual;//是否手动设置,部分地图不设规则连续时可手动设置
    public GameObject iCamera;//摄像机对象
    public float mapWidth;//单个地图的宽度
    public int mapNum;//地图的重复次数
    public float totalWidth;//总地图宽度
    //public Vector2 centerPos;//由于父物体的添加且该脚本挂在父脚本上，中心点位置有误，需重新计算

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
