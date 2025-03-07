using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class AbilityStone : MonoBehaviour, IInteractable,ISaveable
{
    private Ability playerAbility;
    private PlayerInputController inputControl;
    private bool result;//能力添加结果
    private bool inProgress;//能力获得过程中
    public AbilityType abilityType;
    public int jumpTimes;//可跳跃次数
    public float timeScale;//时间尺度，用于设定时间变慢倍率
    public bool isDone;
    //图片和灯光组件
    public float costTime;//扩散灯光耗时
    public float intensity;//扩散时灯光亮度
    public float lightRange;//灯光扩散范围
    public float orginIntensity;//灯光初始亮度
    public float outerRadius;//灯光初始范围
    public float innerRadius;//灯光初始范围
    public SpriteRenderer abilityStoneSprite;
    public Sprite dark;
    public Sprite bright;
    public GameObject lightObj;
    public Light2D lightComponent;

    //添加控制提示信息是否显示事件
    public UnityEvent<bool> OnShowTips;
    private void Awake()
    {
        lightComponent = lightObj.GetComponent<Light2D>();
    }


    private void OnEnable()
    {
        OnShowTips?.Invoke(isDone);
        if (isDone) StoneUNInteractable();
        else StoneInteractable();
        ISaveable saveable = this;
        saveable.RegisterSaveData();

    }

    private void OnDisable()
    {
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }


    public void TriggerAction()
    {
        inProgress = true;
        //添加修改Ability能力
        if (abilityType == AbilityType.jumpTimes) result = playerAbility.LearnAbility(abilityType, jumpTimes);
        else result = playerAbility.LearnAbility(abilityType);
        if (!result) { Debug.Log("能力添加失败"); inProgress = false ; return; }
        OnShowTips?.Invoke(true);
        //设置不可重复交互
        this.gameObject.tag = "UNInteractable";
        GetComponent<BoxCollider2D>().enabled = false;
        //设置玩家不可操作
        //设置时间流速
        //设置灯光和图片
        StartCoroutine(SetTimeAndLight());
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            playerAbility = collision.GetComponent<Ability>();
            inputControl = collision.GetComponent<PlayerController>().inputControl;
        }
    }

    IEnumerator SetTimeAndLight()
    {
        //设置玩家不可操作
        inputControl.GamePlay.Disable();
        //设置时间流速
        Time.timeScale = timeScale;
        //设置灯光
        lightComponent.intensity = intensity;
        Tweener tween2 = null;
        var tween1 = DOTween.To(() => lightComponent.pointLightOuterRadius, x => lightComponent.pointLightOuterRadius = x, lightRange, costTime * Time.timeScale)
            .OnComplete(() =>
            {
                lightComponent.pointLightInnerRadius = 0;
                tween2 = DOTween.To(() => lightComponent.pointLightOuterRadius, x => lightComponent.pointLightOuterRadius = x, 0, 0.15f * Time.timeScale);
            });
        yield return tween1.WaitForCompletion();
        yield return tween2.WaitForCompletion();
        //yield return new WaitForSeconds(costTime * Time.timeScale+ 0.3f * Time.timeScale);
        Time.timeScale = 1f;
        //设置图片
        abilityStoneSprite.sprite = dark;
        //关闭灯光
        lightComponent.enabled = false;
        //恢复玩家操作
        inputControl.GamePlay.Enable();

        isDone = true;
        inProgress = false;

    }
    //还没设置灯光范围等参数
    public void StoneUNInteractable()
    {
        this.gameObject.tag = "UNInteractable";
        GetComponent<BoxCollider2D>().enabled = false;
        //设置图片和灯光
        abilityStoneSprite.sprite = dark;
        lightComponent.enabled = false;
        //此处设置了不可交互不显示灯光组件，可不设置灯光参数
        //lightComponent.intensity = orginIntensity;
        //lightComponent.pointLightInnerRadius = innerRadius;
        //lightComponent.pointLightOuterRadius = outerRadius;
        isDone = true;
        inProgress = false;
    }
    //还没设置灯光范围等参数
    public void StoneInteractable()
    {
        
        this.gameObject.tag = "Interactable";
        GetComponent<BoxCollider2D>().enabled = true;
        //设置图片和灯光
        abilityStoneSprite.sprite = bright;
        lightComponent.enabled = true;
        lightComponent.intensity = orginIntensity;
        lightComponent.pointLightInnerRadius = innerRadius;
        lightComponent.pointLightOuterRadius = outerRadius;
        isDone = false;
        inProgress = false;
    }


    public DataDefinition GetDataID()
    {
        return GetComponent<DataDefinition>();
    }

    public void GetSaveData(Data data)
    {
        if (data.interactableDataDict.ContainsKey(GetDataID().ID))
        {
            data.interactableDataDict[GetDataID().ID] = isDone;
        }
        else data.interactableDataDict.Add(GetDataID().ID, isDone);
    }

    public void LoadData(Data data)
    {
        if (data.interactableDataDict.ContainsKey(GetDataID().ID))
        {
            if (data.interactableDataDict[GetDataID().ID])
            {
                StoneUNInteractable();
                OnShowTips?.Invoke(true);
            }
            else
            {
                StoneInteractable();
                OnShowTips?.Invoke(false);
            }
        }
    }
}
