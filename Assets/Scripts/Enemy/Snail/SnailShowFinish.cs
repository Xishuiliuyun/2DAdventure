using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailShowFinish : StateMachineBehaviour
{
    protected Snail snail;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("snailShowStart");
        snail = animator.GetComponent<Snail>();
        animator.GetComponent<CapsuleCollider2D>().enabled = true;//¿ªÆôÎÏÅ£Åö×²Ìå
        animator.GetComponent<CircleCollider2D>().enabled = false;//¹Ø±ÕÎÏÅ£¿ÇÅö×²Ìå
        animator.SetBool("isWalk", true);
        animator.SetBool("scared", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (snail.foundPlayer)
        {
            animator.SetBool("isWalk", false);
            snail.SwitchState(NPCState.Chase);
        }
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("snailShowFinish");
        if (animator.GetBool("isWalk"))
        {
            snail.SwitchState(NPCState.Patrol);
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
