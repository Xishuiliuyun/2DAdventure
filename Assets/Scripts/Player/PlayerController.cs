using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using DG.Tweening;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UIElements;
using UnityEngine.UI;
using DG.Tweening.Core.Easing;

public class PlayerController : MonoBehaviour
{
    public PlayerInputController inputControl;//声明inputControl
    public Vector2 inputDirection;
    public SceneLoader sceneLoader;

    public Rigidbody2D rb;
    private SpriteRenderer spR;
    private PhysicsCheck physicsCheck;
    private Character character;
    private PlayerAnimationController animationController;
    private CapsuleCollider2D coll;
    private Ability playerAbility;
    public Sign sign;

    [Header("玩家状态")]
    [SerializeField] public int canJumpTimes;// 当前可跳跃次数
    [SerializeField] public bool jumping;//跳跃
    [SerializeField] public bool falling;//下落
    [SerializeField] private bool playerSlipWall;//滑墙
    [SerializeField] private bool playerClimbingWalling;//爬墙
    [SerializeField] private bool onClimbWallJumping;//蹬墙跳
    [SerializeField] private bool toWallToping;//登墙顶
    [SerializeField] private float toWallTopForce;//登墙顶时的力，调试用
    [SerializeField] private float jumpStartTime;//记录跳跃开始时间
    [SerializeField] private float jumpPressTime;//跳跃按下时间
    [SerializeField] private bool leaveWallBtnPress;//离墙按钮按下
    [SerializeField] private float leaveWallPressTime;//离墙按下时间
    [SerializeField] private bool leaveWallDuration;//执行了离墙操作
    [SerializeField] private float lastOnGroundTime;//最后在地面的时间点
    [SerializeField] private float coyoteJumpTime;//郊狼时间
    [SerializeField] private bool canCoyoteJump;//能否郊狼跳跃
    [SerializeField] private bool jumpDuration;//在跳跃持续期间，起跳到落地或爬墙的时间内
    [SerializeField] private bool flag;//用于控制平台下落时跳跃次数只能减一
    [SerializeField] private float slidCostEnergy;//滑铲能量消耗
    [SerializeField] public bool sliding;//正在滑铲
    [SerializeField] private float slidTime;//滑铲时长
    [SerializeField] private float slidSpeed;//滑铲速度
    [SerializeField] public bool slidStart;//滑铲开始
    [SerializeField] public bool slidLoop;//滑铲中
    [SerializeField] public bool slidEnd;//滑铲结束
    //跳跃穿过薄地板参数，已弃用
    //[SerializeField] public bool inThinGround;//卡在薄地板里
    //[SerializeField] public LayerMask thinGroundLayer;//薄地板层
    //[SerializeField] public bool thinGroundFlag1;
    //[SerializeField] public bool thinGroundFlag2;
    //[SerializeField] public Collider2D thinGround;
    //[SerializeField] private Bounds thinGroundBounds;//薄地板碰撞体骨骼
    [SerializeField] public bool downPress;//按住下方向键
    [SerializeField] public bool onDownJumping;//下跳
    [SerializeField] public bool onDownJumpFlag;//下跳判断，用于保持刚开始一段时间的下跳状态

    [Header("基本参数")]
    public float speed; //移动速度
    public float climbUpSpeed;//向上爬墙速度
    public float climbDowwnSpeed;//向下爬墙速度
    public float jumpForce;//跳跃力
    public float jumpHeight;//跳跃高度
    public float maxFallSpeed;//最大下落速度
    public float jumpPressWindow;//跳跃窗口期，按满时长会跳到最高点，否则提前添加重力加速下落
    public float leaveWallPressWindow;//离墙窗口期,按满时长从墙上下落
    [Header("重力系数")]
    public float defaultGravityScale;//默认重力系数
    public float attackGravityScale;//攻击时重力系数
    public float jumpGravityScale;//向上跳时重力系数
    public float fallGravityScale;//掉落时重力系数
    public float slipGravityScale;//滑墙时重力系数

