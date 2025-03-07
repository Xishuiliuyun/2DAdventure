using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailHideState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        if (currentEnemy.curState == NPCState.Chase) currentEnemy.anim.SetBool("scared", true);
        else if (currentEnemy.curState == NPCState.KeepHide)
        {
            currentEnemy.anim.Play("SnailKeepHide");
            currentEnemy.GetComponent<CapsuleCollider2D>().enabled = false;//¹Ø±ÕÎÏÅ£Åö×²Ìå
            currentEnemy.GetComponent<CircleCollider2D>().enabled = true;//¿ªÆôÎÏÅ£¿ÇÅö×²Ìå
        }
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.Show);
            //currentEnemy.anim.SetBool("show",true);
        }
    }
    
    public override void PhysicsUpdate()
    {
        
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("scared", false);
    }
}
