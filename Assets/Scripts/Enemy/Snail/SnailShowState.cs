using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailShowState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        /*if(!currentEnemy.hurt)
            currentEnemy.anim.SetTrigger("show");*/
        currentEnemy.anim.Play("SnailShow");
    }

    public override void LogicUpdate()
    {
        /*if (currentEnemy.lostTimeCounter <= 0)
        {
            //currentEnemy.SwitchState(NPCState.Show);
        }*/
    }

    public override void PhysicsUpdate()
    {
        

    }

    public override void OnExit()
    {

    }
}
