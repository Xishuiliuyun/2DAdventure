using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class Tips : MonoBehaviour
{
    private TextMeshPro text;
    private Sign sign;
    public TextType textType;//0�̶��ı����� 1���������豸��仯���ı�����
    public TipsType tipsType;
    public float disappearTime;
    public bool isDisappearing;//����ִ��������ʧDOTween����
    private Tweener tween;

    private void Start()
    {
        if (textType == TextType.Dynamic)
        {
            if (Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_ANDROID || SceneLoader.instance.RUNTIME_ENVIRONMENT_WEBGL == Constants.RUNTIME_ENVIRONMENT_WEBGL_MOBILE)
            { gameObject.SetActive(false); }
        }
    }

    private void OnEnable()
    {
        if(sign == null)
        {
            var obj = GameObject.FindGameObjectWithTag("Player");
            sign = obj.GetComponentInChildren<Sign>();
        }
        text = GetComponent<TextMeshPro>();
        if (textType == TextType.Dynamic) ChangeShowText(tipsType);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (tween != null) tween.Kill();
            text.alpha = 1;
            text.enabled = true;
            isDisappearing = false;
            //sign = collision.gameObject.transform.Find("Sign").GetComponent<Sign>();
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            text.enabled = true;
            if(textType == TextType.Dynamic) ChangeShowText(tipsType);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //����ʧЧ��
            Disappear();
            //text.enabled = false;
        }
    }


    public void ChangeShowText(TipsType tipsType)
    {
        if (sign == null) { Debug.Log("ChangeShowTextERR");return; }
        switch (sign.inputControlType)
        {
            case 0:
            case 1://Ĭ�ϡ�����
                text.text = GetTipsText_KeyboardAndMouse(tipsType);
                break;
            case 2://XBOX
                text.text = GetTipsText_XBOX(tipsType);
                break;
            case 3://PS
                text.text = "��ҡ���ƶ�  X����Ծ  ��������";
                break;
            case 4://Switch
                text.text = "��ҡ���ƶ�  B����Ծ  Y������";
                break;
        }
    }

    //�����ã���¼��
    /*switch (sign.inputControlType)
        {
            case 0:
            case 1://Ĭ�ϡ�����
                text.text = "AD��/���Ҽ��ƶ�  �ո����Ծ  ������/J������";
                break;
            case 2://XBOX
                text.text = "��ҡ���ƶ�  A����Ծ  X������";
                break;
            case 3://PS
                text.text = "��ҡ���ƶ�  X����Ծ  ��������";
                break;
            case 4://Switch
                text.text = "��ҡ���ƶ�  B����Ծ  Y������";
                break;
        }*/

private string GetTipsText_KeyboardAndMouse(TipsType tipsType)
    {
        string iRet = "null";
        switch (tipsType)
        {
            case TipsType.MoveAndAtt:
                iRet = "AD��/���Ҽ��ƶ�  �ո����Ծ  ������/J������";
                break;
            case TipsType.SlipWall:
                iRet = "����";
                break;
            case TipsType.ClimbWall:
                iRet = "����";
                break;
            case TipsType.Slid:
                iRet = "Shift�����л���";
                break;
            default:
                iRet = "null";
                break;
        }
        return iRet;
    }

    private string GetTipsText_XBOX(TipsType tipsType)
    {
        string iRet = "null";
        switch (tipsType)
        {
            case TipsType.MoveAndAtt:
                iRet = "��ҡ���ƶ�  A����Ծ  X������";
                break;
            case TipsType.SlipWall:
                iRet = "����";
                break;
            case TipsType.ClimbWall:
                iRet = "����";
                break;
            case TipsType.Slid:
                iRet = "RT�����л���";
                break;
            default:
                iRet = "null";
                break;
        }
        return iRet;
    }


    public void Disappear()
    {
        isDisappearing = true;
        tween = text.DOFade(0f, disappearTime);
        tween.OnComplete(SetState);
    }

    public void SetState()
    {
        isDisappearing = false;
        text.enabled = false;
    }

    /*public IEnumerator Disappear()
    {
        yield return new WaitForSeconds(1);
    }*/

}
