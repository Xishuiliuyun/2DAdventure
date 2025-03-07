using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateBar : MonoBehaviour
{
    public Image healthImage;
    public Image healthDelayImage;
    public Image healthFrameImage;
    public RectTransform rtHealthImage;
    public RectTransform rtHealthDelayImage;
    public RectTransform rtHealthFrameImage;
    public Image energyImage;
    public Image energyDelayImage;
    public Image energyFrameImage;
    public RectTransform rtEnergyImage;
    public RectTransform rtEnergyDelayImage;
    public RectTransform rtEnergyFrameImage;
    public TextMeshProUGUI keyNumber;


    public bool isHPGrow;
    public bool isEnergyGrow;

    //修改血量和能量条的初始值获取
    //初始宽度数据
    private float initRtHealthImage_wx;
    private float initRtHealthDelayImage_wx;
    private float initRtHealthFrameImage_wx;
    //初始pos数据
    private float initRtHealthImage_px;
    private float initRtHealthDelayImage_px;
    private float initRtHealthFrameImage_px;
    private float initRtEnergyImage_wx;
    private float initRtEnergyDelayImage_wx;
    private float initRtEnergyFrameImage_wx;
    private float initRtEnergyImage_px;
    private float initRtEnergyDelayImage_px;
    private float initRtEnergyFrameImage_px;

    private void Awake()
    {
        initRtHealthImage_wx = rtHealthImage.sizeDelta.x;
        initRtHealthDelayImage_wx = rtHealthDelayImage.sizeDelta.x;
        initRtHealthFrameImage_wx = rtHealthFrameImage.sizeDelta.x;
        initRtEnergyImage_wx = rtEnergyImage.sizeDelta.x;
        initRtEnergyDelayImage_wx = rtEnergyDelayImage.sizeDelta.x;
        initRtEnergyFrameImage_wx = rtEnergyFrameImage.sizeDelta.x;

        initRtHealthImage_px = rtHealthImage.anchoredPosition.x;
        initRtHealthDelayImage_px = rtHealthDelayImage.anchoredPosition.x;
        initRtHealthFrameImage_px = rtHealthFrameImage.anchoredPosition.x;
        initRtEnergyImage_px = rtEnergyImage.anchoredPosition.x;
        initRtEnergyDelayImage_px = rtEnergyDelayImage.anchoredPosition.x;
        initRtEnergyFrameImage_px = rtEnergyFrameImage.anchoredPosition.x;
    }


    private void FixedUpdate()
    {
        if(healthImage.fillAmount<healthDelayImage.fillAmount)
        {
            healthDelayImage.fillAmount -= Time.deltaTime;
        }
        if (healthImage.fillAmount > healthDelayImage.fillAmount) healthDelayImage.fillAmount = healthImage.fillAmount;//如果有自动缓慢恢复血量(非也，只要会回复血量就需要打开该判断)，可能需要打开该判断
        if (energyImage.fillAmount < energyDelayImage.fillAmount) 
        {
            energyDelayImage.fillAmount -= Time.deltaTime;
        }
        if (energyImage.fillAmount > energyDelayImage.fillAmount) energyDelayImage.fillAmount = energyImage.fillAmount;
    }


    public void OnHealthChange(float persentage)
    {
        if (isHPGrow) healthDelayImage.fillAmount = persentage;
        healthImage.fillAmount = persentage;
        isHPGrow = false;
    }

    public void EnergyChange(float persentage)
    {
        if (isEnergyGrow) energyDelayImage.fillAmount = persentage;
        energyImage.fillAmount = persentage;
        isEnergyGrow = false;
    }

    //已弃用
    /*public void ChangeHPFrame(float beforeHP, float afterHP)
    {
        //原宽度和posX
        *//*float width = rtHealthFrameImage.sizeDelta.x;
        float height = rtHealthFrameImage.sizeDelta.y;
        float posX = rtHealthFrameImage.anchoredPosition.x;
        float posY = rtHealthFrameImage.anchoredPosition.y;
        float changeWidth = width * (1 + ((afterHP - beforeHP) / beforeHP)) - width;*//*
        float changePersentage = (afterHP - beforeHP) / beforeHP;
        float changeWidth_1 = rtHealthImage.sizeDelta.x * changePersentage;
        float changeWidth_2 = rtHealthDelayImage.sizeDelta.x * changePersentage;
        float changeWidth_3 = rtHealthFrameImage.sizeDelta.x * changePersentage;

        //设置宽度
        rtHealthImage.sizeDelta = new Vector2(rtHealthImage.sizeDelta.x + changeWidth_1, rtHealthImage.sizeDelta.y);
        rtHealthDelayImage.sizeDelta = new Vector2(rtHealthDelayImage.sizeDelta.x + changeWidth_2, rtHealthDelayImage.sizeDelta.y);
        rtHealthFrameImage.sizeDelta = new Vector2(rtHealthFrameImage.sizeDelta.x + changeWidth_3, rtHealthFrameImage.sizeDelta.y);
        //设置位置
        rtHealthImage.anchoredPosition = new Vector2(rtHealthImage.anchoredPosition.x + changeWidth_1 / 2, rtHealthImage.anchoredPosition.y);
        rtHealthDelayImage.anchoredPosition = new Vector2(rtHealthDelayImage.anchoredPosition.x + changeWidth_2 / 2, rtHealthDelayImage.anchoredPosition.y);
        rtHealthFrameImage.anchoredPosition = new Vector2(rtHealthFrameImage.anchoredPosition.x + changeWidth_3 / 2, rtHealthFrameImage.anchoredPosition.y);
    }*/
    //已弃用
    /*public void ChangEnergyFrame(float beforeEnergy, float afterEnergy)
    {
        //原宽度和posX
        *//*float width = rtHealthFrameImage.sizeDelta.x;
        float height = rtHealthFrameImage.sizeDelta.y;
        float posX = rtHealthFrameImage.anchoredPosition.x;
        float posY = rtHealthFrameImage.anchoredPosition.y;
        float changeWidth = width * (1 + ((afterHP - beforeHP) / beforeHP)) - width;*//*
        float changePersentage = (afterEnergy - beforeEnergy) / beforeEnergy;
        float changeWidth_1 = rtEnergyImage.sizeDelta.x * changePersentage;
        float changeWidth_2 = rtEnergyDelayImage.sizeDelta.x * changePersentage;
        float changeWidth_3 = rtEnergyFrameImage.sizeDelta.x * changePersentage;

        //设置宽度
        rtEnergyImage.sizeDelta = new Vector2(rtEnergyImage.sizeDelta.x + changeWidth_1, rtEnergyImage.sizeDelta.y);
        rtEnergyDelayImage.sizeDelta = new Vector2(rtEnergyDelayImage.sizeDelta.x + changeWidth_2, rtEnergyDelayImage.sizeDelta.y);
        rtEnergyFrameImage.sizeDelta = new Vector2(rtEnergyFrameImage.sizeDelta.x + changeWidth_3, rtEnergyFrameImage.sizeDelta.y);
        //设置位置
        rtEnergyImage.anchoredPosition = new Vector2(rtEnergyImage.anchoredPosition.x + changeWidth_1 / 2, rtEnergyImage.anchoredPosition.y);
        rtEnergyDelayImage.anchoredPosition = new Vector2(rtEnergyDelayImage.anchoredPosition.x + changeWidth_2 / 2, rtEnergyDelayImage.anchoredPosition.y);
        rtEnergyFrameImage.anchoredPosition = new Vector2(rtEnergyFrameImage.anchoredPosition.x + changeWidth_3 / 2, rtEnergyFrameImage.anchoredPosition.y);
    }*/

    public void ChangeHPFrame(float initHP, float afterHP)
    {
        //原宽度和posX
        /*float width = rtHealthFrameImage.sizeDelta.x;
        float height = rtHealthFrameImage.sizeDelta.y;
        float posX = rtHealthFrameImage.anchoredPosition.x;
        float posY = rtHealthFrameImage.anchoredPosition.y;
        float changeWidth = width * (1 + ((afterHP - beforeHP) / beforeHP)) - width;*/
        float changePersentage = (afterHP - initHP) / initHP;
        float changeWidth_1 = initRtHealthImage_wx * changePersentage;
        float changeWidth_2 = initRtHealthDelayImage_wx * changePersentage;
        float changeWidth_3 = initRtHealthFrameImage_wx * changePersentage;

        //设置宽度
        rtHealthImage.sizeDelta = new Vector2(initRtHealthImage_wx + changeWidth_1, rtHealthImage.sizeDelta.y);
        rtHealthDelayImage.sizeDelta = new Vector2(initRtHealthDelayImage_wx + changeWidth_2, rtHealthDelayImage.sizeDelta.y);
        rtHealthFrameImage.sizeDelta = new Vector2(initRtHealthFrameImage_wx + changeWidth_3, rtHealthFrameImage.sizeDelta.y);
        //设置位置
        rtHealthImage.anchoredPosition = new Vector2(initRtHealthImage_px + changeWidth_1 / 2, rtHealthImage.anchoredPosition.y);
        rtHealthDelayImage.anchoredPosition = new Vector2(initRtHealthDelayImage_px + changeWidth_2 / 2, rtHealthDelayImage.anchoredPosition.y);
        rtHealthFrameImage.anchoredPosition = new Vector2(initRtHealthFrameImage_px + changeWidth_3 / 2, rtHealthFrameImage.anchoredPosition.y);
    }

    public void ChangEnergyFrame(float initEnergy, float afterEnergy)
    {
        //原宽度和posX
        /*float width = rtHealthFrameImage.sizeDelta.x;
        float height = rtHealthFrameImage.sizeDelta.y;
        float posX = rtHealthFrameImage.anchoredPosition.x;
        float posY = rtHealthFrameImage.anchoredPosition.y;
        float changeWidth = width * (1 + ((afterHP - beforeHP) / beforeHP)) - width;*/
        float changePersentage = (afterEnergy - initEnergy) / initEnergy;
        float changeWidth_1 = initRtEnergyImage_wx * changePersentage;
        float changeWidth_2 = initRtEnergyDelayImage_wx * changePersentage;
        float changeWidth_3 = initRtEnergyFrameImage_wx * changePersentage;

        //设置宽度
        rtEnergyImage.sizeDelta = new Vector2(initRtEnergyImage_wx + changeWidth_1, rtEnergyImage.sizeDelta.y);
        rtEnergyDelayImage.sizeDelta = new Vector2(initRtEnergyDelayImage_wx + changeWidth_2, rtEnergyDelayImage.sizeDelta.y);
        rtEnergyFrameImage.sizeDelta = new Vector2(initRtEnergyFrameImage_wx + changeWidth_3, rtEnergyFrameImage.sizeDelta.y);
        //设置位置
        rtEnergyImage.anchoredPosition = new Vector2(initRtEnergyImage_px + changeWidth_1 / 2, rtEnergyImage.anchoredPosition.y);
        rtEnergyDelayImage.anchoredPosition = new Vector2(initRtEnergyDelayImage_px + changeWidth_2 / 2, rtEnergyDelayImage.anchoredPosition.y);
        rtEnergyFrameImage.anchoredPosition = new Vector2(initRtEnergyFrameImage_px + changeWidth_3 / 2, rtEnergyFrameImage.anchoredPosition.y);
    }

    public bool ChangeKeyNumber(int number)
    {
        if(number>=0)
        {
            keyNumber.text = "X"+ number.ToString();
            return true;
        }
        else return false;
    }

}
