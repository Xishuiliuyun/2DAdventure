using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    private PlayerController playerController;
    private Ability playerAbility;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerController = GetComponent<PlayerController>();
        playerAbility = GetComponent<Ability>();
    }
    private void Update()
    {
        SetAnimation();
    }

    public void SetAnimation()
    {
        anim.SetFloat("velocityX",Mathf.Abs(rb.velocityX));
        anim.SetFloat("velocityY", rb.velocityY);
        anim.SetBool("isGround", physicsCheck.isGround);
        anim.SetBool("isDead", playerController.isDead);
        anim.SetBool("isAttack", playerController.isAttack);
        anim.SetBool("sliding", playerController.sliding);
        anim.SetBool("slidStart", playerController.slidStart);
        anim.SetBool("slidLoop", playerController.slidLoop);
        anim.SetBool("slidEnd", playerController.slidEnd);
        anim.SetBool("onDownJumpFlag", playerController.onDownJumpFlag);
        if (playerAbility.slipWall|| playerAbility.climbWall)//用于判断是否在滑墙，控制动画
        {
            if (physicsCheck.touchLeftWall && !playerController.jumping && transform.localScale.x == -1) anim.SetBool("isWall", true);
            else if(physicsCheck.touchRightWall && !playerController.jumping && transform.localScale.x == 1) anim.SetBool("isWall", true);
            else anim.SetBool("isWall", false);
        }
        else anim.SetBool("isWall", false);
    }

    public void PlayHurt()
    {
        anim.SetTrigger("hurt");
    }

    public void PlayAttack()
    {
        anim.SetTrigger("attack");
    }

    public void SetJumpTrigger()
    {
        anim.SetTrigger("jump");
    }


}
