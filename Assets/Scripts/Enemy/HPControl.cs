using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HPControl : MonoBehaviour
{
    //��ⷶΧ���
    public Transform enemy;
    public Vector2 centerOffset;//������ĵ�
    public Vector2 checkSize;//��ⷶΧ
    public LayerMask PlayerLayer;//��������
    public bool isNearPlayer;


    //HP��ʾ���
    public bool showHP;
    public float durationTime;
    public BoolEvenSO showEnemyHPEvenSO;
    public Image healthImage;
    public Image healthDelayImage;
    public Image healthFrameImage;

    private string fadeTweenId = "fadeTween_HPControl";
    private Tween healthImageFadeTween;
    private Tween healthDelayImageFadeTween;
    private Tween healthFrameImageFadeTween;
    //�ֱ��¼�����Ƿ񲥷����
    private bool flag1 = false;
    private bool flag2 = false;
    private bool flag3 = false;
    private bool canPlayFadeTween;//״̬�����ã��Ƿ���Բ���Ѫ����������

    private void Awake()
    {
        showHP = DataManager.instance.settingsData.generalSettingsData.showEnemyHP;
        if (showHP)
        {
            transform.localScale = new Vector3(-enemy.localScale.x, transform.localScale.y, transform.localScale.z);
            //transform.localScale = enemy.localScale;
            Check();
        }
    }

    private void Update()
    {
        if (showHP)
        {
            transform.localScale = new Vector3(-enemy.localScale.x, transform.localScale.y, transform.localScale.z);
            //transform.localScale = enemy.localScale;
            Check();
        }
    }

    private void FixedUpdate()
    {
        if (healthImage.fillAmount < healthDelayImage.fillAmount)
        {
            healthDelayImage.fillAmount -= Time.deltaTime*0.5f;
        }
        if (healthImage.fillAmount > healthDelayImage.fillAmount) healthDelayImage.fillAmount = healthImage.fillAmount;//������Զ������ָ�Ѫ��(��Ҳ��ֻҪ��ظ�Ѫ������Ҫ�򿪸��ж�)��������Ҫ�򿪸��ж�
        
    }

    private void OnEnable()
    {
        showHP = DataManager.instance.settingsData.generalSettingsData.showEnemyHP;
        showEnemyHPEvenSO.OnEventRised += OnShowEnemyHPEven;
    }

    private void OnDisable()
    {
        showEnemyHPEvenSO.OnEventRised -= OnShowEnemyHPEven;
    }

    //��⸽���Ƿ������
    public void Check()
    {
        isNearPlayer = Physics2D.OverlapBox((Vector2)enemy.position + centerOffset * enemy.localScale, checkSize, 0f, PlayerLayer);
        if (isNearPlayer) 
        {//��ʾѪ��
            ShowHPEffect(true);
            //Debug.Log("NearPlayer"); 
        }
        else
        {//����Ѫ��
            ShowHPEffect(false);
        }
    }

    //Ѫ����ʾЧ������ isShow��ʾ������Ѫ�� true��ʾ false����
    public void ShowHPEffect(bool isShow)
    {
        if(isShow)
        {
            flag1 = flag2 = flag3 = false;
            canPlayFadeTween = true;
            DOTween.Kill(fadeTweenId,true);
            healthImage.color = new Color(healthImage.color.r, healthImage.color.g, healthImage.color.b, 1);
            healthDelayImage.color= new Color(healthDelayImage.color.r, healthDelayImage.color.g, healthDelayImage.color.b, 1);
            healthFrameImage.color = new Color(healthFrameImage.color.r, healthFrameImage.color.g, healthFrameImage.color.b, 1);
        }
        else
        {
            if (!canPlayFadeTween) return;
            //canPlayFadeTween = false;
            healthImageFadeTween = healthImage.DOFade(0, durationTime)
                .SetId(fadeTweenId)
                .OnComplete(() => {
                    flag1 = true;
                    if(flag1&& flag2&& flag3) canPlayFadeTween = false;
                    //healthImage.color = new Color(healthImage.color.r, healthImage.color.g, healthImage.color.b, 0);
                    //healthDelayImage.color = new Color(healthDelayImage.color.r, healthDelayImage.color.g, healthDelayImage.color.b, 0);
                    //healthFrameImage.color = new Color(healthFrameImage.color.r, healthFrameImage.color.g, healthFrameImage.color.b, 0);
                    DOTween.Kill(fadeTweenId);
                });
            healthDelayImageFadeTween = healthDelayImage.DOFade(0, durationTime)
                .SetId(fadeTweenId)
                .OnComplete(() => {
                    flag2 = true;
                    if (flag1 && flag2 && flag3) canPlayFadeTween = false;
                    //healthImage.color = new Color(healthImage.color.r, healthImage.color.g, healthImage.color.b, 0);
                    //healthDelayImage.color = new Color(healthDelayImage.color.r, healthDelayImage.color.g, healthDelayImage.color.b, 0);
                    //healthFrameImage.color = new Color(healthFrameImage.color.r, healthFrameImage.color.g, healthFrameImage.color.b, 0);
                    DOTween.Kill(fadeTweenId);
                });
            healthFrameImageFadeTween = healthFrameImage.DOFade(0, durationTime)
                .SetId(fadeTweenId)
                .OnComplete(() => {
                    flag3 = true;
                    if (flag1 && flag2 && flag3) canPlayFadeTween = false;
                    //healthImage.color = new Color(healthImage.color.r, healthImage.color.g, healthImage.color.b, 0);
                    //healthDelayImage.color = new Color(healthDelayImage.color.r, healthDelayImage.color.g, healthDelayImage.color.b, 0);
                    //healthFrameImage.color = new Color(healthFrameImage.color.r, healthFrameImage.color.g, healthFrameImage.color.b, 0);
                    DOTween.Kill(fadeTweenId);
                });
        }
    }

    public void OnShowEnemyHPEven(bool flag)
    {
        if(flag)
        {//����Ѫ����ʾ
            showHP = true;
            DOTween.Kill(fadeTweenId);
            Check();
        }
        else
        {//�ر�Ѫ����ʾ
            showHP = false;
            healthImage.color = new Color(healthImage.color.r, healthImage.color.g, healthImage.color.b, 0);
            healthDelayImage.color = new Color(healthDelayImage.color.r, healthDelayImage.color.g, healthDelayImage.color.b, 0);
            healthFrameImage.color = new Color(healthFrameImage.color.r, healthFrameImage.color.g, healthFrameImage.color.b, 0);
            canPlayFadeTween = false;
        }
    }

    public void OnHealthEvent(Character character)
    {
        var persentage = character.currentHealth / character.maxHealth;
        OnHealthChange(persentage);
    }

    public void OnHealthChange(float persentage)
    {
        healthImage.fillAmount = persentage;
    }

    public void SetHealthDelayImage()
    {
        healthDelayImage.fillAmount = healthImage.fillAmount;
    }

    //���Ƽ������
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube((Vector2)enemy.position + centerOffset * enemy.localScale, checkSize);
    }


}