    [Header("玩家状态")]
    public bool isHurt;
    public float hurtForce;
    public bool isDead;
    public bool isAttack;

    [Header("物理材质")]
    public PhysicsMaterial2D normalMaterial;//普通材质，摩擦0.4
    public PhysicsMaterial2D wallMaterial;//墙，无摩擦


    private void Awake()
    {
        inputControl = new PlayerInputController();//实例化inputControl
        rb = GetComponent<Rigidbody2D>();//获取该脚本绑定物体下的组件
        spR = GetComponent<SpriteRenderer>();
        physicsCheck = GetComponent<PhysicsCheck>();
        character = GetComponent<Character>();
        animationController = GetComponent<PlayerAnimationController>();
        playerAbility = GetComponent<Ability>();
        coll = GetComponent<CapsuleCollider2D>();

        //inputControl.GamePlay.Jump.started += Jump;
        inputControl.GamePlay.Attack.started += Attack;
        inputControl.GamePlay.Jump.performed += Jump;
        inputControl.GamePlay.Jump.canceled += JumpCancel;
        inputControl.GamePlay.Slid.performed += Slid;
        inputControl.UI.Back.performed += BackPerform;
        inputControl.UI.Pause.performed += PauseGame;
        inputControl.GamePlay.Confirm_E.started += sign.OnConfirm_E;
        inputControl.GamePlay.Confirm_F.started += sign.OnConfirm_F;
        //inputControl.GamePlay.Move.canceled += LeaveWall;

        /*inputControl.GamePlay.Test.started += TestStarted;
        inputControl.GamePlay.Test.performed += TestPerformed;
        inputControl.GamePlay.Test.canceled += TestCanceled;*/

        //playerAbility.InitAbility();//初始化角色能力
    }

    

