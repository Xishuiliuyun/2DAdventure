using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.IO;
using TMPro;
using Newtonsoft.Json;

public class Menu : MonoBehaviour
{
    public static Menu instance;

    public GameObject newGameButton;
    public GameObject shelterCanva;
    public Button newGameBtn;
    public Button continueBtn;
    public Button settingBtn;
    public Button exitBtn;

    //public GameObject tipsMessagePanel;//已弃用
    //public TextMeshProUGUI message;//已弃用

    public GameObject title;
    public GameObject allButton;
    private RectTransform titleRt;
    private RectTransform btnAreaRt;
    private float initTitlePosY;
    private Vector2 initButtonAreaSize;
    private Vector2 initButtonAreaPos;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
        //tipsMessagePanel.SetActive(false);

        titleRt = title.GetComponent<RectTransform>();
        btnAreaRt = allButton.GetComponent<RectTransform>();
        initTitlePosY = titleRt.anchoredPosition.y;
        initButtonAreaSize = btnAreaRt.sizeDelta;
        initButtonAreaPos = btnAreaRt.anchoredPosition;
    }

    private void Update()
    {
        if(Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_WEBGL)
        {
            if (Screen.fullScreen)
            {
                SetTitleAndBtn(ResolvingPower.Type1);
            }
            else
            {
                SetTitleAndBtn(ResolvingPower.Type3);
            }
        }
    }

    private void OnEnable()
    {
        if (Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_WEBGL)
        {
            SetTitleAndBtn(ResolvingPower.Type3);
        }
        else
        {
            //根据分辨率设置Menu场景的字体大小
            SetTitleAndBtn(DataManager.instance.settingsData.screensettingsData.resolvingPower);
        }
        //显示按键提示条(改为只在设置菜单才显示)
        //UIManager.instance.SettingTips.SetActive(true);
        EventSystem.current.SetSelectedGameObject(newGameButton);
    }

    private void OnDisable()
    {
        if(UIManager.instance.SettingTips !=null && UIManager.instance!=null)
        {
            //关闭按键提示条
            UIManager.instance.SettingTips.SetActive(false);
        }
    }

    public void SetButtonDisable()
    {
        //shelterCanva.SetActive(true);
        newGameBtn.enabled = false;
        continueBtn.enabled = false;
        exitBtn.enabled = false;
        settingBtn.enabled = false;
    }

    public void SetButtonEnable()
    {
        //shelterCanva.SetActive(true);
        newGameBtn.enabled = true;
        continueBtn.enabled = true;
        exitBtn.enabled = true;
        settingBtn.enabled = true;
    }

    public void ExitGame()
    {
        UIManager.instance.ShowPromptBox("确定退出游戏吗？", false);
        UIManager.instance.confirmBtn.onClick.AddListener(() => {
            //Debug.Log("退出游戏");
            UIManager.instance.ClosePromptBox();
            Application.Quit();
        });
        UIManager.instance.cancelBtn.onClick.AddListener(PressCancelBtn);
        
    }

    public void NewGame()
    {
        string path = Path.Combine(Application.persistentDataPath, Constants.SAVE_DATA_FILE_NAME);
        if (DataManager.instance.FileCheck(path))
        {//存在存档
            string json = System.IO.File.ReadAllText(path);
            Data data = JsonConvert.DeserializeObject<Data>(json);
            if (data.isGameOver)
            {//已通关游戏,直接开始新游戏
                DataManager.instance.saveData.isGameOver = false;
                SceneLoader.instance.NewGame();
            }
            else
            {//未通关游戏，正常走询问流程
                UIManager.instance.ShowPromptBox("确定要开始新的冒险吗？会覆盖原有存档", false);
                UIManager.instance.confirmBtn.onClick.AddListener(PressConfirmBtn);
                UIManager.instance.cancelBtn.onClick.AddListener(PressCancelBtn);
            }
        }
        else SceneLoader.instance.NewGame();//不存在存档，直接开始新游戏
    }

    public void ContinueGame()
    {
        //检查存档，没有文件时，弹出提示信息，存在存档时执行继续游戏的流程
        string path = Path.Combine(Application.persistentDataPath, Constants.SAVE_DATA_FILE_NAME);
        if (DataManager.instance.FileCheck(path))//存在存档
        {
            string json = System.IO.File.ReadAllText(path);
            Data data = JsonConvert.DeserializeObject<Data>(json);
            if(data.isGameOver)
            {//已通关游戏
                UIManager.instance.ShowPromptBox("已通关游戏，是否开始新的冒险？", true);
                UIManager.instance.confirmBtn.onClick.AddListener(PressConfirmBtn);
                UIManager.instance.cancelBtn.onClick.AddListener(PressCancelBtn);
            }
            else
            {//未通关游戏，正常走继续游戏流程
                SceneLoader.instance.ContinueGame();
            }
        }
        else//没有存档
        {
            //message.text = "当前没有存档数据，是否开始新的冒险？";
            //tipsMessagePanel.SetActive(true);
            UIManager.instance.ShowPromptBox("当前没有存档数据，是否开始新的冒险？",true);
            UIManager.instance.confirmBtn.onClick.AddListener(PressConfirmBtn);
            UIManager.instance.cancelBtn.onClick.AddListener(PressCancelBtn);
        }
    }

    //游戏设置
    public void GameSettings()
    {
        UIManager.instance.GameSetting();
        //隐藏Menu菜单
        title.SetActive(false);
        allButton.SetActive(false);
        UIManager.instance.menu = this;
    }
    //恢复Menu的UI项
    public void RecoverMenuUI()
    {
        title.SetActive(true);
        allButton.SetActive(true);
        EventSystem.current.SetSelectedGameObject(newGameButton);
    }

    public void PressConfirmBtn()
    {
        DataManager.instance.saveData.isGameOver = false;
        //tipsMessagePanel.SetActive(false);
        UIManager.instance.ClosePromptBox();
        SceneLoader.instance.NewGame();
    }

    public void PressCancelBtn()
    {
        //tipsMessagePanel.SetActive(false);
        UIManager.instance.ClosePromptBox();
        SetButtonEnable();
        EventSystem.current.SetSelectedGameObject(newGameButton);
    }

    //根据分辨率设置Menu场景的字体大小
    public void SetTitleAndBtn(ResolvingPower resolvingPower)
    {
        float fontSizeScale;
        float titlePosYScale;
        float btnAreaSizeScale;
        float buttonAreaPosScale;
        switch (resolvingPower)
        {
            case ResolvingPower.Type1:
                titlePosYScale = 1;
                fontSizeScale = 1;
                btnAreaSizeScale = 1;
                buttonAreaPosScale = 1;
                break;
            case ResolvingPower.Type2:
                titlePosYScale = fontSizeScale = btnAreaSizeScale = buttonAreaPosScale = 0.67f;
                break;
            case ResolvingPower.Type3:
                titlePosYScale = fontSizeScale = btnAreaSizeScale = buttonAreaPosScale = 0.53f;
                break;
            case ResolvingPower.Type4:
                titlePosYScale = fontSizeScale = btnAreaSizeScale = buttonAreaPosScale = 0.42f;
                break;
            case ResolvingPower.Type5:
                titlePosYScale = fontSizeScale = btnAreaSizeScale = buttonAreaPosScale = 0.75f;
                break;
            case ResolvingPower.Type6:
                titlePosYScale = buttonAreaPosScale = 0.8f;
                fontSizeScale = btnAreaSizeScale = 1f; 
                break;
            case ResolvingPower.Type7:
                titlePosYScale = fontSizeScale = btnAreaSizeScale = buttonAreaPosScale = 1f;
                break;
            case ResolvingPower.Type8:
                titlePosYScale = fontSizeScale = btnAreaSizeScale = buttonAreaPosScale = 1f;
                break;
            case ResolvingPower.Type9:
                titlePosYScale = fontSizeScale = btnAreaSizeScale = buttonAreaPosScale = 1f;
                break;
            case ResolvingPower.Type10:
                titlePosYScale = fontSizeScale = btnAreaSizeScale = buttonAreaPosScale = 1f;
                break;
            default:
                titlePosYScale = fontSizeScale = btnAreaSizeScale = buttonAreaPosScale = 1f;
                break;
        }
        SetFontSize(Constants.MENU_TITLE_FONT_SIZE * fontSizeScale, Constants.MENU_BTN_FONT_SIZE * fontSizeScale);
        btnAreaRt.sizeDelta = initButtonAreaSize * btnAreaSizeScale;
        btnAreaRt.anchoredPosition = initButtonAreaPos * buttonAreaPosScale;
        titleRt.anchoredPosition = new Vector2(titleRt.anchoredPosition.x, initTitlePosY * titlePosYScale);
    }

    public void SetFontSize(float titleSize,float btnSize)
    {
        title.GetComponent<TextMeshProUGUI>().fontSize = titleSize;
        for(int i = 0;i< allButton.transform.childCount;i++)
        {
            allButton.transform.GetChild(i).GetComponent<TextMeshProUGUI>().fontSize = btnSize;
        }
    }


}
