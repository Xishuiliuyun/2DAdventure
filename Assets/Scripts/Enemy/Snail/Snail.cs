using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snail : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        enemyName = "Snail";
        patrolState = new SnailPatrolState();
        chaseState = new SnailHideState();
        showState = new SnailShowState();
    }

    
}
