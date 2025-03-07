using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        Debug.Log("state change to chase");
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.anim.SetBool("isRun", true);
    }
    public override void LogicUpdate()
    {
        //非地面/检测到荆棘/碰墙后转身
        if ((!currentEnemy.physcisCheck.isGround) || currentEnemy.physcisCheck.isSpike || (currentEnemy.physcisCheck.touchLeftWall && currentEnemy.faceDir.x < 0) || (currentEnemy.physcisCheck.touchRightWall && currentEnemy.faceDir.x > 0))
        {
            currentEnemy.transform.localScale = new Vector3(currentEnemy.faceDir.x, 1, 1);
        }

        if(currentEnemy.lostTimeCounter<=0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
        }
    }

    public override void PhysicsUpdate()
    {

    }

    public override void OnExit()
    {
        //currentEnemy.lostTimeCounter = currentEnemy.lostTime;
        currentEnemy.anim.SetBool("isRun", false);
    }

    
}
