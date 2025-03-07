using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyVector3 
{
    public float x;
    public float y;
    public float z;

    //构造函数及重载
    //无参数
    public MyVector3() { }

    //三个参数x,y,z
    public MyVector3(float ox, float oy, float oz)
    {
        x = ox;
        y = oy;
        z = oz;
    }

    //两个参数x,y
    public MyVector3(float ox, float oy)
    {
        x = ox;
        y = oy;
        z = 0;
    }

    //一个参数Vector3
    public MyVector3(Vector3 vector3)
    {
        x = vector3.x;
        y = vector3.y;
        z = vector3.z;
    }

    //数据转类型输出
    public Vector3 GetValue()
    {
        return new Vector3(x, y, z);
    }

    /*public void SetValue(Vector3 vector3)
    {
        x = vector3.x;
        y = vector3.y; 
        z = vector3.z;
    }*/
}
