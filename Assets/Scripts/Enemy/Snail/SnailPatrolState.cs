using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
        currentEnemy.anim.SetBool("isWalk", true);
        currentEnemy.anim.Play("SnailWalk");
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.foundPlayer)
        {
            currentEnemy.SwitchState(NPCState.Chase);
        }

        //非地面/检测到荆棘/碰墙后转身
        if ((!currentEnemy.physcisCheck.isGround) || currentEnemy.physcisCheck.isSpike || (currentEnemy.physcisCheck.touchLeftWall && currentEnemy.faceDir.x < 0 )|| (currentEnemy.physcisCheck.touchRightWall && currentEnemy.faceDir.x > 0))
        {
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
        currentEnemy.anim.SetBool("isWalk", false);
    }

}
