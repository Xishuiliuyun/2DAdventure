using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsEffect_Enum_BtnArea : MonoBehaviour
{
    public GameObject settings;
    public SettingsEffect_Enum settingsEffect;

    public Button panelBtn;
    public Button btn1;
    public Button btn2;
    public Button btn3;
    public Button btn4;
    public Button btn5;
    public Button btn6;
    public Button btn7;
    public Button btn8;
    public Button btn9;
    public Button btn10;

    private void Start()
    {
        gameObject.SetActive(false);
        panelBtn.onClick.AddListener(OnClick_panelBtn);

        if(settingsEffect.enumSettingsType == EnumSettingsType.Language)
        {
            btn1.onClick.AddListener(() => OnClick_Btn(Language.Chinese));
            btn2.onClick.AddListener(() => OnClick_Btn(Language.English));
            btn3.onClick.AddListener(() => OnClick_Btn(Language.Japanese));
            btn4.onClick.AddListener(() => OnClick_Btn(Language.Korean));
        }
        else if (settingsEffect.enumSettingsType == EnumSettingsType.ResolvingPower)
        {
            btn1.onClick.AddListener(() => OnClick_Btn(ResolvingPower.Type1));
            btn2.onClick.AddListener(() => OnClick_Btn(ResolvingPower.Type2));
            btn3.onClick.AddListener(() => OnClick_Btn(ResolvingPower.Type3));
            btn4.onClick.AddListener(() => OnClick_Btn(ResolvingPower.Type4));
            btn5.onClick.AddListener(() => OnClick_Btn(ResolvingPower.Type5));
            btn6.onClick.AddListener(() => OnClick_Btn(ResolvingPower.Type6));
            //btn7.onClick.AddListener(() => OnClick_Btn(ResolvingPower.Type7));
            //btn8.onClick.AddListener(() => OnClick_Btn(ResolvingPower.Type8));
            //btn9.onClick.AddListener(() => OnClick_Btn(ResolvingPower.Type9));
            //btn10.onClick.AddListener(() => OnClick_Btn(ResolvingPower.Type10));
        }
        else Debug.Log("ERR_SettingsEffect_Enum_BtnArea");
    }

    //点击非按钮区域，返回上一级，关闭此菜单
    public void OnClick_panelBtn()
    {
        Debug.Log("OnClick_SettingsEffect_Enum_BtnArea");
        //关闭选择语言菜单
        gameObject.SetActive(false);
        //设置被选择项为语言设置项
        EventSystem.current.SetSelectedGameObject(settings);
    }

    //点击按钮区域选项
    public void OnClick_Btn(Enum enumVal)
    {
        //设置相关的值
        //设置相关的UI显示相关
        settingsEffect.SetValue(enumVal);
        //设置被选择项为语言设置项
        EventSystem.current.SetSelectedGameObject(settings);
        //关闭选择语言菜单
        gameObject.SetActive(false);
        
    }

}
