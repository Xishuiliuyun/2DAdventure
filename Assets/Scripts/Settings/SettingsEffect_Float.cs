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
    public float[] value_StrArr = { 0f, 0.25f, 0.5f, 0.75f, 1f };//已弃用，本来想要不要鼠标点击时为各点位设置值的，当前鼠标点击后值设为0
    public float settingsVal;

    public FloatEvenSO floatEvenSO;
    public bool pointerClickDontSetValToZero;//鼠标点击image_back是否将滑动条的值设为0 默认false点击设为0，true点击不进行操作
    //public Sign sign;//已弃用，本想用来进行输入设备的类型判定，但无法实现
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
        if(settingsEffect.beSelected) return;//image_back和Slider组件下均引用了该脚本，此处解决重复执行会导致的UI显示异常问题
        SetElementDisplay(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (beSelected) return;
        if (settingsEffect.beSelected) return;
        SetElementDisplay(false);
    }

    //此方式无法区分键盘和鼠标点击确认事件，已弃用
    //只有image_back组件的button会激活此函数
    /*public void OnClick()
    {
        Debug.Log("OnClick_SettingsEffect_Float");
        if (sign.inputControlType == 0)
        { //鼠标操作
            //滑动条归零，设置被选择项为滑动条
        }
        else
        {//键盘或手柄
            //设置被选择项为滑动条
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

    //自适应调整元素大小
    IEnumerator SetRectTransform(GameObject tragGameObject, GameObject sizeGameObject)
    {
        yield return null;
        float totalWidth = 0f;
        totalWidth = sizeGameObject.GetComponent<RectTransform>().anchoredPosition.x + sizeGameObject.GetComponent<RectTransform>().rect.size.x;
        tragGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(totalWidth, tragGameObject.GetComponent<RectTransform>().rect.size.y);
    }

    //应该只有image_back会激活此函数
    //鼠标点击按钮确认
    public void OnPointerClick(PointerEventData eventData)
    {
        if (pointerClickDontSetValToZero) return;
        if(GetComponent<Image>()!=null &&GetComponent<Slider>() == null)
        {
            //滑动条归零，设置被选择项为滑动条
            settingsVal = 0;
            settingsValue.value = 0;
            EventSystem.current.SetSelectedGameObject(settingsValue.gameObject);
        }
        
    }

    //键盘或手柄确认
    public void OnSubmit(BaseEventData eventData)
    {
        if (GetComponent<Image>() != null && GetComponent<Slider>() == null)
        {
            //设置被选择项为滑动条
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
