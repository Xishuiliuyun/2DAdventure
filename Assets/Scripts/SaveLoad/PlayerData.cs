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
    [Header("基础属性")]
    public float currentHP;
    public float currentEnergy;
    public float hp;
    public float att;
    public float def;
    public float energy;
    public float exp;
    [Header("能力/属性")]
    public int jumpTimes;//设置可跳跃次数
    public bool slipWall;//滑墙
    public bool climbWall;//爬墙
    public bool slid;//滑铲
    [Header("道具数量")]
    public int keyNumber;//钥匙数量

}