    private void OnEnable()
    {
        inputControl.Enable();
    }
    private void OnDisable()
    {
        inputControl.Disable();
    }
    private void Update()
    {
        inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();
        //设置手柄死区
        if ((inputDirection.x >= 0 && inputDirection.x < 0.2) || (inputDirection.x <= 0 && inputDirection.x > -0.2))
        {
            inputDirection.x = 0;
        }
        if((inputDirection.y >= 0 && inputDirection.y < 0.2) || (inputDirection.y <= 0 && inputDirection.y > -0.2))
        {
            inputDirection.y = 0;
        }

        CheckState();

        if(rb.velocity.y < 0)
        {
            jumping = false;
            falling = true;
        }

        //设置重力系数
        /*if (isAttack && !physicsCheck.isGround) //攻击略微浮空
        {
            rb.velocity = new Vector2(rb.velocity.x,-0.5f);
            rb.gravityScale = attackGravityScale;
        }*/
        /*else if (isCoyoteJump)
        {
            rb.gravityScale = jumpGravityScale;
            if (!jumping ) rb.velocity = new Vector2(rb.velocity.x, 0);
            //rb.velocity = new Vector2(rb.velocity.x, 0);
            //rb.gravityScale = 0;
        }*/
        /*else */if (jumping)//跳跃
        {
            rb.gravityScale = jumpGravityScale;
        }
        else if(playerClimbingWalling)//爬墙
        {
            rb.gravityScale = 0;
        }
        else if (playerSlipWall)//滑墙
        {
            //Debug.Log("slipGravityScale");
            rb.gravityScale = slipGravityScale;
        }
        else if (falling && !playerSlipWall)//下落
        {
            //Debug.Log("fallGravityScale");
            rb.gravityScale = fallGravityScale;
        }
        else//默认
        {
            //Debug.Log("jumpGravityScale");
            rb.gravityScale = defaultGravityScale;
        }

        if (rb.velocity.y < maxFallSpeed)//设置最大下落速度
        {
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
        }




        if (playerSlipWall || playerClimbingWalling)//滑墙/爬墙时离墙
        {
            leaveWallDuration = false;
            //Debug.Log("SlipWall");
            if (inputControl.GamePlay.Move.WasPerformedThisFrame())
            {
                //Debug.Log("Move.WasPerformedThisFrame");
                if (inputDirection.x > 0 && transform.localScale.x == -1)
                {
                    leaveWallBtnPress = true;
                }
                else if (inputDirection.x < 0 && transform.localScale.x == 1)
                {
                    leaveWallBtnPress = true;
                }
                else 
                {
                    leaveWallBtnPress = false;
                    leaveWallPressTime = 0;
                }
            }
            else if(inputControl.GamePlay.Move.WasReleasedThisFrame())
            {
                leaveWallBtnPress = false;
                leaveWallPressTime = 0;
            }
            if(leaveWallBtnPress)
            {
                leaveWallPressTime += Time.deltaTime;
                //Debug.Log(leaveWallPressTime);
                if(leaveWallPressTime>= leaveWallPressWindow)//长按离墙
                {
                    playerSlipWall = false;
                    playerClimbingWalling = false;
                    leaveWallDuration = true;
                    canJumpTimes = playerAbility.jumpTimes - 1;

                    leaveWallBtnPress = false;
                    leaveWallPressTime = 0;
                }

            }/*
            if (inputControl.GamePlay.Move.WasPressedThisFrame())
            {
                leaveWallPressTime = Time.time - leaveWallStartTime;
                Debug.Log(leaveWallPressTime);
            }*/

        }


        //能否进行郊狼跳跃的判断
        //平台落下跳跃次数减少的处理
        if (physicsCheck.isGround ) 
        {
            lastOnGroundTime = Time.time;
            canCoyoteJump = true;
            if (rb.velocity.y <= 0 && !physicsCheck.isInThinGround && !onDownJumpFlag) jumpDuration = false;
            if (!jumpDuration) onDownJumping = false;
            flag = true;
            if (onDownJumping) flag = false;
            leaveWallDuration = false;
        }
        if (!physicsCheck.isGround)
        {
            if (jumpDuration) canCoyoteJump = false;
            else if (playerClimbingWalling || playerSlipWall) canCoyoteJump = false;
            else if (leaveWallDuration) canCoyoteJump = false;
            else if(onDownJumping) canCoyoteJump = false;
            else if (Time.time - lastOnGroundTime > coyoteJumpTime)
            {
                canCoyoteJump = false;
                while (flag)
                {
                    Debug.Log("leavePlatform canJumpTimes--");
                    canJumpTimes--;
                    flag = false;
                }
            }
        }


        //薄地板处理
        //跳跃时可穿过处理 (已弃用，用platform effector2d组件实现该功能了)
        /*if (physicsCheck.isHitThinGround)
        {
            //Debug.Log(physicsCheck.isHitThinGround);
            //设置thinGround的rb.excludeLayers包含Player层
            thinGroundFlag1 = true;
            thinGroundFlag2 = false;
            thinGround = physicsCheck.isHitThinGround;
            int excludedLayerMask = (1 << 7 | 1 << 3);//1左移7位，表示物理图层7，排除该图层碰撞
            thinGround.gameObject.GetComponent<Rigidbody2D>().excludeLayers = excludedLayerMask;
            //Debug.Log(physicsCheck.isHitThinGround.bounds.Contains(transform.position)); 
            //thinGroundBounds = physicsCheck.isHitThinGround.bounds;
        }*/
        /*if (physicsCheck.isThinGround)
        {
            thinGroundFlag2 = true;
        }*/
        //if (rb.velocity.y < 0)
        //{
        //Vector2 checkPoint = new Vector2(transform.position.x, transform.position.y + (coll.bounds.size.y / 10));
        //physicsCheck.isHitThinGround.gameObject.GetComponent<Rigidbody2D>().excludeLayers = 0;
        //Debug.Log(thinGroundBounds.Contains(checkPoint));
        //bool checkPoint;
        //checkPoint = Physics2D.OverlapCircle((Vector2)transform.position , 0.1f, thinGroundLayer);
        //Debug.Log(checkPoint);
        //Debug.Log(LayerMask.NameToLayer("ThinGround"));

        //if (thinGroundFlag1)
        //{
        //    thinGroundFlag1 = false;

        //}
        //if (thinGroundFlag2)
        //{
        //    thinGround.gameObject.GetComponent<Rigidbody2D>().excludeLayers = 0;
        //}
        //}

        //在薄地板上按下加跳跃时穿过处理
        if (physicsCheck.isThinGround)
        {
            if(inputDirection.y<0)//按下下方向键
            {
                downPress = true;
            }
        }
        if(!physicsCheck.isThinGround || inputDirection.y > 0)
        {
            downPress = false;
        }


        



        //按键输入测试//调试按钮
        /*if (inputControl.GamePlay.Test.IsPressed()) TestPerformed1();
        if (inputControl.GamePlay.Test.WasPressedThisFrame()) TestStarted1();
        if (inputControl.GamePlay.Test.WasReleasedThisFrame()) TestCanceled1();*/
        /*if (Input.GetKeyDown(KeyCode.Escape)) TestStarted1();
        if (Input.GetKey(KeyCode.Escape)) TestPerformed1();
        if (Input.GetKeyUp(KeyCode.Escape)) TestCanceled1();*/
        /*if (Keyboard.current.tabKey.wasPressedThisFrame) TestStarted2();
        if (Keyboard.current.tabKey.wasReleasedThisFrame) TestCanceled2();
        if (Keyboard.current.tabKey.isPressed) TestPerformed2();*/

    }
    private void FixedUpdate()
    {


        if (!isHurt && !isAttack && !onClimbWallJumping && !playerClimbingWalling && !playerSlipWall && !sliding)  
            Move();

        /*Debug.Log(physicsCheck.isGround);
        Debug.Log(rb.velocity.y);
        Debug.Log(onDownJumping);*/
        if (physicsCheck.isGround && !(rb.velocity.y > 0.1f) && !onDownJumping && !physicsCheck.isInThinGround) canJumpTimes = playerAbility.jumpTimes;//在地面后重置跳跃次数，解决从跳台下落时不能二段跳问题，圆检测，范围会导致起跳后次数重置，所以要排除跳跃时速度大于0的情况

        if ((playerAbility.slipWall || playerAbility.climbWall))//实现滑墙/爬墙功能
        {
            if (!physicsCheck.isGround && (physicsCheck.touchLeftWall || physicsCheck.touchRightWall) && !jumping && !onDownJumpFlag && !isHurt)
            {
                if (physicsCheck.touchLeftWall && transform.localScale.x == -1) SlipWall();
                else if (physicsCheck.touchRightWall && transform.localScale.x == 1) SlipWall();
                else { playerSlipWall = false; playerClimbingWalling = false; /*rb.gravityScale = jumpGravityScale;*/ }
            }
            else { playerSlipWall = false; playerClimbingWalling = false; /*rb.gravityScale = jumpGravityScale;*/ }
        }

        if (playerClimbingWalling && !physicsCheck.seeWall && !toWallToping && !falling) //爬墙到墙顶
        {
            if (rb.velocity.y <= 0) return;
            else StartCoroutine(ToWallTop());
        }

        if (sceneLoader.playerMove)
        {
            rb.velocity = new Vector2(transform.localScale.x * 100 * Time.deltaTime, 0);
        }
    }

