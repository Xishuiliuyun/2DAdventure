using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//此脚本用于设置画面的亮度和对比度参数，且此脚本的节点被作为切换至Menu场景时的锚点，此脚本的节点不应修改postion
public class ScreenManager : MonoBehaviour
{
    public static ScreenManager instance;
    public SettingsEffect_Enum resolvingPowerSettings;//分辨率设置脚本(调函数用)
    //亮度、对比度调节
    public FloatEvenSO brightnessEvenSO;
    public FloatEvenSO contrastRatioEvenSO;
    public Volume volume;
    public ColorAdjustments colorAdjustments;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);


        volume.profile.TryGet(out colorAdjustments);
        StartCoroutine(SetScreenValLater());

    }

    private void OnEnable()
    {
        brightnessEvenSO.OnEventRised += OnBrightnessEven;
        contrastRatioEvenSO.OnEventRised += OnContrastRatioEven;
    }

    private void OnDisable()
    {
        brightnessEvenSO.OnEventRised -= OnBrightnessEven;
        contrastRatioEvenSO.OnEventRised -= OnContrastRatioEven;
    }

    public void OnBrightnessEven(float val)
    {
        if (colorAdjustments == null) { Debug.Log("OnBrightnessEvenERR");return; }
        colorAdjustments.postExposure.value = val*7-4;//范围 -4~3  默认设定为0
    }

    public void OnContrastRatioEven(float val)
    {
        if (colorAdjustments == null) { Debug.Log("OnContrastRatioEven"); return; }
        colorAdjustments.contrast.value = val*140-70;//范围 -70~70  默认设定为0
    }

    IEnumerator SetScreenValLater()
    {
        yield return new WaitForSeconds(0.1f);
        colorAdjustments.postExposure.value = DataManager.instance.settingsData.screensettingsData.brightness * 7 - 4;
        colorAdjustments.contrast.value = DataManager.instance.settingsData.screensettingsData.contrastRatio * 140 - 70;
    }


}
