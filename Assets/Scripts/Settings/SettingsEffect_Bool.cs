using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsEffect_Bool : MonoBehaviour, ISelectHandler,IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject settings;
    public Image image_back;
    public TextMeshProUGUI settingsName;
    public TextMeshProUGUI settingsValue;
    public string[] value_StrArr;

    public bool settingsVal;
    public BoolEvenSO boolEvenSO;
    //public EventTrigger settingsTrigger;

    public bool beSelected;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnEnable()
    {
        SetElementDisplay(false);
        StartCoroutine(SetRectTransform(settings, settingsValue.gameObject));
    }

    public void OnSelect(BaseEventData eventData)
    {
        SetElementDisplay(true);
        beSelected = true;
    }
    public void OnDeselect(BaseEventData eventData)
    {
        SetElementDisplay(false);
        beSelected= false;
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

    public void OnClick()
    {
        if (settingsVal) ChangeValue(false);
        else ChangeValue(true);
        StartCoroutine(SetRectTransform(settings, settingsValue.gameObject));
    }

    public void ChangeValue(bool val)
    {
        if (val) { settingsValue.text = value_StrArr[1]; settingsVal = true; }
        else { settingsValue.text = value_StrArr[0]; settingsVal = false; }
        //����ֵ���޸ı���(ͨ���㲥��ȥ������������ʵ�־��幦��)
        //ʵ�ָ���ľ��幦��(ͨ���㲥��ȥ������������ʵ�־��幦��)
        boolEvenSO.RaiseEvent(val);
    }

    public void SetElementDisplay(bool isShow)
    {
        if (isShow)
        {
            image_back.color = new Color(image_back.color.r, image_back.color.g, image_back.color.b, 0.2f);
            Color color;
            ColorUtility.TryParseHtmlString("#0084FF", out color);
            settingsName.color = color;
            settingsValue.color = color;
        }
        else
        {
            image_back.color = new Color(image_back.color.r, image_back.color.g, image_back.color.b, 0f);
            settingsName.color = Color.white;
            settingsValue.color = Color.white;
        }

        StartCoroutine(SetRectTransform(settings, settingsValue.gameObject));
    }

    //����Ӧ����Ԫ�ش�С
    IEnumerator SetRectTransform(GameObject tragGameObject, GameObject sizeGameObject)
    {
        yield return null;//�ӳ�һ֡��ִ���޸�Ŀ������ĳߴ磬�����޸�UIԪ�غ�ߴ�δ���µ��µĳߴ�����쳣
        float totalWidth = 0f;
        totalWidth = sizeGameObject.GetComponent<RectTransform>().anchoredPosition.x + sizeGameObject.GetComponent<RectTransform>().rect.size.x;
        tragGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(totalWidth, tragGameObject.GetComponent<RectTransform>().rect.size.y);
    }



}
