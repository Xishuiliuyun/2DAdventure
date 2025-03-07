using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HPControl : MonoBehaviour
{
    //检测范围相关
    public Transform enemy;
    public Vector2 centerOffset;//检测中心点
    public Vector2 checkSize;//检测范围
    public LayerMask PlayerLayer;//检测物理层
    public bool isNearPlayer;


    //HP显示相关
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
    //分别记录动画是否播放完毕
    private bool flag1 = false;
    private bool flag2 = false;
    private bool flag3 = false;
    private bool canPlayFadeTween;//状态控制用，是否可以播放血条渐隐动画

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
        if (healthImage.fillAmount > healthDelayImage.fillAmount) healthDelayImage.fillAmount = healthImage.fillAmount;//如果有自动缓慢恢复血量(非也，只要会回复血量就需要打开该判断)，可能需要打开该判断
        
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

    //检测附近是否有玩家
    public void Check()
    {
        isNearPlayer = Physics2D.OverlapBox((Vector2)enemy.position + centerOffset * enemy.localScale, checkSize, 0f, PlayerLayer);
        if (isNearPlayer) 
        {//显示血条
            ShowHPEffect(true);
            //Debug.Log("NearPlayer"); 
        }
        else
        {//隐藏血条
            ShowHPEffect(false);
        }
    }

    //血条显示效果处理 isShow显示或隐藏血条 true显示 false隐藏
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
        {//开启血量显示
            showHP = true;
            DOTween.Kill(fadeTweenId);
            Check();
        }
        else
        {//关闭血量显示
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

    //绘制检测区域
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube((Vector2)enemy.position + centerOffset * enemy.localScale, checkSize);
    }


}
