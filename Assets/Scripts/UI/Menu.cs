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

    //public GameObject tipsMessagePanel;//������
    //public TextMeshProUGUI message;//������

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
            //���ݷֱ�������Menu�����������С
            SetTitleAndBtn(DataManager.instance.settingsData.screensettingsData.resolvingPower);
        }
        //��ʾ������ʾ��(��Ϊֻ�����ò˵�����ʾ)
        //UIManager.instance.SettingTips.SetActive(true);
        EventSystem.current.SetSelectedGameObject(newGameButton);
    }

    private void OnDisable()
    {
        if(UIManager.instance.SettingTips !=null && UIManager.instance!=null)
        {
            //�رհ�����ʾ��
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
        UIManager.instance.ShowPromptBox("ȷ���˳���Ϸ��", false);
        UIManager.instance.confirmBtn.onClick.AddListener(() => {
            //Debug.Log("�˳���Ϸ");
            UIManager.instance.ClosePromptBox();
            Application.Quit();
        });
        UIManager.instance.cancelBtn.onClick.AddListener(PressCancelBtn);
        
    }

    public void NewGame()
    {
        string path = Path.Combine(Application.persistentDataPath, Constants.SAVE_DATA_FILE_NAME);
        if (DataManager.instance.FileCheck(path))
        {//���ڴ浵
            string json = System.IO.File.ReadAllText(path);
            Data data = JsonConvert.DeserializeObject<Data>(json);
            if (data.isGameOver)
            {//��ͨ����Ϸ,ֱ�ӿ�ʼ����Ϸ
                DataManager.instance.saveData.isGameOver = false;
                SceneLoader.instance.NewGame();
            }
            else
            {//δͨ����Ϸ��������ѯ������
                UIManager.instance.ShowPromptBox("ȷ��Ҫ��ʼ�µ�ð���𣿻Ḳ��ԭ�д浵", false);
                UIManager.instance.confirmBtn.onClick.AddListener(PressConfirmBtn);
                UIManager.instance.cancelBtn.onClick.AddListener(PressCancelBtn);
            }
        }
        else SceneLoader.instance.NewGame();//�����ڴ浵��ֱ�ӿ�ʼ����Ϸ
    }

    public void ContinueGame()
    {
        //���浵��û���ļ�ʱ��������ʾ��Ϣ�����ڴ浵ʱִ�м�����Ϸ������
        string path = Path.Combine(Application.persistentDataPath, Constants.SAVE_DATA_FILE_NAME);
        if (DataManager.instance.FileCheck(path))//���ڴ浵
        {
            string json = System.IO.File.ReadAllText(path);
            Data data = JsonConvert.DeserializeObject<Data>(json);
            if(data.isGameOver)
            {//��ͨ����Ϸ
                UIManager.instance.ShowPromptBox("��ͨ����Ϸ���Ƿ�ʼ�µ�ð�գ�", true);
                UIManager.instance.confirmBtn.onClick.AddListener(PressConfirmBtn);
                UIManager.instance.cancelBtn.onClick.AddListener(PressCancelBtn);
            }
            else
            {//δͨ����Ϸ�������߼�����Ϸ����
                SceneLoader.instance.ContinueGame();
            }
        }
        else//û�д浵
        {
            //message.text = "��ǰû�д浵���ݣ��Ƿ�ʼ�µ�ð�գ�";
            //tipsMessagePanel.SetActive(true);
            UIManager.instance.ShowPromptBox("��ǰû�д浵���ݣ��Ƿ�ʼ�µ�ð�գ�",true);
            UIManager.instance.confirmBtn.onClick.AddListener(PressConfirmBtn);
            UIManager.instance.cancelBtn.onClick.AddListener(PressCancelBtn);
        }
    }

    //��Ϸ����
    public void GameSettings()
    {
        UIManager.instance.GameSetting();
        //����Menu�˵�
        title.SetActive(false);
        allButton.SetActive(false);
        UIManager.instance.menu = this;
    }
    //�ָ�Menu��UI��
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

    //���ݷֱ�������Menu�����������С
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
