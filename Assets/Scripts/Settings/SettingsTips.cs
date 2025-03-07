using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.DualShock;
//using UnityEngine.InputSystem.Switch;//打包时报错，暂时注释掉
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using System;
using UnityEngine.UI;

public class SettingsTips : MonoBehaviour
{
    public TextMeshProUGUI tipText;
    public int inputControlType;//输入设备类型，0默认、1键鼠、2XBOX、3PS、4Switch
    public bool isRecoveryBtn;

    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
        ChangeTipText(inputControlType);
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
    }

    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        if ((actionChange == InputActionChange.ActionStarted || actionChange == InputActionChange.ActionPerformed) && !isRecoveryBtn)
        {
            //Debug.Log(((InputAction)obj).activeControl.device);

            var d = ((InputAction)obj).activeControl.device;
            //Debug.Log(actionChange.ToString()+ "  " + d.device);
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
                /*case SwitchProControllerHID://打包时报错，暂时注释掉
                    inputControlType = 4;
                    break;*/
                default:
                    inputControlType = 0;
                    break;
            }
            //设备切换时显示对应的按键提示信息
            ChangeTipText(inputControlType);
            //isRecoveryBtn = false;
        }
        
    }

    //用于设置判定条件，解决手柄重置键鼠按键时按键提示条显示异常
    //重置按键时action.RemoveBindingOverride(bindingIndex);方法调用时会激活OnActionChange方法，且传入值为ActionPerformed  Mouse:/Mouse，会导致按键提示条显示异常
    public void IsRecoveryBtn(bool flag)
    {
        isRecoveryBtn = flag;
    }

    public void ChangeTipText(int inputControlType)
    {
        //Debug.Log("ChangeTipText");
        //按键提示条仅在手柄操作时显示
        if (inputControlType == 2 || inputControlType == 3 || inputControlType == 4)
        {//显示
            GetComponent<Image>().color = Color.black;
            tipText.color = Color.white;
        }
        else
        {//隐藏
            GetComponent<Image>().color = Color.clear;
            tipText.color = Color.clear;
        }
        switch (inputControlType)
        {
            case 0:
                tipText.text = "上下左右、WSAD移动,enter确认,ESC取消";
                break;
            case 1:
                tipText.text = "上下左右、WSAD移动,enter确认,ESC取消";
                break;
            case 2:
                tipText.text = "左摇杆、右摇杆、十字键移动,A键确认,B键取消";
                break;
            case 3:
                tipText.text = "左摇杆、右摇杆、十字键移动,A键确认,B键取消";
                break;
            case 4:
                tipText.text = "左摇杆、右摇杆、十字键移动,A键确认,B键取消";
                break;
            default:
                tipText.text = "上下左右、WSAD移动,enter确认,ESC取消";
                break;

        }
    }
}
