using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyVector3 
{
    public float x;
    public float y;
    public float z;

    //���캯��������
    //�޲���
    public MyVector3() { }

    //��������x,y,z
    public MyVector3(float ox, float oy, float oz)
    {
        x = ox;
        y = oy;
        z = oz;
    }

    //��������x,y
    public MyVector3(float ox, float oy)
    {
        x = ox;
        y = oy;
        z = 0;
    }

    //һ������Vector3
    public MyVector3(Vector3 vector3)
    {
        x = vector3.x;
        y = vector3.y;
        z = vector3.z;
    }

    //����ת�������
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