    //碰撞检测//已弃用
    /*private void OnCollisionStay2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name == "ThinPlantform")
        {
            Debug.Log("inThinPlantform");
            inThinGround = true;
        }
    }*/
    //碰撞检测//已弃用
    /*private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "ThinPlantform")
        {
            Debug.Log("exitThinPlantform");
            inThinGround = false;
        }
    }*/



    /*//碰撞检测//此处是Trigger碰撞体的检测函数，原为Sign下设置检测是否有可互动物体的，此处用来进行穿薄地板功能的实现
    //Trigger检测精度不行，已弃用
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name == "ThinPlantform")
        {
            Debug.Log("inThinPlantform");
            inThinGround = true;
        }
    }
    //碰撞检测//此处是Trigger碰撞体的检测函数，原为Sign下设置检测是否有可互动物体的，此处用来进行穿薄地板功能的实现
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "ThinPlantform")
        {
            Debug.Log("exitThinPlantform");
            inThinGround = false;
        } 
    }*/


    private void Move()
    {
        Vector2 movement = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);
        //用来阻止Player卡墙，似乎不需要，且会影响隐藏区域的碰撞检测，已弃用
        //if (physicsCheck.touchLeftWall && inputDirection.x < 0) {movement = new Vector2(0, rb.velocity.y);
        //if (physicsCheck.touchRightWall && inputDirection.x > 0) movement = new Vector2(0, rb.velocity.y);
        //rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);
        rb.velocity = movement;

