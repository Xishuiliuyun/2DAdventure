using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.DualShock;
//using UnityEngine.InputSystem.Switch;//���ʱ������ʱע�͵�
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using System;
using UnityEngine.UI;

public class SettingsTips : MonoBehaviour
{
    public TextMeshProUGUI tipText;
    public int inputControlType;//�����豸���ͣ�0Ĭ�ϡ�1����2XBOX��3PS��4Switch
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
                case Keyboard://�л��豸�л��������ã���ⲻ��Ƶ������ײ��ͬ�ɻ�������ʱ�޷��л���Ӧ����
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
                /*case SwitchProControllerHID://���ʱ������ʱע�͵�
                    inputControlType = 4;
                    break;*/
                default:
                    inputControlType = 0;
                    break;
            }
            //�豸�л�ʱ��ʾ��Ӧ�İ�����ʾ��Ϣ
            ChangeTipText(inputControlType);
            //isRecoveryBtn = false;
        }
        
    }

    //���������ж�����������ֱ����ü��󰴼�ʱ������ʾ����ʾ�쳣
    //���ð���ʱaction.RemoveBindingOverride(bindingIndex);��������ʱ�ἤ��OnActionChange�������Ҵ���ֵΪActionPerformed  Mouse:/Mouse���ᵼ�°�����ʾ����ʾ�쳣
    public void IsRecoveryBtn(bool flag)
    {
        isRecoveryBtn = flag;
    }

    public void ChangeTipText(int inputControlType)
    {
        //Debug.Log("ChangeTipText");
        //������ʾ�������ֱ�����ʱ��ʾ
        if (inputControlType == 2 || inputControlType == 3 || inputControlType == 4)
        {//��ʾ
            GetComponent<Image>().color = Color.black;
            tipText.color = Color.white;
        }
        else
        {//����
            GetComponent<Image>().color = Color.clear;
            tipText.color = Color.clear;
        }
        switch (inputControlType)
        {
            case 0:
                tipText.text = "�������ҡ�WSAD�ƶ�,enterȷ��,ESCȡ��";
                break;
            case 1:
                tipText.text = "�������ҡ�WSAD�ƶ�,enterȷ��,ESCȡ��";
                break;
            case 2:
                tipText.text = "��ҡ�ˡ���ҡ�ˡ�ʮ�ּ��ƶ�,A��ȷ��,B��ȡ��";
                break;
            case 3:
                tipText.text = "��ҡ�ˡ���ҡ�ˡ�ʮ�ּ��ƶ�,A��ȷ��,B��ȡ��";
                break;
            case 4:
                tipText.text = "��ҡ�ˡ���ҡ�ˡ�ʮ�ּ��ƶ�,A��ȷ��,B��ȡ��";
                break;
            default:
                tipText.text = "�������ҡ�WSAD�ƶ�,enterȷ��,ESCȡ��";
                break;

        }
    }
}
