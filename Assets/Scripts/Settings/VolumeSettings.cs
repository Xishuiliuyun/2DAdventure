using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSettings : MonoBehaviour
{
    private VolumeSettingsData volumeSettingsData;

    public SettingsEffect_Float masterVolume;
    public FloatEvenSO masterVolumeEvenSO;
    public SettingsEffect_Float BGMVolume;
    public FloatEvenSO BGMVolumeEvenSO;
    public SettingsEffect_Float FXVolume;
    public FloatEvenSO FXVolumeEvenSO;


    private void Start()
    {
        volumeSettingsData = DataManager.instance.settingsData.volumeSettingsData;
    }

    private void OnEnable()
    {
        masterVolumeEvenSO.OnEventRised += OnMasterVolumeEven;
        BGMVolumeEvenSO.OnEventRised += OnBGMVolumeEven;
        FXVolumeEvenSO.OnEventRised += OnFXVolumeEven;
    }

    private void OnDisable()
    {
        masterVolumeEvenSO.OnEventRised -= OnMasterVolumeEven;
        BGMVolumeEvenSO.OnEventRised -= OnBGMVolumeEven;
        FXVolumeEvenSO.OnEventRised -= OnFXVolumeEven;
    }

    public void SetSettingsValue()
    {
        volumeSettingsData = DataManager.instance.settingsData.volumeSettingsData;
        masterVolume.settingsValue.value = volumeSettingsData.masterVolume;
        BGMVolume.settingsValue.value = volumeSettingsData.BGMVolume;
        FXVolume.settingsValue.value = volumeSettingsData.FXVolume;
    }

    public void OnMasterVolumeEven(float val)
    {
        volumeSettingsData.masterVolume = val;
        DataManager.instance.SaveSettingsData();
    }

    public void OnBGMVolumeEven(float val)
    {
        volumeSettingsData.BGMVolume = val;
        DataManager.instance.SaveSettingsData();
    }

    public void OnFXVolumeEven(float val)
    {
        volumeSettingsData.FXVolume = val;
        DataManager.instance.SaveSettingsData();
    }

}