        //人物反转transform.localScale.x
        int faceDir = (int)transform.localScale.x;
        if (inputDirection.x < 0)
            faceDir = -1;
        if (inputDirection.x > 0)
            faceDir = 1;
        transform.localScale = new Vector3(faceDir, 1, 1);

        //人物反转spriteRenderer.Flip.x
        //注意，此方法只改变角色的精灵图片反转方向，不会影响角色下子物体的方向反转
        //后续会导致角色攻击的检测区域不跟随角色方向的问题
        /*int faceDir;
        if(spR.flipX) faceDir = -1;
        else faceDir = 1;

        if (inputDirection.x < 0)
            faceDir = -1;
        if (inputDirection.x > 0)
            faceDir = 1;

        if (faceDir == -1) spR.flipX = true;
        if (faceDir == 1) spR.flipX = false;*/

    }

    private void JumpCancel(InputAction.CallbackContext context)
    {
        if(!downPress)//下+跳不需要计算是否中途取消跳跃
        {
            //Debug.Log("JumpCancel");
            jumping = false;
            jumpPressTime = Time.time - jumpStartTime;
            if (jumpPressTime < jumpPressWindow && !onClimbWallJumping)
            {
                falling = true;
            }
        }
    }

    private IEnumerator OnDownJump()
    {
        //Debug.Log("downJump");
        falling = true;
        onDownJumping = true;
        jumpDuration = true;
        onDownJumpFlag = true;
        canJumpTimes = playerAbility.jumpTimes - 1;
        int excludedLayerMask = ~(1 << 7);//1左移7位，表示物理图层7，~取反后表示排除该图层碰撞
        Collider2D thinGroundColl = physicsCheck.isThinGround;
        thinGroundColl.gameObject.GetComponent<PlatformEffector2D>().colliderMask = excludedLayerMask;
        //Debug.Log("downJump");
        yield return new WaitForSeconds(0.3f);
        onDownJumpFlag = false;
        //onDownJumping = false;
        thinGroundColl.gameObject.GetComponent<PlatformEffector2D>().colliderMask = ~0;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (sliding) return;
        //Debug.Log("Jump");
        jumpPressTime = 0;
        jumpStartTime = Time.time;
        if (downPress)//下+跳，穿过薄地板
        {
            StartCoroutine(OnDownJump());
        }
        else if ((physicsCheck.isGround || canCoyoteJump) && !onDownJumping && canJumpTimes > 0 ) 
        {
            jumping = true;
            falling = false;
            jumpDuration = true;
            canJumpTimes = playerAbility.jumpTimes - 1;
            if(isAttack) animationController.SetJumpTrigger();
            //StartCoroutine(OnPlayerJumping());
            //Debug.Log("Jump");
            //jumpForce = MathF.Sqrt(jumpHeight * (Physics2D.gravity.y * jumpGravityScale) * -2) * rb.mass;//通过设定跳跃高度计算跳跃所需的力的大小，高度会略微低于设定值
            rb.velocity = Vector2.zero;//重置Player速度
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            GetComponent<AudioDefination>()?.PlayAudioClip();//播放音效
        }
        else if(playerSlipWall|| playerClimbingWalling)//在滑墙/爬墙,蹬墙跳
        {
            jumping = true;
            falling = false;
            jumpDuration = true;
            canJumpTimes = playerAbility.jumpTimes;
            if (isAttack) animationController.SetJumpTrigger();
            //StartCoroutine(OnPlayerJumping());
            StartCoroutine(OnClimbWallJump());
        }
        else//在空中，多段跳
        {
            if (canJumpTimes > 0) //已习得二段跳能力
            {
                //Debug.Log("DoubleJump");
                jumping = true;
                falling = false;
                jumpDuration = true;
                canJumpTimes--;
                if (isAttack) animationController.SetJumpTrigger();
                //StartCoroutine(OnPlayerJumping());
                rb.velocity = Vector2.zero;//重置Player速度
                rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
                GetComponent<AudioDefination>()?.PlayAudioClip();//播放音效
            }
        }
    }

    /*private IEnumerator OnPlayerJumping()//用于设置跳跃状态，延时后退出状态
    {
        jumping = true;
        yield return new WaitForSeconds(0.5f);
        jumping = false;
    }*/

    private IEnumerator OnClimbWallJump()
    {
        //Debug.Log("ClimbWallJump");
        onClimbWallJumping = true;
        jumping = true;
        leaveWallBtnPress = false;//蹬墙跳后重置离墙按钮情况，处理卡墙问题
        playerSlipWall = false;//设置状态
        playerClimbingWalling = false;//设置状态
        inputControl.GamePlay.Disable();
        rb.velocity = Vector2.zero;//重置Player速度
        Vector2 dir = new Vector2(1 * -transform.localScale.x, 3).normalized;
        rb.AddForce(dir * jumpForce, ForceMode2D.Impulse);
        //rb.AddForce(transform.right * jumpForce * transform.localScale.x, ForceMode2D.Impulse);
        GetComponent<AudioDefination>()?.PlayAudioClip();//播放音效
        yield return new WaitForSeconds(0.2f);
        onClimbWallJumping = false;
        canJumpTimes = playerAbility.jumpTimes - 1;//蹬墙跳后重置二段跳
        inputControl.GamePlay.Enable();

    }

    /*private void LeaveWall(InputAction.CallbackContext context)
    {
        
    }*/


    private void SlipWall()//滑墙/爬墙
    {
        if (rb.velocity.x <= 0) jumpDuration = false;
        if (rb.velocity.x <= 0 && !onDownJumpFlag) onDownJumping = false; 
        if (playerAbility.climbWall && !onClimbWallJumping)
        {
            if (!playerClimbingWalling) rb.velocity = Vector2.zero;
            playerClimbingWalling = true;
            falling = false;
            canJumpTimes = playerAbility.jumpTimes;
            if (inputDirection.y >= 0) rb.velocity = new Vector2(rb.velocity.x, inputDirection.y * climbUpSpeed * Time.deltaTime);
            else if (inputDirection.y < 0) rb.velocity = new Vector2(rb.velocity.x, inputDirection.y * climbDowwnSpeed * Time.deltaTime);
        }
        else if(!onClimbWallJumping)
        {
            if (!playerSlipWall) rb.velocity = Vector2.zero; //滑墙时重设速度为0处理墙上异常滑动
            playerSlipWall = true;
            //rb.gravityScale = slipGravityScale;
            canJumpTimes = playerAbility.jumpTimes - 1;//滑墙时重置二段跳
            /*if (rb.velocity.y < 0) rb.gravityScale = 0.5f;
            else { playerClimbWalling = false; rb.gravityScale = 4; }*/
        }
    }

    private IEnumerator ToWallTop()
    {
        Debug.Log("TOP");
        //设置状态
        toWallToping = true;
        playerSlipWall = false;
        playerClimbingWalling = false;
        //禁用玩家操作
        inputControl.GamePlay.Disable();
        //上墙(播放动画，移动坐标)
        //transform.position = new Vector3(transform.position.x, coll.bounds.center.y + coll.bounds.size.y * 0.5f +1f, transform.position.z);
        //yield return new WaitForSeconds(0.2f);
        //向前移动
        //transform.position = transform.position + new Vector3(1 * transform.localScale.x, 0, 0);
        transform.DOMove(new Vector3(transform.position.x, coll.bounds.center.y + coll.bounds.size.y * 0.5f + 0.3f, transform.position.z), 0.3f);
        yield return new WaitForSeconds(0.3f);
        transform.DOMove(new Vector3(transform.position.x + 1 * transform.localScale.x, transform.position.y, transform.position.z), 0.2f);
        //结束状态
        yield return new WaitForSeconds(0.2f);
        toWallToping = false;
        inputControl.GamePlay.Enable();
    }

    /*private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            //Debug.Log("leaveGround");
            if (rb.velocity.y>0)
            {
                Debug.Log("JumpLeaveGround");
            }
            else if (rb.velocity.y<=0 && (!physicsCheck.touchLeftWall || !physicsCheck.touchRightWall))
            {
                Debug.Log("FallLeaveGround");
                StartCoroutine(ExtendJump());
            }
            
        }
    }*/

    /*private IEnumerator ExtendJump()
    {
        //rb.velocity = new Vector2(rb.velocity.x, 0);
        isExtendJump = true;
        yield return new WaitForSeconds(extendJumpTime);
        isExtendJump = false;
    }*/

    private void Attack(InputAction.CallbackContext context)
    {
        if (playerSlipWall || playerClimbingWalling)
        {
            //停止攻击动画的播放(在animator中通过条件判断后修改)
            isAttack = false;
            return;
        }
        isAttack = true;
        if(physicsCheck.isGround) rb.velocity = new Vector2(0, rb.velocity.y);
        animationController.PlayAttack();
    }

    //滑铲
    private void Slid(InputAction.CallbackContext context)
    {
        if (sliding || !playerAbility.slid) return;
        if (character.currentEnergy < slidCostEnergy) { UIManager.instance.ShowMessage("能量不足"); /*Debug.Log("能量不足");*/ }//提示能量不够
        if(physicsCheck.isGround && !isHurt && !isDead && character.currentEnergy>= slidCostEnergy)
        {
            StartCoroutine(IESlid());
        }
    }
    private IEnumerator IESlid()
    {
        if (!character.UseEnergy(slidCostEnergy)) yield return null;//消耗能量，若失败，返回空
        Debug.Log("Slid");
        sliding = true;
        inputControl.GamePlay.Disable();
        //transform.DOMoveX(transform.position.x + 1, 0.2f);
        rb.velocity = new Vector2(2 * transform.localScale.x, rb.velocity.y);
        slidStart = true;
        yield return new WaitForSeconds(0.1f);
        slidStart = false;
        if(isHurt) sliding = false;

        if(sliding)
        {
            int excludedLayerMask = ((~(1 << 8))&(~(1 << 11))) & (~0);
            coll.contactCaptureLayers = excludedLayerMask;
            coll.excludeLayers = (1 << 11);
            //transform.DOMoveX(transform.position.x + slidDistance, slidTime);
            rb.velocity = new Vector2(slidSpeed * transform.localScale.x, rb.velocity.y);
            slidLoop = true;
            yield return new WaitForSeconds(slidTime);
            slidLoop = false;
            coll.contactCaptureLayers = ~0;
            coll.excludeLayers = 0;
            //transform.DOMoveX(transform.position.x + 1, 0.2f);
            //rb.velocity = new Vector2(2 * transform.localScale.x, rb.velocity.y);
            slidEnd = true;
            yield return new WaitForSeconds(0.1f);
            slidEnd = false;
            sliding = false;
        }
        if(!isDead) inputControl.GamePlay.Enable();
    }


    public void GetHurt(Vector3 hitPos,bool isSpike)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(transform.position.x - hitPos.x, 0).normalized;
        rb.AddForce(dir*hurtForce,ForceMode2D.Impulse);

    }
    //玩家死亡
    public void PlayerDead()
    {
        isDead = true;
        inputControl.GamePlay.Disable();
        //改变物理碰撞图层和碰撞检测图层，让玩家死后不会触发怪物巡逻和碰撞
        gameObject.layer = 2;
        int excludedLayerMask = (1 << 8/* | 1 << 3*/);//1左移8位，表示物理图层8，排除该图层碰撞
        coll.excludeLayers = excludedLayerMask;
        transform.Find("Sign").gameObject.SetActive(false);
        //显示GameOver界面
        UIManager.instance.SetGaemOverPanel(true);
        //禁用暂停菜单唤起
        inputControl.UI.Pause.Disable();
        inputControl.UI.Back.Disable();
    }

    //玩家复活
    public void PlayerRevive()
    {
        isDead = false;
        inputControl.GamePlay.Enable();
        //gameObject.layer = 7;
        coll.excludeLayers = 0;
        StartCoroutine(SetPlayerLayerLater());
        transform.Find("Sign").gameObject.SetActive(true);
        //关闭GameOver界面
        UIManager.instance.SetGaemOverPanel(false);
        UIManager.instance.SetSettingIconActive(true);
        //恢复暂停菜单唤起
        inputControl.UI.Pause.Enable();
    }
    //延迟设置玩家物理层，避免复活时怪物状态改变
    IEnumerator SetPlayerLayerLater()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public void CheckState()
    {
        coll.sharedMaterial = physicsCheck.isGround ? normalMaterial : wallMaterial;
    }

    //暂停游戏
    private void PauseGame(InputAction.CallbackContext context)
    {
        UIManager.instance.PauseGamePerform();
    }
    //返回
    private void BackPerform(InputAction.CallbackContext context)
    {
        //如果按下的是ESC键/*且当前settingPanel被激活*/且不是Menu场景，跳过此次返回功能(解决ESC返回两次问题)已弃用
        //应通过情景控制ESC和Back的enable和disable来进行功能的控制，原处理方式已弃用
        //if (inputControl.UI.Back.bindings[0].path.Replace("<","/").Replace(">","") == context.control.path /*&& UIManager.instance.settingPanel.activeInHierarchy*/ && SceneLoader.instance.currentLoadedScene.sceneType != SceneType.Menu) return;
        UIManager.instance.BackPerform();
    }

    /*private void TestStarted(InputAction.CallbackContext context)
    {
        Debug.Log("TestStarted");
    }

    private void TestPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("TestPerformed");
    }

    private void TestCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("TestCanceled");
    }

    private void TestStarted1()
    {
        Debug.Log("TestStarted1");
    }

    private void TestPerformed1()
    {
        Debug.Log("TestPerformed1");
        rb.AddForce(transform.up * jumpForce*//*, ForceMode2D.Impulse*//*);
    }

    private void TestCanceled1()
    {
        Debug.Log("TestCanceled1");
    }*/

    /*private void TestStarted2()
    {
        Debug.Log("TestStarted2");
    }

    private void TestPerformed2()
    {
        Debug.Log("TestPerformed2");
    }

    private void TestCanceled2()
    {
        Debug.Log("TestCanceled2");
    }*/
}
