using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class Tips : MonoBehaviour
{
    private TextMeshPro text;
    private Sign sign;
    public TextType textType;//0¹Ì¶¨ÎÄ±¾ÄÚÈÝ 1¸ù¾ÝÊäÈëÉè±¸Ðè±ä»¯µÄÎÄ±¾ÄÚÈÝ
    public TipsType tipsType;
    public float disappearTime;
    public bool isDisappearing;//ÕýÔÚÖ´ÐÐÎÄ×ÖÏûÊ§DOTween¶¯»­
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
            //½¥ÏûÊ§Ð§¹û
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
            case 1://Ä¬ÈÏ¡¢¼üÊó
                text.text = GetTipsText_KeyboardAndMouse(tipsType);
                break;
            case 2://XBOX
                text.text = GetTipsText_XBOX(tipsType);
                break;
            case 3://PS
                text.text = "×óÒ¡¸ËÒÆ¶¯  X¼üÌøÔ¾  ¡õ¼ü¹¥»÷";
                break;
            case 4://Switch
                text.text = "×óÒ¡¸ËÒÆ¶¯  B¼üÌøÔ¾  Y¼ü¹¥»÷";
                break;
        }
    }

    //ÒÑÆúÓÃ£¬¼ÇÂ¼ÓÃ
    /*switch (sign.inputControlType)
        {
            case 0:
            case 1://Ä¬ÈÏ¡¢¼üÊó
                text.text = "AD¼ü/×óÓÒ¼üÒÆ¶¯  ¿Õ¸ñ¼üÌøÔ¾  Êó±ê×ó¼ü/J¼ü¹¥»÷";
                break;
            case 2://XBOX
                text.text = "×óÒ¡¸ËÒÆ¶¯  A¼üÌøÔ¾  X¼ü¹¥»÷";
                break;
            case 3://PS
                text.text = "×óÒ¡¸ËÒÆ¶¯  X¼üÌøÔ¾  ¡õ¼ü¹¥»÷";
                break;
            case 4://Switch
                text.text = "×óÒ¡¸ËÒÆ¶¯  B¼üÌøÔ¾  Y¼ü¹¥»÷";
                break;
        }*/

private string GetTipsText_KeyboardAndMouse(TipsType tipsType)
    {
        string iRet = "null";
        switch (tipsType)
        {
            case TipsType.MoveAndAtt:
                iRet = "AD¼ü/×óÓÒ¼üÒÆ¶¯  ¿Õ¸ñ¼üÌøÔ¾  Êó±ê×ó¼ü/J¼ü¹¥»÷";
                break;
            case TipsType.SlipWall:
                iRet = "ÔÝÎÞ";
                break;
            case TipsType.ClimbWall:
                iRet = "ÔÝÎÞ";
                break;
            case TipsType.Slid:
                iRet = "Shift¼ü½øÐÐ»¬²ù";
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
                iRet = "×óÒ¡¸ËÒÆ¶¯  A¼üÌøÔ¾  X¼ü¹¥»÷";
                break;
            case TipsType.SlipWall:
                iRet = "ÔÝÎÞ";
                break;
            case TipsType.ClimbWall:
                iRet = "ÔÝÎÞ";
                break;
            case TipsType.Slid:
                iRet = "RT¼ü½øÐÐ»¬²ù";
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
