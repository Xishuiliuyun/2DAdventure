using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public PlayerController playerController;

    public PlayerStateBar playerStateBar;
    public TextMeshProUGUI message;
    public bool isShowingMessage;
    public float disappearTime;
    private Tweener tween;
    [Header("事件监听")]
    public CharacterEventSO healthEvent;
    public SceneLoadEventSO sceneLoadEvent;

    //暂停相关的物体组件
    public GameObject gameOverPanel;
    public GameObject mobileTouch;
    public GameObject pausePanel;
    public GameObject settingIcon;
    public GameObject settingsObj;
    public GameObject settingPanel;
    public GameObject settingTips;//菜单底部操作提示信息

    //设置相关的物体组件
    public Image currentSelectedImage;
    public Image generalSettingsImage;
    public Image volumeSettingsImage;
    public Image screensettingsImage;
    public Image buttonsettingsImage;
    public GameObject currentSelectedArea;
    public GameObject generalSettingsArea;
    public GameObject volumeSettingsArea;
    public GameObject screensettingsArea;
    public GameObject buttonSettingsArea;
    public Button generalSettingsButton;
    public Button volumeSettingsButton;
    public Button screensettingsButton;
    public Button buttonSettingsButton;
    public Button backButton;
    public Selectable generalDefaultOption;
    public Selectable volumeDefaultOption;
    public Selectable screenDefaultOption;
    public Selectable screenDefaultOption2;
    public Selectable buttonDefaultOption;
    public Selectable backSelectable;
    public GameObject enumList1;
    public GameObject enumList2;
    public GameObject currentLevel1Obj;
    public GameObject currentLevel2Obj;
    public GameObject SettingTips;//按键提示条
    //WEBGL时禁用的画面设置项
    public GameObject screenDelOption;
    public GameObject screenDelOption2;
    //WEBGL时禁用画面设置项后需处理的Navigation
    public Selectable dynamicBlurOption;//动态模糊
    public Selectable contrastRatioOption;//对比度


    //用于设置界面首选项相关的物体
    public GameObject loadGame;//GameOver界面
    public GameObject continueGame;//PauseGame界面
    public GameObject generalSettings;//GameSetting界面

    //Menu菜单脚本，setting激活时传入，用于访问Menu
    public Menu menu;

    //提示框相关
    public GameObject promptBoxPanel;
    public TextMeshProUGUI messageTMP;
    public TextMeshProUGUI titleTMP;
    public Button confirmBtn;
    public Button cancelBtn;

    //运行环境
    //public bool isPC;
    //public bool isANDROID;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);

        InitPanelActive();

        //已弃用，虚拟按键和设置图标仅在场景加载时处理，初始进入游戏均禁用
        /*if (Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_PC)
        {//PC环境
            mobileTouch.SetActive(false);
            settingIcon.SetActive(false);
        }
        else if(Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_ANDROID)
        {
            //mobileTouch.SetActive(true);//(场景加载时处理)
            //settingIcon.SetActive(true);//(场景加载时处理)
        }
        else if (Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_WEBGL)
        {//WEBGL环境
            //mobileTouch.SetActive(false);//(场景加载时处理)
            //settingIcon.SetActive(false);//(场景加载时处理)
        }*/
    }

    private void Start()
    {
        generalSettingsButton.onClick.AddListener(() => {
            SwitchOption(generalSettingsArea, generalSettingsImage);
            currentLevel1Obj = generalSettingsButton.gameObject;
            SetNavigation(generalDefaultOption);
        });
        volumeSettingsButton.onClick.AddListener(() => {
            SwitchOption(volumeSettingsArea, volumeSettingsImage);
            currentLevel1Obj = volumeSettingsButton.gameObject;
            SetNavigation(volumeDefaultOption);
        });
        if(Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_WEBGL)
        {
            //WEBGL项目画面设置特殊处理，禁用全屏和分辨率设置
            screensettingsButton.onClick.AddListener(() => {
                SwitchOption(screensettingsArea, screensettingsImage);
                currentLevel1Obj = screensettingsButton.gameObject;
                SetNavigation(screenDefaultOption2);
                //禁用设置
                screenDelOption.SetActive(false);
                screenDelOption2.SetActive(false);
                SetNavigation3();
                //调整夫节点高度尺寸
                screensettingsArea.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(850, 400);
                screensettingsArea.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -280);
            });
        }
        else
        {
            screensettingsButton.onClick.AddListener(() => {
                SwitchOption(screensettingsArea, screensettingsImage);
                currentLevel1Obj = screensettingsButton.gameObject;
                SetNavigation(screenDefaultOption);
            });
        }
        buttonSettingsButton.onClick.AddListener(() => {
            SwitchOption(buttonSettingsArea, buttonsettingsImage);
            currentLevel1Obj = buttonSettingsButton.gameObject; 
            SetNavigation(buttonDefaultOption);
        });
        backButton.onClick.AddListener(BackPerform);

        //confirmBtn.onClick.AddListener(() => { });
        //cancelBtn.onClick.AddListener(() => { });
    }

    private void OnEnable()
    {
        healthEvent.OnEventRaised += OnHealthEvent;
        //sceneLoadEvent.LoadRequestEvent += OnsceneLoadEvent;
    }

    private void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;
        //sceneLoadEvent.LoadRequestEvent -= OnsceneLoadEvent;
    }

    //加载场景时监听，用于判断UI是否显示，已弃用，功能在ScenenLoader脚本中实现
    /*private void OnsceneLoadEvent(GameSceneSO sceneToLoad, Vector3 posToGo, bool fadeScreen)
    {
        var isLocation = (sceneToLoad.sceneType == SceneType.Loaction);
        playerStateBar.gameObject.SetActive(isLocation);
    }*/

    private void OnHealthEvent(Character character)
    {
        var persentage = character.currentHealth / character.maxHealth;
        playerStateBar.OnHealthChange(persentage);
    }

    public void EnergyChange(Character character)
    {
        var persentage = character.currentEnergy / character.maxEnergy;
        playerStateBar.EnergyChange(persentage);
    }

    public void ChangeHPFrame(float initHP,float afterHP)
    {
        playerStateBar.isHPGrow = true;
        playerStateBar.ChangeHPFrame(initHP, afterHP);
    }

    public void ChangeEnergyFrame(float initEnergy, float afterEnergy)
    {
        playerStateBar.isEnergyGrow = true;
        playerStateBar.ChangEnergyFrame(initEnergy, afterEnergy);
    }

    public bool ChangeKeyNumber(int num)
    {
        return playerStateBar.ChangeKeyNumber(num);
    }

    public bool ShowMessage(String str )
    {
        if (tween != null) tween.Kill();
        //处理连续显示内容
        if(isShowingMessage)
        {
            if (message.text.Contains(str)) str = message.text;
            else str = message.text + "\n" + str;
        }
        //if (!message.text.Contains(str) && isShowingMessage) str = message.text + "\n" + str;
        message.enabled = true;
        isShowingMessage = true;
        message.text = str;
        message.alpha = 1;
        tween = message.DOFade(0, disappearTime);
        tween.OnComplete(SetState);

        return true;
    }
    //ShowMessage重载，添加显示时间参数，控制显示时间
    public bool ShowMessage(String str,float disTime)
    {
        if (tween != null) tween.Kill();
        //处理连续显示内容
        if (isShowingMessage)
        {
            if (message.text.Contains(str)) str = message.text;
            else str = message.text + "\n" + str;
        }
        //if (!message.text.Contains(str) && isShowingMessage) str = message.text + "\n" + str;
        message.enabled = true;
        isShowingMessage = true;
        message.text = str;
        message.alpha = 1;
        tween = message.DOFade(0, disTime);
        tween.OnComplete(SetState);
        return true;
    }

    public void SetState()
    {
        isShowingMessage = false;
        message.enabled = false;
    }

    //控制GaemOverPanel是否显示
    public void SetGaemOverPanel(bool active)
    {
        gameOverPanel.SetActive(active);
        //SettingTips.SetActive(active);
        if (active) SetSettingIconActive(false); 
        if (active) EventSystem.current.SetSelectedGameObject(loadGame);
    }

    //控制虚拟按键和设置图标是否显示
    public void SetSettingIconActive(bool active)
    {
        Debug.Log(SceneLoader.instance.RUNTIME_ENVIRONMENT_WEBGL);
        if (Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_ANDROID || SceneLoader.instance.RUNTIME_ENVIRONMENT_WEBGL == Constants.RUNTIME_ENVIRONMENT_WEBGL_MOBILE)
        {
            mobileTouch.SetActive(active);
            settingIcon.SetActive(active);
        }
        else return;
    }

    //初始化设置面板及UI是否显示
    public void InitPanelActive()
    {
        pausePanel.SetActive(false);
        settingsObj.SetActive(false);
        settingPanel.SetActive(false);
        SettingTips.SetActive(false);
        mobileTouch.SetActive(false);
        settingIcon.SetActive(false);
        generalSettingsArea.SetActive(false);
        volumeSettingsArea.SetActive(false);
        screensettingsArea.SetActive(false);
        buttonSettingsArea.SetActive(false);
        promptBoxPanel.SetActive(false);
    }

    //暂停游戏//因为暂停按键在暂停时禁用，该函数中的多数逻辑已不会被执行了
    public void PauseGamePerform()
    {
        if (settingPanel.activeInHierarchy) BackPerform();//setting菜单开启时,逻辑应为执行返回功能
        else if(promptBoxPanel.activeInHierarchy) cancelBtn.onClick.Invoke();
        else
        {
            if (pausePanel.activeInHierarchy) RecoverGame();//暂停面板已激活
            else PauseGame();
        }
    }
    //菜单界面按下返回
    public void BackPerform()
    {
        if (promptBoxPanel.activeInHierarchy) cancelBtn.onClick.Invoke();
        else if (settingPanel.activeInHierarchy)
        {//setting菜单开启时按下ESC或点击返回
            //分3级进行处理
            if(CheckIsLevel1())
            {//当前选项处在1级选项上，直接返回上一级
                settingPanel.SetActive(false);
                SettingTips.SetActive(false);
                if (SceneLoader.instance.currentLoadedScene.sceneType != SceneType.Menu)
                {//只有不是在Menu界面时才显示pausePanel
                    pausePanel.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(continueGame);
                }
                else
                {//Menu界面时恢复Menu界面的显示
                    menu.RecoverMenuUI();
                }
            }
            else
            {//当前选项没有处在1级选项上，判断是否在3级选项上
                //检测当前选项是否在3级选项上
                if (CheckIsLevel3(enumList1) || CheckIsLevel3(enumList2))
                {//当前选项在3级选项,返回时返回2级菜单
                    if (currentLevel2Obj == null) { Debug.Log("BackPerform_backToLevel2ERR"); return; }
                    else
                    {
                        enumList1.gameObject.SetActive(false);
                        enumList2.gameObject.SetActive(false);
                        EventSystem.current.SetSelectedGameObject(currentLevel2Obj); 
                    }
                }
                else
                {//当前选项不在3级选项,返回时返回1级菜单
                    if (currentLevel1Obj == null) { Debug.Log("BackPerform_backToLevel1ERR"); return; }
                    else EventSystem.current.SetSelectedGameObject(currentLevel1Obj);
                }
            }
            
        }
        else if (pausePanel.activeInHierarchy) RecoverGame();//暂停面板时返回
    }

    //恢复游戏
    public void RecoverGame()
    {
        Debug.Log("RecoverGame");
        pausePanel.SetActive(false);
        SettingTips.SetActive(false);
        SetSettingIconActive(true);
        playerController.inputControl.UI.Pause.Enable();
        playerController.inputControl.UI.Back.Disable();
        Time.timeScale = 1;
        playerController.inputControl.GamePlay.Enable();
    }

    //暂停游戏
    public void PauseGame()
    {
        Debug.Log("PauseGame");
        pausePanel.SetActive(true);
        SetSettingIconActive(false);
        //SettingTips.SetActive(true);
        EventSystem.current.SetSelectedGameObject(continueGame);
        Time.timeScale = 0;
        playerController.inputControl.GamePlay.Disable();
        playerController.inputControl.UI.Pause.Disable();
        playerController.inputControl.UI.Back.Enable();
    }

    //返回主菜单
    public void BackToMenu()
    {
        Debug.Log("BackToMenu");
        pausePanel.SetActive(false);
        ShowPromptBox("确定要返回主菜单吗？", false);
        confirmBtn.onClick.AddListener(() => {
            playerController.inputControl.UI.Pause.Disable();
            playerController.inputControl.UI.Back.Disable();
            Time.timeScale = 1;
            ClosePromptBox();
            SceneLoader.instance.BackToMenu();
        });
        cancelBtn.onClick.AddListener(() => {
            PauseGame();
            ClosePromptBox();
        });

    }

    //游戏设置
    public void GameSetting()
    {
        if(Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_WEBGL)
        {
            if(SceneLoader.instance.RUNTIME_ENVIRONMENT_WEBGL == Constants.RUNTIME_ENVIRONMENT_WEBGL_MOBILE)
            {//移动设备的WEBGL环境不显示按键设置功能
                buttonSettingsButton.gameObject.SetActive(false);
                SetNavigation4();
            }
            else
            {
                buttonSettingsButton.gameObject.SetActive(true);
            }
            SettingTips.SetActive(false);
        }
        else if (Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_ANDROID)
        {//ANDROID环境不显示按键设置功能
            buttonSettingsButton.gameObject.SetActive(false);
            SetNavigation4();
            SettingTips.SetActive(false);
        }
        else SettingTips.SetActive(true);

        Debug.Log("GameSetting");
        pausePanel.SetActive(false);
        settingsObj.SetActive(true);
        settingPanel.SetActive(true);
        playerController.inputControl.UI.Pause.Disable();
        playerController.inputControl.UI.Back.Enable();
        //显示默认选择项
        SwitchOption(generalSettingsArea, generalSettingsImage);
        currentLevel1Obj = generalSettingsButton.gameObject;
        EventSystem.current.SetSelectedGameObject(generalSettings);
        SetNavigation(generalDefaultOption);
    }

    //切换选项
    public void SwitchOption(GameObject selectPanel,Image selsectImage)
    {
        //关闭原Panel
        if (currentSelectedArea != null) currentSelectedArea.SetActive(false);
        //打开新Panel
        currentSelectedArea = selectPanel;
        if (currentSelectedArea != null) currentSelectedArea.SetActive(true);
        //根据存档数据设定被设置项(已在DataManager处理)
        //设置原选择项Image
        if (currentSelectedImage != null) currentSelectedImage.color = new Color(currentSelectedImage.color.r, currentSelectedImage.color.g, currentSelectedImage.color.b, 0f);
        //设置被选择项Image
        currentSelectedImage = selsectImage;
        if (currentSelectedImage != null) currentSelectedImage.color = new Color(currentSelectedImage.color.r, currentSelectedImage.color.g, currentSelectedImage.color.b, 0.3f);
    }

    //弹出提示框，isSingleBtn是否只有确认按钮，str提示信息，titleStr标题 defaultBtn设置默认按钮true时设置为确认false时为取消
    public void ShowPromptBox(bool isSingleBtn, string str, string titleStr, bool defaultBtn)
    {
        //调整内容(按钮、str、title)
        if (isSingleBtn) cancelBtn.gameObject.SetActive(false);
        else cancelBtn.gameObject.SetActive(true);
        if (str != null) messageTMP.text = str; 
        else messageTMP.text = "Information not set";
        if (titleStr != null) titleTMP.text = titleStr;
        else titleTMP.text = "提示";
        //设置默认项(默认为确认按钮)
        if(isSingleBtn) EventSystem.current.SetSelectedGameObject(confirmBtn.gameObject);
        else
        {
            if (defaultBtn) EventSystem.current.SetSelectedGameObject(confirmBtn.gameObject);
            else EventSystem.current.SetSelectedGameObject(cancelBtn.gameObject);
        }
        //显示Panel
        promptBoxPanel.SetActive(true);
        //清空按钮上绑定的事件
        confirmBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.RemoveAllListeners();
    }
    //提示框重载，默认显示确认、取消两个按钮，标题为“提示”
    public void ShowPromptBox(string str, bool defaultBtn)
    {
        //调整内容(按钮、str、title)
        cancelBtn.gameObject.SetActive(true);
        if (str != null) messageTMP.text = str;
        else messageTMP.text = "Information not set";
        titleTMP.text = "提示";
        //设置默认项(默认为确认按钮)
        if (defaultBtn) EventSystem.current.SetSelectedGameObject(confirmBtn.gameObject);
        else EventSystem.current.SetSelectedGameObject(cancelBtn.gameObject);
        //显示Panel
        promptBoxPanel.SetActive(true);
        //清空按钮上绑定的事件
        confirmBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.RemoveAllListeners();
    }
    //提示框重载，isSingleBtn是否只有确认按钮，str提示信息，titleStr标题 defaultBtn设置默认按钮true时设置为确认false时为取消 alpha透明度
    public void ShowPromptBox(bool isSingleBtn, string str, string titleStr, bool defaultBtn,float alpha)
    {
        //调整内容(按钮、str、title)
        if (isSingleBtn) cancelBtn.gameObject.SetActive(false);
        else cancelBtn.gameObject.SetActive(true);
        if (str != null) messageTMP.text = str;
        else messageTMP.text = "Information not set";
        if (titleStr != null) titleTMP.text = titleStr;
        else titleTMP.text = "提示";
        //设置默认项(默认为确认按钮)
        if (isSingleBtn) EventSystem.current.SetSelectedGameObject(confirmBtn.gameObject);
        else
        {
            if (defaultBtn) EventSystem.current.SetSelectedGameObject(confirmBtn.gameObject);
            else EventSystem.current.SetSelectedGameObject(cancelBtn.gameObject);
        }
        //设置透明度
        Color color = promptBoxPanel.GetComponent<Image>().color;
        promptBoxPanel.GetComponent<Image>().color = new Color(color.r, color.g, color.b, alpha);
        //显示Panel
        promptBoxPanel.SetActive(true);
        //清空按钮上绑定的事件
        confirmBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.RemoveAllListeners();
    }
    //关闭提示框
    public void ClosePromptBox()
    {
        promptBoxPanel.SetActive(false);
    }
    //恢复透明度设置
    public void RecoverAlpha()
    {
        //设置透明度
        Color color = promptBoxPanel.GetComponent<Image>().color;
        promptBoxPanel.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.65f);

    }
    //按下确认按钮
    public void PressConfirmBtn()
    {
        Debug.Log("PressConfirmBtn");
        //tipsMessagePanel.SetActive(false);
        //SceneLoader.instance.NewGame();
    }
    //按下取消按钮
    public void PressCancelBtn()
    {
        Debug.Log("PressCancelBtn");
        //tipsMessagePanel.SetActive(false);
        //SetButtonEnable();
    }

    public bool CheckIsLevel1()
    {
        bool iRet = false;
        var current = EventSystem.current.currentSelectedGameObject;
        if (current == generalSettingsButton.gameObject || current == volumeSettingsButton.gameObject || current == screensettingsButton.gameObject || current == buttonSettingsButton.gameObject || current == backButton.gameObject) { iRet = true; }
        return iRet;
    }

    public void SetLevel2GameObj(GameObject gameObject)
    {
        currentLevel2Obj = gameObject;
    }

    public bool CheckIsLevel3(GameObject enumList)
    {
        bool iRet = false;
        var current = EventSystem.current.currentSelectedGameObject;
        foreach (Transform child in enumList.transform)
        {
            if (child.gameObject == current) { iRet = true; break; }
        }
        return iRet;
    }

    //切换Panel时设置部分按键的Navigation
    public void SetNavigation(Selectable defaultOption)
    {
        SetNavigation1(generalSettingsButton, defaultOption);
        SetNavigation1(volumeSettingsButton, defaultOption);
        SetNavigation1(screensettingsButton, defaultOption);
        SetNavigation1(buttonSettingsButton, defaultOption);
        SetNavigation1(backButton, defaultOption);
        EventSystem.current.SetSelectedGameObject(defaultOption.gameObject);
    }
    public void SetNavigation1(Button button, Selectable defaultOption)
    {
        Navigation navigation = button.navigation;
        navigation.selectOnRight = defaultOption;
        button.navigation = navigation;
    }
    public void SetNavigation2(Button button, Selectable defaultOption)
    {
        Navigation navigation = button.navigation;
        navigation.selectOnDown = defaultOption;
        button.navigation = navigation;
    }

    //处理画面设置选项被禁用后需修改的Navigation
    public void SetNavigation3()
    {
        Navigation navigation = dynamicBlurOption.navigation;
        navigation.selectOnUp = contrastRatioOption;
        dynamicBlurOption.navigation = navigation;

        Navigation navigation2 = contrastRatioOption.navigation;
        navigation2.selectOnDown = dynamicBlurOption;
        contrastRatioOption.navigation = navigation2;
    }

    //处理按键设置被禁用后需修改的Navigation
    public void SetNavigation4()
    {
        Navigation navigation = screensettingsButton.navigation;
        navigation.selectOnDown = backSelectable;
        screensettingsButton.navigation = navigation;

        Navigation navigation2 = backSelectable.navigation;
        navigation2.selectOnUp = screensettingsButton;
        backSelectable.navigation = navigation2;
    }


}
