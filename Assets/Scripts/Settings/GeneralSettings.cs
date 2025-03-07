using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSettings : MonoBehaviour
{
    private GeneralSettingsData generalSettingsData;

    public SettingsEffect_Bool showEnemyHP;
    public BoolEvenSO showEnemyHPEvenSO;
    public SettingsEffect_Bool showDamage;
    public BoolEvenSO showDamageEvenSO;
    public SettingsEffect_Bool isScreenShock;
    public BoolEvenSO isScreenShockEvenSO;
    public SettingsEffect_Float handleShock;
    public FloatEvenSO handleShockEvenSO;
    public SettingsEffect_Enum language;
    

    private void Start()
    {
        generalSettingsData = DataManager.instance.settingsData.generalSettingsData;
    }

    private void OnEnable()
    {
        showEnemyHPEvenSO.OnEventRised += OnShowEnemyHPEven;
        showDamageEvenSO.OnEventRised += OnShowDamageEven;
        isScreenShockEvenSO.OnEventRised += OnIsScreenShockEven;
        handleShockEvenSO.OnEventRised += OnHandleShockEven;
    }

    private void OnDisable()
    {
        showEnemyHPEvenSO.OnEventRised -= OnShowEnemyHPEven;
        showDamageEvenSO.OnEventRised -= OnShowDamageEven;
        isScreenShockEvenSO.OnEventRised -= OnIsScreenShockEven;
        handleShockEvenSO.OnEventRised -= OnHandleShockEven;
    }

    public void SetSettingsValue()
    {
        generalSettingsData = DataManager.instance.settingsData.generalSettingsData;
        showEnemyHP.ChangeValue(generalSettingsData.showEnemyHP);
        showDamage.ChangeValue(generalSettingsData.showDamage);
        isScreenShock.ChangeValue(generalSettingsData.isScreenShock);
        handleShock.settingsValue.value = generalSettingsData.handleShock;
        language.SetLanguage(generalSettingsData.language);
    }

    //修改和保存设置值
    public void OnShowEnemyHPEven(bool flag)
    {
        generalSettingsData.showEnemyHP = flag;
        DataManager.instance.SaveSettingsData();
    }
    //修改和保存设置值
    public void OnShowDamageEven(bool flag)
    {
        generalSettingsData.showDamage = flag;
        DataManager.instance.SaveSettingsData();
    }
    //修改和保存设置值
    public void OnIsScreenShockEven(bool flag)
    {
        generalSettingsData.isScreenShock = flag;
        DataManager.instance.SaveSettingsData();
    }
    //修改和保存设置值
    public void OnHandleShockEven(float val)
    {
        generalSettingsData.handleShock = val;
        DataManager.instance.SaveSettingsData();
    }
    //修改和保存设置值
    public void OnLanguageChange(Language language)
    {
        generalSettingsData.language = language;
        DataManager.instance.SaveSettingsData();
    }
}
