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
    private bool result;//������ӽ��
    private bool inProgress;//������ù�����
    public AbilityType abilityType;
    public int jumpTimes;//����Ծ����
    public float timeScale;//ʱ��߶ȣ������趨ʱ���������
    public bool isDone;
    //ͼƬ�͵ƹ����
    public float costTime;//��ɢ�ƹ��ʱ
    public float intensity;//��ɢʱ�ƹ�����
    public float lightRange;//�ƹ���ɢ��Χ
    public float orginIntensity;//�ƹ��ʼ����
    public float outerRadius;//�ƹ��ʼ��Χ
    public float innerRadius;//�ƹ��ʼ��Χ
    public SpriteRenderer abilityStoneSprite;
    public Sprite dark;
    public Sprite bright;
    public GameObject lightObj;
    public Light2D lightComponent;

    //��ӿ�����ʾ��Ϣ�Ƿ���ʾ�¼�
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
        //����޸�Ability����
        if (abilityType == AbilityType.jumpTimes) result = playerAbility.LearnAbility(abilityType, jumpTimes);
        else result = playerAbility.LearnAbility(abilityType);
        if (!result) { Debug.Log("�������ʧ��"); inProgress = false ; return; }
        OnShowTips?.Invoke(true);
        //���ò����ظ�����
        this.gameObject.tag = "UNInteractable";
        GetComponent<BoxCollider2D>().enabled = false;
        //������Ҳ��ɲ���
        //����ʱ������
        //���õƹ��ͼƬ
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
        //������Ҳ��ɲ���
        inputControl.GamePlay.Disable();
        //����ʱ������
        Time.timeScale = timeScale;
        //���õƹ�
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
        //����ͼƬ
        abilityStoneSprite.sprite = dark;
        //�رյƹ�
        lightComponent.enabled = false;
        //�ָ���Ҳ���
        inputControl.GamePlay.Enable();

        isDone = true;
        inProgress = false;

    }
    //��û���õƹⷶΧ�Ȳ���
    public void StoneUNInteractable()
    {
        this.gameObject.tag = "UNInteractable";
        GetComponent<BoxCollider2D>().enabled = false;
        //����ͼƬ�͵ƹ�
        abilityStoneSprite.sprite = dark;
        lightComponent.enabled = false;
        //�˴������˲��ɽ�������ʾ�ƹ�������ɲ����õƹ����
        //lightComponent.intensity = orginIntensity;
        //lightComponent.pointLightInnerRadius = innerRadius;
        //lightComponent.pointLightOuterRadius = outerRadius;
        isDone = true;
        inProgress = false;
    }
    //��û���õƹⷶΧ�Ȳ���
    public void StoneInteractable()
    {
        
        this.gameObject.tag = "Interactable";
        GetComponent<BoxCollider2D>().enabled = true;
        //����ͼƬ�͵ƹ�
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
