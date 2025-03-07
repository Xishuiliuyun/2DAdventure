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

    //������������
    public Dictionary<Keys_Keyboard, BindInfoData> Keys_KeyboardDict;
    public Dictionary<Keys_Gamepad, BindInfoData> Keys_GamepadDict;

    //���������ȡ�������󶨺������
    public Button coverPanel;//����¼�������
    public SettingsEffect_BtnSettings btnSettingScript;//ʵ���Ѿ����ᱻ������������

    private void Awake()
    {
        //������
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
        coverPanel.onClick.AddListener(CancelRebind);//ʵ���Ѿ����ᱻ������������
    }

    private void OnEnable()
    {
        //��ʼ�����������
        //����Ĭ��ѡ��
        EventSystem.current.SetSelectedGameObject(mouseKeyBtn.gameObject);
        //������ʾ���
        mouseKeySettingsArea.SetActive(false);
        handleSettingsArea.SetActive(false);
        currentSelectedSettings = null;
        SwitchOption(mouseKeySettingsArea, mouseKeyImage);
        UIManager.instance.SetNavigation(mouseDefaultOption);
        UIManager.instance.SetNavigation2(mouseKeyBtn, mouseDefaultOption);
        UIManager.instance.SetNavigation2(handleBtn, mouseDefaultOption);
    }

    //�л�ѡ��
    public void SwitchOption(GameObject selectPanel, Image selsectImage)
    {
        //�ر�ԭPanel
        if (currentSelectedSettings != null) currentSelectedSettings.SetActive(false);
        //����Panel
        currentSelectedSettings = selectPanel;
        if (currentSelectedSettings != null) currentSelectedSettings.SetActive(true);
        //���ݴ浵�����趨��������(����DataManager����)
        //����ԭѡ����Image
        if (currentSelectedImage != null) currentSelectedImage.color = new Color(currentSelectedImage.color.r, currentSelectedImage.color.g, currentSelectedImage.color.b, 0f);
        //���ñ�ѡ����Image
        currentSelectedImage = selsectImage;
        if (currentSelectedImage != null) currentSelectedImage.color = new Color(currentSelectedImage.color.r, currentSelectedImage.color.g, currentSelectedImage.color.b, IMAGE_DEFAULT_HALF_RGBA);
    }

    
    //���������ȡ�����������̺����õ�ǰѡ����
    //ʵ���Ѿ����ᱻ������������
    public void CancelRebind()
    {
        if ( btnSettingScript != null)
        {
            btnSettingScript.CoverPanelClick();
        }
        else Debug.Log("CancelRebind_ERR");
    }

    


}
