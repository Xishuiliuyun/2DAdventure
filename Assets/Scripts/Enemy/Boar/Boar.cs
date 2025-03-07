using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        enemyName = "Boar";
        patrolState = new BoarPatrolState();
        chaseState = new BoarChaseState();
    }
}
