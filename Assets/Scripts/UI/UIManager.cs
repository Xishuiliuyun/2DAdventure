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
    [Header("�¼�����")]
    public CharacterEventSO healthEvent;
    public SceneLoadEventSO sceneLoadEvent;

    //��ͣ��ص��������
    public GameObject gameOverPanel;
    public GameObject mobileTouch;
    public GameObject pausePanel;
    public GameObject settingIcon;
    public GameObject settingsObj;
    public GameObject settingPanel;
    public GameObject settingTips;//�˵��ײ�������ʾ��Ϣ

    //������ص��������
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
    public GameObject SettingTips;//������ʾ��
    //WEBGLʱ���õĻ���������
    public GameObject screenDelOption;
    public GameObject screenDelOption2;
    //WEBGLʱ���û�����������账���Navigation
    public Selectable dynamicBlurOption;//��̬ģ��
    public Selectable contrastRatioOption;//�Աȶ�


    //�������ý�����ѡ����ص�����
    public GameObject loadGame;//GameOver����
    public GameObject continueGame;//PauseGame����
    public GameObject generalSettings;//GameSetting����

    //Menu�˵��ű���setting����ʱ���룬���ڷ���Menu
    public Menu menu;

    //��ʾ�����
    public GameObject promptBoxPanel;
    public TextMeshProUGUI messageTMP;
    public TextMeshProUGUI titleTMP;
    public Button confirmBtn;
    public Button cancelBtn;

    //���л���
    //public bool isPC;
    //public bool isANDROID;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);

        InitPanelActive();

        //�����ã����ⰴ��������ͼ����ڳ�������ʱ������ʼ������Ϸ������
        /*if (Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_PC)
        {//PC����
            mobileTouch.SetActive(false);
            settingIcon.SetActive(false);
        }
        else if(Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_ANDROID)
        {
            //mobileTouch.SetActive(true);//(��������ʱ����)
            //settingIcon.SetActive(true);//(��������ʱ����)
        }
        else if (Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_WEBGL)
        {//WEBGL����
            //mobileTouch.SetActive(false);//(��������ʱ����)
            //settingIcon.SetActive(false);//(��������ʱ����)
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
            //WEBGL��Ŀ�����������⴦������ȫ���ͷֱ�������
            screensettingsButton.onClick.AddListener(() => {
                SwitchOption(screensettingsArea, screensettingsImage);
                currentLevel1Obj = screensettingsButton.gameObject;
                SetNavigation(screenDefaultOption2);
                //��������
                screenDelOption.SetActive(false);
                screenDelOption2.SetActive(false);
                SetNavigation3();
                //������ڵ�߶ȳߴ�
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

    //���س���ʱ�����������ж�UI�Ƿ���ʾ�������ã�������ScenenLoader�ű���ʵ��
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
        //����������ʾ����
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
    //ShowMessage���أ������ʾʱ�������������ʾʱ��
    public bool ShowMessage(String str,float disTime)
    {
        if (tween != null) tween.Kill();
        //����������ʾ����
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

    //����GaemOverPanel�Ƿ���ʾ
    public void SetGaemOverPanel(bool active)
    {
        gameOverPanel.SetActive(active);
        //SettingTips.SetActive(active);
        if (active) SetSettingIconActive(false); 
        if (active) EventSystem.current.SetSelectedGameObject(loadGame);
    }

    //�������ⰴ��������ͼ���Ƿ���ʾ
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

    //��ʼ��������弰UI�Ƿ���ʾ
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

    //��ͣ��Ϸ//��Ϊ��ͣ��������ͣʱ���ã��ú����еĶ����߼��Ѳ��ᱻִ����
    public void PauseGamePerform()
    {
        if (settingPanel.activeInHierarchy) BackPerform();//setting�˵�����ʱ,�߼�ӦΪִ�з��ع���
        else if(promptBoxPanel.activeInHierarchy) cancelBtn.onClick.Invoke();
        else
        {
            if (pausePanel.activeInHierarchy) RecoverGame();//��ͣ����Ѽ���
            else PauseGame();
        }
    }
    //�˵����水�·���
    public void BackPerform()
    {
        if (promptBoxPanel.activeInHierarchy) cancelBtn.onClick.Invoke();
        else if (settingPanel.activeInHierarchy)
        {//setting�˵�����ʱ����ESC��������
            //��3�����д���
            if(CheckIsLevel1())
            {//��ǰѡ���1��ѡ���ϣ�ֱ�ӷ�����һ��
                settingPanel.SetActive(false);
                SettingTips.SetActive(false);
                if (SceneLoader.instance.currentLoadedScene.sceneType != SceneType.Menu)
                {//ֻ�в�����Menu����ʱ����ʾpausePanel
                    pausePanel.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(continueGame);
                }
                else
                {//Menu����ʱ�ָ�Menu�������ʾ
                    menu.RecoverMenuUI();
                }
            }
            else
            {//��ǰѡ��û�д���1��ѡ���ϣ��ж��Ƿ���3��ѡ����
                //��⵱ǰѡ���Ƿ���3��ѡ����
                if (CheckIsLevel3(enumList1) || CheckIsLevel3(enumList2))
                {//��ǰѡ����3��ѡ��,����ʱ����2���˵�
                    if (currentLevel2Obj == null) { Debug.Log("BackPerform_backToLevel2ERR"); return; }
                    else
                    {
                        enumList1.gameObject.SetActive(false);
                        enumList2.gameObject.SetActive(false);
                        EventSystem.current.SetSelectedGameObject(currentLevel2Obj); 
                    }
                }
                else
                {//��ǰѡ���3��ѡ��,����ʱ����1���˵�
                    if (currentLevel1Obj == null) { Debug.Log("BackPerform_backToLevel1ERR"); return; }
                    else EventSystem.current.SetSelectedGameObject(currentLevel1Obj);
                }
            }
            
        }
        else if (pausePanel.activeInHierarchy) RecoverGame();//��ͣ���ʱ����
    }

    //�ָ���Ϸ
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

    //��ͣ��Ϸ
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

    //�������˵�
    public void BackToMenu()
    {
        Debug.Log("BackToMenu");
        pausePanel.SetActive(false);
        ShowPromptBox("ȷ��Ҫ�������˵���", false);
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

    //��Ϸ����
    public void GameSetting()
    {
        if(Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_WEBGL)
        {
            if(SceneLoader.instance.RUNTIME_ENVIRONMENT_WEBGL == Constants.RUNTIME_ENVIRONMENT_WEBGL_MOBILE)
            {//�ƶ��豸��WEBGL��������ʾ�������ù���
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
        {//ANDROID��������ʾ�������ù���
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
        //��ʾĬ��ѡ����
        SwitchOption(generalSettingsArea, generalSettingsImage);
        currentLevel1Obj = generalSettingsButton.gameObject;
        EventSystem.current.SetSelectedGameObject(generalSettings);
        SetNavigation(generalDefaultOption);
    }

    //�л�ѡ��
    public void SwitchOption(GameObject selectPanel,Image selsectImage)
    {
        //�ر�ԭPanel
        if (currentSelectedArea != null) currentSelectedArea.SetActive(false);
        //����Panel
        currentSelectedArea = selectPanel;
        if (currentSelectedArea != null) currentSelectedArea.SetActive(true);
        //���ݴ浵�����趨��������(����DataManager����)
        //����ԭѡ����Image
        if (currentSelectedImage != null) currentSelectedImage.color = new Color(currentSelectedImage.color.r, currentSelectedImage.color.g, currentSelectedImage.color.b, 0f);
        //���ñ�ѡ����Image
        currentSelectedImage = selsectImage;
        if (currentSelectedImage != null) currentSelectedImage.color = new Color(currentSelectedImage.color.r, currentSelectedImage.color.g, currentSelectedImage.color.b, 0.3f);
    }

    //������ʾ��isSingleBtn�Ƿ�ֻ��ȷ�ϰ�ť��str��ʾ��Ϣ��titleStr���� defaultBtn����Ĭ�ϰ�ťtrueʱ����Ϊȷ��falseʱΪȡ��
    public void ShowPromptBox(bool isSingleBtn, string str, string titleStr, bool defaultBtn)
    {
        //��������(��ť��str��title)
        if (isSingleBtn) cancelBtn.gameObject.SetActive(false);
        else cancelBtn.gameObject.SetActive(true);
        if (str != null) messageTMP.text = str; 
        else messageTMP.text = "Information not set";
        if (titleStr != null) titleTMP.text = titleStr;
        else titleTMP.text = "��ʾ";
        //����Ĭ����(Ĭ��Ϊȷ�ϰ�ť)
        if(isSingleBtn) EventSystem.current.SetSelectedGameObject(confirmBtn.gameObject);
        else
        {
            if (defaultBtn) EventSystem.current.SetSelectedGameObject(confirmBtn.gameObject);
            else EventSystem.current.SetSelectedGameObject(cancelBtn.gameObject);
        }
        //��ʾPanel
        promptBoxPanel.SetActive(true);
        //��հ�ť�ϰ󶨵��¼�
        confirmBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.RemoveAllListeners();
    }
    //��ʾ�����أ�Ĭ����ʾȷ�ϡ�ȡ��������ť������Ϊ����ʾ��
    public void ShowPromptBox(string str, bool defaultBtn)
    {
        //��������(��ť��str��title)
        cancelBtn.gameObject.SetActive(true);
        if (str != null) messageTMP.text = str;
        else messageTMP.text = "Information not set";
        titleTMP.text = "��ʾ";
        //����Ĭ����(Ĭ��Ϊȷ�ϰ�ť)
        if (defaultBtn) EventSystem.current.SetSelectedGameObject(confirmBtn.gameObject);
        else EventSystem.current.SetSelectedGameObject(cancelBtn.gameObject);
        //��ʾPanel
        promptBoxPanel.SetActive(true);
        //��հ�ť�ϰ󶨵��¼�
        confirmBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.RemoveAllListeners();
    }
    //��ʾ�����أ�isSingleBtn�Ƿ�ֻ��ȷ�ϰ�ť��str��ʾ��Ϣ��titleStr���� defaultBtn����Ĭ�ϰ�ťtrueʱ����Ϊȷ��falseʱΪȡ�� alpha͸����
    public void ShowPromptBox(bool isSingleBtn, string str, string titleStr, bool defaultBtn,float alpha)
    {
        //��������(��ť��str��title)
        if (isSingleBtn) cancelBtn.gameObject.SetActive(false);
        else cancelBtn.gameObject.SetActive(true);
        if (str != null) messageTMP.text = str;
        else messageTMP.text = "Information not set";
        if (titleStr != null) titleTMP.text = titleStr;
        else titleTMP.text = "��ʾ";
        //����Ĭ����(Ĭ��Ϊȷ�ϰ�ť)
        if (isSingleBtn) EventSystem.current.SetSelectedGameObject(confirmBtn.gameObject);
        else
        {
            if (defaultBtn) EventSystem.current.SetSelectedGameObject(confirmBtn.gameObject);
            else EventSystem.current.SetSelectedGameObject(cancelBtn.gameObject);
        }
        //����͸����
        Color color = promptBoxPanel.GetComponent<Image>().color;
        promptBoxPanel.GetComponent<Image>().color = new Color(color.r, color.g, color.b, alpha);
        //��ʾPanel
        promptBoxPanel.SetActive(true);
        //��հ�ť�ϰ󶨵��¼�
        confirmBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.RemoveAllListeners();
    }
    //�ر���ʾ��
    public void ClosePromptBox()
    {
        promptBoxPanel.SetActive(false);
    }
    //�ָ�͸��������
    public void RecoverAlpha()
    {
        //����͸����
        Color color = promptBoxPanel.GetComponent<Image>().color;
        promptBoxPanel.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.65f);

    }
    //����ȷ�ϰ�ť
    public void PressConfirmBtn()
    {
        Debug.Log("PressConfirmBtn");
        //tipsMessagePanel.SetActive(false);
        //SceneLoader.instance.NewGame();
    }
    //����ȡ����ť
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

    //�л�Panelʱ���ò��ְ�����Navigation
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

    //����������ѡ����ú����޸ĵ�Navigation
    public void SetNavigation3()
    {
        Navigation navigation = dynamicBlurOption.navigation;
        navigation.selectOnUp = contrastRatioOption;
        dynamicBlurOption.navigation = navigation;

        Navigation navigation2 = contrastRatioOption.navigation;
        navigation2.selectOnDown = dynamicBlurOption;
        contrastRatioOption.navigation = navigation2;
    }

    //���������ñ����ú����޸ĵ�Navigation
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
