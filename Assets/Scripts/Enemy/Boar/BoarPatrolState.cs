using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
        currentEnemy.wait = false;
        currentEnemy.anim.SetBool("isWalk", true);
    }

    public override void LogicUpdate()
    {
        if(currentEnemy.foundPlayer)
        {
            //Debug.Log("foundPlayer");
            currentEnemy.SwitchState(NPCState.Chase);
        }

        //非地面/检测到荆棘/碰墙后转身
        if ((!currentEnemy.physcisCheck.isGround) || currentEnemy.physcisCheck.isSpike || (currentEnemy.physcisCheck.touchLeftWall && currentEnemy.faceDir.x < 0) || (currentEnemy.physcisCheck.touchRightWall && currentEnemy.faceDir.x > 0))
        {
            //Debug.Log(currentEnemy.physcisCheck.isGround);
            currentEnemy.rb.velocity = Vector2.zero;
            currentEnemy.wait = true;
            currentEnemy.anim.SetBool("isWalk", false);
        }
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void OnExit()
    {
        //Debug.Log("Exit");
        currentEnemy.anim.SetBool("isWalk", false);
    }

    
}
