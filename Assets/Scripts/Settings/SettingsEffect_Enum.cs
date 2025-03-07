using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsEffect_Enum : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject settings;
    public Image image_back;
    public TextMeshProUGUI settingsName;
    public TextMeshProUGUI settingsValue;
    public GameObject enumList;
    public GeneralSettings generalSettings;
    public ScreenSettings screenSettings;

    public EnumSettingsType enumSettingsType;
    //语言设置相关参数
    public Language settingsVal_Language = 0;
    public TMP_FontAsset chineseFont, englishFont, japaneseFont, koreanFont;
    //分辨率设置相关参数
    public ResolvingPower settingsVal_ResolvingPower;

    //画面设置相关
    private bool isFullScreen;
    public BoolEvenSO fullScreenSettingsEvenSO;

    //public EventTrigger settingsTrigger;
    public bool beSelected;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnEnable()
    {
        fullScreenSettingsEvenSO.OnEventRised += OnFullScreenSettingsEven;
        SetElementDisplay(false);
        StartCoroutine(SetRectTransform(settings, settingsValue.gameObject));
    }

    private void OnDisable()
    {
        fullScreenSettingsEvenSO.OnEventRised -= OnFullScreenSettingsEven;
    }

    public void OnSelect(BaseEventData eventData)
    {
        SetElementDisplay(true);
        beSelected = true;
    }
    public void OnDeselect(BaseEventData eventData)
    {
        SetElementDisplay(false);
        beSelected = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (beSelected) return;
        SetElementDisplay(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (beSelected) return;
        SetElementDisplay(false);
    }

    public void OnClick()
    {
        //显示enum列表
        enumList.SetActive(true);
        if (enumSettingsType == EnumSettingsType.Language)
        {
            //设置被选择项为当前语言，或者一直默认选择第一项也可以
            EventSystem.current.SetSelectedGameObject(enumList.transform.GetChild((int)settingsVal_Language + 1).gameObject);//第一个子物体被Panel占了
        }
        else if (enumSettingsType == EnumSettingsType.ResolvingPower)
        {
            EventSystem.current.SetSelectedGameObject(enumList.transform.GetChild((int)settingsVal_ResolvingPower + 1).gameObject);
        }
        else Debug.Log("ERR_SettingsEffect_Enum");

        UIManager.instance.SetLevel2GameObj(gameObject);
    }

    public void SetElementDisplay(bool isShow)
    {
        if (isShow)
        {
            image_back.color = new Color(image_back.color.r, image_back.color.g, image_back.color.b, 0.2f);
            Color color;
            ColorUtility.TryParseHtmlString("#0084FF", out color);
            settingsName.color = color;
            settingsValue.color = color;
        }
        else
        {
            image_back.color = new Color(image_back.color.r, image_back.color.g, image_back.color.b, 0f);
            settingsName.color = Color.white;
            settingsValue.color = Color.white;
        }

        StartCoroutine(SetRectTransform(settings, settingsValue.gameObject));
    }

    //自适应调整元素大小
    IEnumerator SetRectTransform(GameObject tragGameObject, GameObject sizeGameObject)
    {
        yield return null;
        float totalWidth = 0f;
        totalWidth = sizeGameObject.GetComponent<RectTransform>().anchoredPosition.x + sizeGameObject.GetComponent<RectTransform>().rect.size.x;
        tragGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(totalWidth, tragGameObject.GetComponent<RectTransform>().rect.size.y);
    }

    public void SetValue( Enum enumVal )
    {
        if (enumSettingsType == EnumSettingsType.Language)
        {
            SetLanguage((Language)enumVal);
        }
        else if (enumSettingsType == EnumSettingsType.ResolvingPower)
        {
            SetResolvingPower((ResolvingPower)enumVal);
        }
        else Debug.Log("ERR_SettingsEffect_Enum");
        //刷新自适应调整元素大小
        StartCoroutine(SetRectTransform(settings, settingsValue.gameObject));
    }


    public void SetLanguage(Language language)
    {
        //设置相关的值
        settingsVal_Language = language;
        //设置相关的UI显示相关
        switch(language)
        {
            case Language.Chinese:
                settingsValue.font = chineseFont;
                settingsValue.text = "中文";
                break;
            case Language.English:
                settingsValue.font = englishFont;
                settingsValue.text = "English";
                break;
            case Language.Japanese:
                settingsValue.font = japaneseFont;
                settingsValue.text = "日本語";
                break;
            case Language.Korean:
                settingsValue.font = koreanFont;
                settingsValue.text = "한국어";
                break;
            default://默认设置为中文
                Debug.Log("ERR_SettingsEffect_Enum_SetLanguage");
                settingsValue.font = chineseFont;
                settingsValue.text = "中文";
                break;
        }
        if (generalSettings == null) { Debug.Log("SetLanguageERR");return; }
        generalSettings.OnLanguageChange(language);
        //设置语言(当前功能未实装)

        //设置被选择项为语言设置项
        //EventSystem.current.SetSelectedGameObject(gameObject);

    }

    public void SetResolvingPower(ResolvingPower resolvingPower)
    {
        isFullScreen = DataManager.instance.settingsData.screensettingsData.fullScreen;
        settingsVal_ResolvingPower = resolvingPower;
        switch (resolvingPower)
        {
            case ResolvingPower.Type1:
                settingsValue.text = "1920x1080[16:9]";
                Screen.SetResolution(1920, 1080, isFullScreen);
                break;
            case ResolvingPower.Type2:
                settingsValue.text = "1280x720[16:9]";
                Screen.SetResolution(1280, 720, isFullScreen);
                break;
            case ResolvingPower.Type3:
                settingsValue.text = "1024x768[4:3]";
                Screen.SetResolution(1024, 768, isFullScreen);
                break;
            case ResolvingPower.Type4:
                settingsValue.text = "800x600[4:3]";
                Screen.SetResolution(800, 600, isFullScreen);
                break;
            case ResolvingPower.Type5:
                settingsValue.text = "1440x900[16:10]";
                Screen.SetResolution(1440, 900, isFullScreen);
                break;
            case ResolvingPower.Type6:
                settingsValue.text = "1920x810[21:9]";
                Screen.SetResolution(1920, 810, isFullScreen);
                break;
            case ResolvingPower.Type7:
                settingsValue.text = "中文";
                break;
            case ResolvingPower.Type8:
                settingsValue.text = "中文";
                break;
            case ResolvingPower.Type9:
                settingsValue.text = "中文";
                break;
            case ResolvingPower.Type10:
                settingsValue.text = "中文";
                break;
        }
        if(DataManager.instance.saveData.currentScene == "Menu") Menu.instance.SetTitleAndBtn(resolvingPower);
        if (screenSettings == null) { Debug.Log("SetResolvingPowerERR"); return; }
        screenSettings.OnResolvingPowerChange(resolvingPower);
    }


    //是否全屏设置
    public void OnFullScreenSettingsEven(bool flag)
    {
        isFullScreen = flag;
        if(flag)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }



}
