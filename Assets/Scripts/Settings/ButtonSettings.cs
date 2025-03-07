using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using static Constants;

public class ButtonSettings : MonoBehaviour
{

    public Button mouseKeyBtn;
    public Button handleBtn;
    public Selectable mouseDefaultOption;
    public Selectable handleDefaultOption;

    public Image mouseKeyImage;
    public Image handleImage;

    public GameObject mouseKeySettingsArea;
    public GameObject handleSettingsArea;

    public GameObject currentSelectedSettings;
    public Image currentSelectedImage;

    //按键设置数据
    public Dictionary<Keys_Keyboard, BindInfoData> Keys_KeyboardDict;
    public Dictionary<Keys_Gamepad, BindInfoData> Keys_GamepadDict;

    //处理鼠标点击取消按键绑定后的流程
    public Button coverPanel;//点击事件监听用
    public SettingsEffect_BtnSettings btnSettingScript;//实际已经不会被触发，可弃用

    private void Awake()
    {
        //已弃用
        //Keys_KeyboardDict = DataManager.instance.buttonSettingsData.Keys_KeyboardDict;
        //Keys_GamepadDict = DataManager.instance.buttonSettingsData.Keys_GamepadDict;
        
    }

    private void Start()
    {
        mouseKeyBtn.onClick.AddListener(() => { 
            SwitchOption(mouseKeySettingsArea, mouseKeyImage);
            UIManager.instance.SetNavigation(mouseDefaultOption);
            UIManager.instance.SetNavigation2(mouseKeyBtn, mouseDefaultOption);
            UIManager.instance.SetNavigation2(handleBtn, mouseDefaultOption);
        });
        handleBtn.onClick.AddListener(() =>
        {
            SwitchOption(handleSettingsArea, handleImage);
            UIManager.instance.SetNavigation(handleDefaultOption);
            UIManager.instance.SetNavigation2(mouseKeyBtn, handleDefaultOption);
            UIManager.instance.SetNavigation2(handleBtn, handleDefaultOption);
        });
        coverPanel.onClick.AddListener(CancelRebind);//实际已经不会被触发，可弃用
    }

    private void OnEnable()
    {
        //初始设置设置面板
        //设置默认选项
        EventSystem.current.SetSelectedGameObject(mouseKeyBtn.gameObject);
        //设置显示面板
        mouseKeySettingsArea.SetActive(false);
        handleSettingsArea.SetActive(false);
        currentSelectedSettings = null;
        SwitchOption(mouseKeySettingsArea, mouseKeyImage);
        UIManager.instance.SetNavigation(mouseDefaultOption);
        UIManager.instance.SetNavigation2(mouseKeyBtn, mouseDefaultOption);
        UIManager.instance.SetNavigation2(handleBtn, mouseDefaultOption);
    }

    //切换选项
    public void SwitchOption(GameObject selectPanel, Image selsectImage)
    {
        //关闭原Panel
        if (currentSelectedSettings != null) currentSelectedSettings.SetActive(false);
        //打开新Panel
        currentSelectedSettings = selectPanel;
        if (currentSelectedSettings != null) currentSelectedSettings.SetActive(true);
        //根据存档数据设定被设置项(已在DataManager处理)
        //设置原选择项Image
        if (currentSelectedImage != null) currentSelectedImage.color = new Color(currentSelectedImage.color.r, currentSelectedImage.color.g, currentSelectedImage.color.b, 0f);
        //设置被选择项Image
        currentSelectedImage = selsectImage;
        if (currentSelectedImage != null) currentSelectedImage.color = new Color(currentSelectedImage.color.r, currentSelectedImage.color.g, currentSelectedImage.color.b, IMAGE_DEFAULT_HALF_RGBA);
    }

    
    //处理鼠标点击取消按键绑定流程后设置当前选择项
    //实际已经不会被触发，可弃用
    public void CancelRebind()
    {
        if ( btnSettingScript != null)
        {
            btnSettingScript.CoverPanelClick();
        }
        else Debug.Log("CancelRebind_ERR");
    }

    


}
