using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeCanvas : MonoBehaviour
{
    public Image fadeImage;
    public FadeEventSO fadeEvent;

    private void OnEnable()
    {
        fadeEvent.OnEventRaised += OnFadeEvent;
    }

    private void OnDisable()
    {
        fadeEvent.OnEventRaised -= OnFadeEvent;
    }


    public void OnFadeEvent(Color target,float duration,bool fadeIn)
    {
        if(!fadeIn)
        {//渐显时，先持续黑屏一部分时间再显示
            StartCoroutine(ShowLater(target, duration));
        }
        else
        {
            fadeImage.DOBlendableColor(target, duration);
        }
    }

    IEnumerator ShowLater(Color target, float duration)
    {
        yield return new WaitForSeconds(duration);
        fadeImage.DOBlendableColor(target, duration);
    }
}
