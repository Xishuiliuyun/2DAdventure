using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSettings : MonoBehaviour
{
    private ScreenSettingsData screenSettingsData;

    public SettingsEffect_Bool fullScreenSettings;
    public BoolEvenSO fullScreenSettingsEvenSO;
    public SettingsEffect_Bool dynamicBlur;
    public BoolEvenSO dynamicBlurEvenSO;
    public SettingsEffect_Bool verticalSynchronization;
    public BoolEvenSO verticalSynchronizationEvenSO;
    public SettingsEffect_Float brightness;
    public FloatEvenSO brightnessEvenSO;
    public SettingsEffect_Float contrastRatio;
    public FloatEvenSO contrastRatioEvenSO;
    public SettingsEffect_Enum resolvingPower;


    private void Start()
    {
        screenSettingsData = DataManager.instance.settingsData.screensettingsData;
    }

    private void OnEnable()
    {
        fullScreenSettingsEvenSO.OnEventRised += OnFullScreenSettingsEven;
        dynamicBlurEvenSO.OnEventRised += OnDynamicBlurEven;
        verticalSynchronizationEvenSO.OnEventRised += OnVerticalSynchronizationEven;
        brightnessEvenSO.OnEventRised += OnBrightnessEven;
        contrastRatioEvenSO.OnEventRised += OnContrastRatioEven;
    }

    private void OnDisable()
    {
        fullScreenSettingsEvenSO.OnEventRised -= OnFullScreenSettingsEven;
        dynamicBlurEvenSO.OnEventRised -= OnDynamicBlurEven;
        verticalSynchronizationEvenSO.OnEventRised -= OnVerticalSynchronizationEven;
        brightnessEvenSO.OnEventRised -= OnBrightnessEven;
        contrastRatioEvenSO.OnEventRised -= OnContrastRatioEven;
    }

    public void SetSettingsValue()
    {
        screenSettingsData = DataManager.instance.settingsData.screensettingsData;
        fullScreenSettings.ChangeValue(screenSettingsData.fullScreen);
        dynamicBlur.ChangeValue(screenSettingsData.dynamicBlur);
        verticalSynchronization.ChangeValue(screenSettingsData.verticalSynchronization);
        brightness.settingsValue.value = screenSettingsData.brightness;
        contrastRatio.settingsValue.value = screenSettingsData.contrastRatio;
        resolvingPower.SetResolvingPower(screenSettingsData.resolvingPower);
    }

    public void OnFullScreenSettingsEven(bool flag)
    {
        screenSettingsData.fullScreen = flag;
        DataManager.instance.SaveSettingsData();
    }
    public void OnDynamicBlurEven(bool flag)
    {
        screenSettingsData.dynamicBlur = flag;
        DataManager.instance.SaveSettingsData();
    }
    public void OnVerticalSynchronizationEven(bool flag)
    {
        screenSettingsData.verticalSynchronization = flag;
        DataManager.instance.SaveSettingsData();
    }

    public void OnBrightnessEven(float val)
    {
        screenSettingsData.brightness = val;
        DataManager.instance.SaveSettingsData();
    }

    public void OnContrastRatioEven(float val)
    {
        screenSettingsData.contrastRatio = val;
        DataManager.instance.SaveSettingsData();
    }


    public void OnResolvingPowerChange(ResolvingPower resolvingPower)
    {
        if (screenSettingsData != null) //防止初始时DataManager未初始化获取到数据
        {
            screenSettingsData.resolvingPower = resolvingPower;
            DataManager.instance.SaveSettingsData();
        }
    }
}
