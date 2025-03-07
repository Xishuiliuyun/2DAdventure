using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData 
{
    public MyVector3 pos;
    public MyVector3 scale;
    public bool isDead;
    [Header("��������")]
    public float currentHP;
    public float currentEnergy;
    public float hp;
    public float att;
    public float def;
    public float energy;
    public float exp;
    [Header("����/����")]
    public int jumpTimes;//���ÿ���Ծ����
    public bool slipWall;//��ǽ
    public bool climbWall;//��ǽ
    public bool slid;//����
    [Header("��������")]
    public int keyNumber;//Կ������

}
