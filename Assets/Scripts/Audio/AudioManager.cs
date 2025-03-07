using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public PlayAudioEventSO FXEvent;
    public PlayAudioEventSO BGMEvent;
    public AudioSource BGMSource;
    public AudioSource FXSource;

    //音量设置用
    public AudioMixer mixer;
    public FloatEvenSO masterVolumeEvenSO;
    public FloatEvenSO BGMVolumeEvenSO;
    public FloatEvenSO FXVolumeEvenSO;

    private void OnEnable()
    {
        StartCoroutine(SetVolumeValLater());
        FXEvent.OnEventRised += OnFXEvent;
        BGMEvent.OnEventRised += OnBGMEvent;
        masterVolumeEvenSO.OnEventRised += OnMasterVolumeEven;
        BGMVolumeEvenSO.OnEventRised += OnBGMVolumeEven;
        FXVolumeEvenSO.OnEventRised += OnFXVolumeEven;
    }

    private void OnDisable()
    {
        FXEvent.OnEventRised -= OnFXEvent;
        BGMEvent.OnEventRised -= OnBGMEvent;
        masterVolumeEvenSO.OnEventRised -= OnMasterVolumeEven;
        BGMVolumeEvenSO.OnEventRised -= OnBGMVolumeEven;
        FXVolumeEvenSO.OnEventRised -= OnFXVolumeEven;
    }

    private void OnFXEvent(AudioClip audioClip)
    {
        FXSource.clip = audioClip;
        FXSource.Play();
    }
    private void OnBGMEvent(AudioClip audioClip)
    {
        BGMSource.clip = audioClip;
        BGMSource.Play();
    }


    public void OnMasterVolumeEven(float val)
    {
        mixer.SetFloat("MasterVolume", val * 90 - 80);
    }

    public void OnBGMVolumeEven(float val)
    {
        mixer.SetFloat("BGMVolume", val * 90 - 80);
    }

    public void OnFXVolumeEven(float val)
    {
        mixer.SetFloat("FXVolume", val * 90 - 80);
    }

    //延迟获取当前设置值，避免初始化时空引用
    IEnumerator SetVolumeValLater()
    {
        yield return new WaitForSeconds(0.1f);
        mixer.SetFloat("MasterVolume", DataManager.instance.settingsData.volumeSettingsData.masterVolume * 90 - 80);
        mixer.SetFloat("BGMVolume", DataManager.instance.settingsData.volumeSettingsData.BGMVolume * 90 - 80);
        mixer.SetFloat("FXVolume", DataManager.instance.settingsData.volumeSettingsData.FXVolume * 90 - 80);
    }
}
