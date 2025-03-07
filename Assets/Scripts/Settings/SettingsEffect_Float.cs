using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics.Eventing.Reader;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsEffect_Float : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISubmitHandler
{
    public GameObject settings;
    public Image image_back;
    public TextMeshProUGUI settingsName;
    public Slider settingsValue;
    public float[] value_StrArr = { 0f, 0.25f, 0.5f, 0.75f, 1f };//�����ã�������Ҫ��Ҫ�����ʱΪ����λ����ֵ�ģ���ǰ�������ֵ��Ϊ0
    public float settingsVal;

    public FloatEvenSO floatEvenSO;
    public bool pointerClickDontSetValToZero;//�����image_back�Ƿ񽫻�������ֵ��Ϊ0 Ĭ��false�����Ϊ0��true��������в���
    //public Sign sign;//�����ã������������������豸�������ж������޷�ʵ��
    public SettingsEffect_Float settingsEffect;

    public bool beSelected; 

    private void Start()
    {
        //if(GetComponent<Button>() !=null) GetComponent<Button>().onClick.AddListener(OnClick);
        settingsValue.onValueChanged.AddListener(OnValueChanged) ;
    }

    private void OnEnable()
    {
        SetElementDisplay(false);
        StartCoroutine(SetRectTransform(settings, settingsValue.gameObject));
        //settingsValue.navigation = new Navigation { mode = Navigation.Mode.None };
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
        if(settingsEffect.beSelected) return;//image_back��Slider����¾������˸ýű����˴�����ظ�ִ�лᵼ�µ�UI��ʾ�쳣����
        SetElementDisplay(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (beSelected) return;
        if (settingsEffect.beSelected) return;
        SetElementDisplay(false);
    }

    //�˷�ʽ�޷����ּ��̺������ȷ���¼���������
    //ֻ��image_back�����button�ἤ��˺���
    /*public void OnClick()
    {
        Debug.Log("OnClick_SettingsEffect_Float");
        if (sign.inputControlType == 0)
        { //������
            //���������㣬���ñ�ѡ����Ϊ������
        }
        else
        {//���̻��ֱ�
            //���ñ�ѡ����Ϊ������
            EventSystem.current.SetSelectedGameObject(settingsValue.gameObject);
            //settingsValue.navigation = new Navigation { mode = Navigation.Mode.Automatic };
        }
    }*/

    public void SetElementDisplay(bool isShow)
    {
        if (isShow)
        {
            image_back.color = new Color(image_back.color.r, image_back.color.g, image_back.color.b, 0.2f);
            Color color;
            ColorUtility.TryParseHtmlString("#0084FF", out color);
            settingsName.color = color;
            //settingsValue.color = color;
        }
        else
        {
            image_back.color = new Color(image_back.color.r, image_back.color.g, image_back.color.b, 0f);
            settingsName.color = Color.white;
            //settingsValue.color = Color.white;
            //settingsValue.navigation = new Navigation { mode = Navigation.Mode.None };
        }

        StartCoroutine(SetRectTransform(settings, settingsValue.gameObject));
    }

    //����Ӧ����Ԫ�ش�С
    IEnumerator SetRectTransform(GameObject tragGameObject, GameObject sizeGameObject)
    {
        yield return null;
        float totalWidth = 0f;
        totalWidth = sizeGameObject.GetComponent<RectTransform>().anchoredPosition.x + sizeGameObject.GetComponent<RectTransform>().rect.size.x;
        tragGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(totalWidth, tragGameObject.GetComponent<RectTransform>().rect.size.y);
    }

    //Ӧ��ֻ��image_back�ἤ��˺���
    //�������ťȷ��
    public void OnPointerClick(PointerEventData eventData)
    {
        if (pointerClickDontSetValToZero) return;
        if(GetComponent<Image>()!=null &&GetComponent<Slider>() == null)
        {
            //���������㣬���ñ�ѡ����Ϊ������
            settingsVal = 0;
            settingsValue.value = 0;
            EventSystem.current.SetSelectedGameObject(settingsValue.gameObject);
        }
        
    }

    //���̻��ֱ�ȷ��
    public void OnSubmit(BaseEventData eventData)
    {
        if (GetComponent<Image>() != null && GetComponent<Slider>() == null)
        {
            //���ñ�ѡ����Ϊ������
            EventSystem.current.SetSelectedGameObject(settingsValue.gameObject);
        }
        
    }

    public void OnValueChanged(float val)
    {
        settingsVal = val;
        if(floatEvenSO!=null) floatEvenSO.RaiseEvent(val);
    }

    public void RecoveryBrightnessVal()
    {
        settingsValue.value = 0.6f;
    }

    public void RecoveryContrastRatioVal()
    {
        settingsValue.value = 0.5f;
    }

}
