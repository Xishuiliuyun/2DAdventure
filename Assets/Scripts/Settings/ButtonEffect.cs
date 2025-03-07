using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//只保留按键显示相关的内容，设置为单个按钮用
public class ButtonEffect : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{

    public GameObject settings;
    public Image image_back;
    public TextMeshProUGUI settingsName;
    public GameObject settingsValue;
    //public string[] value_StrArr;

    //public bool settingsVal;
    //public EventTrigger settingsTrigger;

    public bool beSelected;

    private void Start()
    {
        //GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnEnable()
    {
        SetElementDisplay(false);
        if (settings != null && settingsValue != null) StartCoroutine(SetRectTransform(settings, settingsValue.gameObject));
    }

    public void OnSelect(BaseEventData eventData)
    {
        SetElementDisplay(true);
        beSelected = true;
    }
    public void OnDeselect(BaseEventData eventData)
    {
        SetElementDisplay(false);
        beSelected = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (beSelected) return;
        SetElementDisplay(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (beSelected) return;
        SetElementDisplay(false);
    }

    /*public void OnClick()
    {
        if (settingsVal) { settingsValue.text = value_StrArr[0]; settingsVal = false; }
        else { settingsValue.text = value_StrArr[1]; settingsVal = true; }
        StartCoroutine(SetRectTransform(settings, settingsValue.gameObject));
    }*/

    public void SetElementDisplay(bool isShow)
    {
        if (isShow)
        {
            image_back.color = new Color(image_back.color.r, image_back.color.g, image_back.color.b, 0.2f);
            Color color;
            ColorUtility.TryParseHtmlString("#0084FF", out color);
            if(settingsName!=null) settingsName.color = color;
            //settingsValue.color = color;
        }
        else
        {
            image_back.color = new Color(image_back.color.r, image_back.color.g, image_back.color.b, 0f);
            if (settingsName != null) settingsName.color = Color.white;
            //settingsValue.color = Color.white;
        }

        if(settings!=null && settingsValue!=null) StartCoroutine(SetRectTransform(settings, settingsValue));
    }

    //自适应调整元素大小
    IEnumerator SetRectTransform(GameObject tragGameObject, GameObject sizeGameObject)
    {
        yield return null;//延迟一帧后执行修改目标物体的尺寸，避免修改UI元素后尺寸未更新导致的尺寸计算异常
        float totalWidth = 0f;
        totalWidth = sizeGameObject.GetComponent<RectTransform>().anchoredPosition.x + sizeGameObject.GetComponent<RectTransform>().rect.size.x;
        tragGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(totalWidth, tragGameObject.GetComponent<RectTransform>().rect.size.y);
    }



}
