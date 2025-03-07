using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//�˽ű��������û�������ȺͶԱȶȲ������Ҵ˽ű��Ľڵ㱻��Ϊ�л���Menu����ʱ��ê�㣬�˽ű��Ľڵ㲻Ӧ�޸�postion
public class ScreenManager : MonoBehaviour
{
    public static ScreenManager instance;
    public SettingsEffect_Enum resolvingPowerSettings;//�ֱ������ýű�(��������)
    //���ȡ��Աȶȵ���
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
        colorAdjustments.postExposure.value = val*7-4;//��Χ -4~3  Ĭ���趨Ϊ0
    }

    public void OnContrastRatioEven(float val)
    {
        if (colorAdjustments == null) { Debug.Log("OnContrastRatioEven"); return; }
        colorAdjustments.contrast.value = val*140-70;//��Χ -70~70  Ĭ���趨Ϊ0
    }

    IEnumerator SetScreenValLater()
    {
        yield return new WaitForSeconds(0.1f);
        colorAdjustments.postExposure.value = DataManager.instance.settingsData.screensettingsData.brightness * 7 - 4;
        colorAdjustments.contrast.value = DataManager.instance.settingsData.screensettingsData.contrastRatio * 140 - 70;
    }


}
