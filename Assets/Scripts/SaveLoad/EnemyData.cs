using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyData 
{
    public MyVector3 pos;
    public MyVector3 scale;
    public string enemyName;
    public string sceneName;
    public NPCState state;
    public float waitTimeCounter;
    public float lostTimeCounter;
    public float currentHP;
    public bool isDead = false;

}
