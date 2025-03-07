using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;//PS
using UnityEngine.InputSystem.XInput;//XBOX
//using UnityEngine.InputSystem.Switch;//打包时报错，暂时注释掉
using TMPro;//Switch


//可互动标识的功能实现
public class Sign : MonoBehaviour
{
    //public PlayerController playerController;
    private Animator anim;
    public Transform playerTrans;
    public GameObject signSprite;
    private bool canPress;
    private IInteractable targetItem;
    //可互动物体输入设备类型后续可以换成枚举控制
    private int interactableType;//可互动物体类型，0默认、1宝箱物品、2NPC可对话类
    public int inputControlType;//输入设备类型，0默认、1键鼠、2XBOX、3PS、4Switch

    private void Awake()
    {
        //anim = GetComponentInChildren<Animator>();//子物体未激活会获取不到，不适用
        anim = signSprite.GetComponent<Animator>();

    }

    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
        //playerController.inputControl.GamePlay.Confirm_E.started += OnConfirm_E;
        //playerController.inputControl.GamePlay.Confirm_F.started += OnConfirm_F;
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
        //playerController.inputControl.GamePlay.Confirm_E.started -= OnConfirm_E;
        //playerController.inputControl.GamePlay.Confirm_F.started -= OnConfirm_F;
    }


    private void Update()
    {
        signSprite.GetComponent<SpriteRenderer>().enabled = canPress;
        signSprite.transform.localScale = playerTrans.localScale;
    }

    public void OnConfirm_E(InputAction.CallbackContext context)
    {
        if(canPress && (interactableType == 1 || interactableType == 0))
        {
            targetItem.TriggerAction();
        }
    }
    public void OnConfirm_F(InputAction.CallbackContext context)
    {
        if (canPress && interactableType == 2)
        {
            targetItem.TriggerAction();
        }
    }

    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        if(actionChange == InputActionChange.ActionStarted)
        {
            //Debug.Log(((InputAction)obj).activeControl.device);

            var d = ((InputAction)obj).activeControl.device;

            switch (d.device) 
            {
                case Mouse:
                    inputControlType = 0;
                    break;
                case Keyboard://切换设备切换动画弃用，检测不够频繁，碰撞不同可互动物体时无法切换对应动画
                    /*if (interactableType == 0 || interactableType == 1)
                    { anim.Play("Keyboard_E"); }
                    else if (interactableType == 2)
                    { anim.Play("Keyboard_F"); }*/
                    inputControlType = 1;
                    break;
                case XInputController:
                    /*if (interactableType == 0 || interactableType == 1)
                    { anim.Play("Xbox_B"); }
                    else if (interactableType == 2)
                    { anim.Play("Xbox_Y"); }*/
                    inputControlType = 2;
                    break;
                case DualShockGamepad: 
                    inputControlType = 3; 
                    break;
                /*case SwitchProControllerHID: //打包时报错，暂时注释掉
                    inputControlType = 4; 
                    break;*/
                default:
                    inputControlType = 0;
                    break;
            }
            
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable")) 
        {
            interactableType = 1;
            canPress = true;
            targetItem = collision.GetComponent<IInteractable>();
            PlaySignAnim();
        }
        else if (collision.CompareTag("NPC")|| collision.CompareTag("SavePoint")) 
        { 
            interactableType = 2;
            canPress = true;
            targetItem = collision.GetComponent<IInteractable>();
            PlaySignAnim();
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("UNInteractable")|| collision.CompareTag("Interactable")|| collision.CompareTag("NPC")|| collision.CompareTag("SavePoint"))
        {
            canPress = false;
        }
    }

    private void PlaySignAnim()
    {
        if(interactableType == 1|| interactableType ==0)//宝箱物品
        {
            switch (inputControlType)
            {
                case 0:
                case 1://默认、键鼠
                    anim.Play("Keyboard_E");
                    break;
                case 2://XBOX
                    anim.Play("Xbox_B");
                    break;
                case 3://PS
                    anim.Play("PS_B");
                    break;
                case 4://Switch
                    anim.Play("Switch_B");
                    break;
            }
        }
        else if (interactableType == 2)//NPC可对话类
        {
            switch (inputControlType)
            {
                case 0:
                case 1://默认、键鼠
                    anim.Play("Keyboard_F");
                    break;
                case 2://XBOX
                    anim.Play("Xbox_Y");
                    break;
                case 3://PS
                    anim.Play("PS_Y");
                    break;
                case 4://Switch
                    anim.Play("Switch_Y");
                    break;
            }
        }
    }

    
}
