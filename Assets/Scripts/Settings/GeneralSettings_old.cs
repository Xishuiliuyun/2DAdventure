/*using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//�����ã����ܷŵ��������͵�UI��������ʵ���ˣ��˽ű�����Ŀ�������á�
public class GeneralSettings_old : MonoBehaviour
{
    public GameObject languagePanel;

    public Button showEnemyHP;
    public Button showDamage;
    public Button isScreenShock;
    public Button isHandleShock;
    public Button language;

    public TextMeshProUGUI showEnemyHP_Value;
    public TextMeshProUGUI showDamage_Value;
    public TextMeshProUGUI isScreenShock_Value;
    public Slider isHandleShock_Value;
    public TextMeshProUGUI language_Value;

    private string[] showEnemyHP_Value_StrArr = { "����ʾ", "��ʾ" };
    private string[] showDamage_Value_StrArr = { "����ʾ", "��ʾ" };
    private string[] isScreenShock_Value_StrArr = { "������", "����" };
    private float[] isHandleShock_Value_FloatArr = { 0f, 0.25f, 0.5f, 0.75f, 1f };

    public Button language_Chinese;
    public Button language_English;
    public Button language_Japanese;
    public Button language_Korean;

    public bool showEnemyHP_Val;
    public bool showDamage_Val;
    public bool isScreenShock_Val;
    public float isHandleShock_Val;
    public Language language_Val;

    public Button showEnemyHP_Btn;
    public EventTrigger showEnemyHP_Trigger;
    public EventTrigger showDamage_Trigger;

    public GameObject currentHoveredGameObject;

    private void Start()
    {
        //showEnemyHP.onClick.AddListener(() => { ChangeValue_Bool(showEnemyHP_Value.gameObject, showEnemyHP_Value_StrArr); });
        //showEnemyHP_Value.onClick.AddListener(() => { ChangeValue_Bool(showEnemyHP_Value.gameObject, showEnemyHP_Value_StrArr); });
        //showDamage.onClick.AddListener(() => { ChangeValue_Bool(showDamage_Value.gameObject, showDamage_Value_StrArr); });
        //showDamage_Value.onClick.AddListener(() => { ChangeValue_Bool(showDamage_Value.gameObject, showDamage_Value_StrArr); });
        //isScreenShock.onClick.AddListener(() => { ChangeValue_Bool(isScreenShock_Value.gameObject, isScreenShock_Value_StrArr); });
        //isScreenShock_Value.onClick.AddListener(() => { ChangeValue_Bool(isScreenShock_Value.gameObject, isScreenShock_Value_StrArr); });
        //showEnemyHP_Btn.onClick.AddListener(ShowEnemyHP_BtnOnSelect);

        //�������¼�
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback = new EventTrigger.TriggerEvent();
        entryEnter.callback.AddListener((data) => { BeSelect(data); });
        
        //����뿪�¼�
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback = new EventTrigger.TriggerEvent();
        entryExit.callback.AddListener((data) => { Deselect(data); });
        
        //��ѡ���¼�
        EventTrigger.Entry entrySelect = new EventTrigger.Entry();
        entrySelect.eventID = EventTriggerType.Select;
        entrySelect.callback = new EventTrigger.TriggerEvent();
        entrySelect.callback.AddListener((data) => { BeSelect(data); });
        
        //ȡ��ѡ���¼�
        EventTrigger.Entry entryDeselect = new EventTrigger.Entry();
        entryDeselect.eventID = EventTriggerType.Deselect;
        entryDeselect.callback = new EventTrigger.TriggerEvent();
        entryDeselect.callback.AddListener((data) => { Deselect(data); });
        
        //ȷ���¼�
        EventTrigger.Entry entrySubmit = new EventTrigger.Entry();
        entrySubmit.eventID = EventTriggerType.Submit;
        entrySubmit.callback = new EventTrigger.TriggerEvent();
        entrySubmit.callback.AddListener((data) => { OnClick(data); });

        //������¼�
        EventTrigger.Entry clickSubmit = new EventTrigger.Entry();
        clickSubmit.eventID = EventTriggerType.PointerClick;
        clickSubmit.callback = new EventTrigger.TriggerEvent();
        clickSubmit.callback.AddListener((data) => { OnClick(data); });


        showEnemyHP_Trigger.triggers.Add(entryEnter);
        showEnemyHP_Trigger.triggers.Add(entryExit);
        showEnemyHP_Trigger.triggers.Add(entrySelect);
        showEnemyHP_Trigger.triggers.Add(entryDeselect);
        //showEnemyHP_Trigger.triggers.Add(entrySubmit);
        //showEnemyHP_Trigger.triggers.Add(clickSubmit);
        showEnemyHP_Trigger.GetComponent<Button>().onClick.AddListener(() => { ElementOnClick(showEnemyHP_Trigger.gameObject); });

        showDamage_Trigger.triggers.Add(entryEnter);
        showDamage_Trigger.triggers.Add(entryExit);
        showDamage_Trigger.triggers.Add(entrySelect);
        showDamage_Trigger.triggers.Add(entryDeselect);
        //showDamage_Trigger.triggers.Add(entrySubmit);
        //showDamage_Trigger.triggers.Add(clickSubmit);
        showDamage_Trigger.GetComponent<Button>().onClick.AddListener(() => { ElementOnClick(showDamage_Trigger.gameObject); });


    }
    //δ����
    //�����ִ��(bool��)
    //����Ĭ����
    //�޸�ѡ��ֵ

    //�����ִ��(float��)
    //����Ĭ����
    //�޸�ѡ��ֵ(�ظ����ʱ��ѭ��������ֵ��ÿ��25%)

    //�����ִ��(enum��)
    //��ʾPanel
    //����Ĭ����
    //ѡ����Ŀ��
    //�ر�Panel
    //�޸�ѡ��ֵ

    //Val�౻�����ִ��(bool��)
    //�޸�ѡ��ֵ

    //Val�౻�����ִ��(float��)
    //��������(ԭ�����ʵ�ֹ���)

    //Val�౻�����ִ��(enum��)
    //ͬԭ����������



    public void ChangeFocuedGameObject(GameObject gameObject)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
    public void ChangeValue_Bool(GameObject gameObject, string[] strArr*//*,bool val*//*)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
        string text = gameObject.GetComponent<TextMeshProUGUI>().text;
        if (gameObject == showEnemyHP.gameObject || gameObject == showEnemyHP_Value.gameObject)
        {
            if (text == strArr[0]) { showEnemyHP_Value.GetComponent<TextMeshProUGUI>().text = strArr[1]; showEnemyHP_Val = true; }
            else { showEnemyHP_Value.GetComponent<TextMeshProUGUI>().text = strArr[0]; showEnemyHP_Val = false; }
        }
        else if (gameObject == showDamage.gameObject || gameObject == showDamage_Value.gameObject)
        {
            if (text == strArr[0]) { showDamage_Value.GetComponent<TextMeshProUGUI>().text = strArr[1]; showDamage_Val = true; }
            else { showDamage_Value.GetComponent<TextMeshProUGUI>().text = strArr[0]; showDamage_Val = false; }
        }
        else if (gameObject == isScreenShock.gameObject || gameObject == isScreenShock_Value.gameObject)
        {
            if (text == strArr[0]) { isScreenShock_Value.GetComponent<TextMeshProUGUI>().text = strArr[1]; isScreenShock_Val = true; }
            else { isScreenShock_Value.GetComponent<TextMeshProUGUI>().text = strArr[0]; isScreenShock_Val = false; }
        }
        else Debug.Log("ERR_ChangeValue_Bool");
    }

    public void ChangeValue_Float(GameObject gameObject, float[] floatArr)
    {

    }

    public void ChangeValue_Enum()
    {

    }

    *//*public void ShowEnemyHP_BtnOnSelect(BaseEventData eventData)
    {
        Debug.Log("ShowEnemyHP_BtnOnSelect");
        //SetRectTransform(showEnemyHP_Btn.gameObject, showEnemyHP_Value.gameObject);

        // �� BaseEventData ת��Ϊ PointerEventData
        PointerEventData pointerEventData = eventData as PointerEventData;
        if (pointerEventData != null)
        {
            // ��ȡ�������¼�������
            GameObject targetObject = pointerEventData.pointerCurrentRaycast.gameObject;
            Debug.Log("Pointer Enter on: " + targetObject.name);
        }
    }*/


    /*public void OnSelect(BaseEventData eventData)
    {
        // �� BaseEventData ת��Ϊ PointerEventData
        PointerEventData pointerEventData = eventData as PointerEventData;
        if (pointerEventData != null)
        {
            // ��ȡ�������¼�������
            GameObject targetObject = pointerEventData.pointerCurrentRaycast.gameObject;
            Debug.Log("Pointer Enter on: " + targetObject.name);
        }
    }*//*

    //Ԫ�ر�ѡ��
    public void BeSelect(BaseEventData eventData)
    {
        bool iRet = false;
        // �� BaseEventData ת��Ϊ PointerEventData
        PointerEventData pointerEventData = eventData as PointerEventData;
        if (pointerEventData != null)
        {
            // ��ȡ�������¼�������
            GameObject targetObject = pointerEventData.pointerCurrentRaycast.gameObject;
            currentHoveredGameObject = targetObject;
            iRet = SetElementDisplay(targetObject, true);
            //Image tImage = targetObject.GetComponent<Image>();
            //tImage.color = new Color(tImage.color.r, tImage.color.g, tImage.color.b, 0.3f);
            //Debug.Log("Pointer Enter on: " + targetObject.name);
        }

        AxisEventData axisEventData = eventData as AxisEventData;
        if(axisEventData!= null)
        {
            // ��ȡ�������¼�������
            GameObject targetObject = axisEventData.selectedObject;
            iRet = SetElementDisplay(targetObject, true);
            //Image tImage = targetObject.GetComponent<Image>();
            //tImage.color = new Color(tImage.color.r, tImage.color.g, tImage.color.b, 0.3f);
            //Debug.Log("Axis Enter on: " + targetObject.name);
        }
        if (iRet == false) Debug.Log("ERR_BeSelect");
    }
    //Ԫ��ȡ��ѡ��
    public void Deselect(BaseEventData eventData)
    {
        bool iRet = false;
        // �� BaseEventData ת��Ϊ PointerEventData
        PointerEventData pointerEventData = eventData as PointerEventData;
        if (pointerEventData != null)
        {
            // ��ȡ�������¼�������
            //GameObject targetObject = pointerEventData.selectedObject.gameObject;
            iRet = SetElementDisplay(currentHoveredGameObject, false);
            //Image tImage = currentHoveredGameObject.GetComponent<Image>();
            //tImage.color = new Color(tImage.color.r, tImage.color.g, tImage.color.b, 0f);
            //Debug.Log("Pointer Enter on: " + currentHoveredGameObject.name);
        }

        AxisEventData axisEventData = eventData as AxisEventData;
        if (axisEventData != null)
        {
            // ��ȡ�������¼�������
            GameObject targetObject = axisEventData.selectedObject;
            iRet = SetElementDisplay(targetObject, false);
            //Image tImage = targetObject.GetComponent<Image>();
            //tImage.color = new Color(tImage.color.r, tImage.color.g, tImage.color.b, 0f);
            //Debug.Log("Axis Enter on: " + targetObject.name);
        }
        if (iRet == false) Debug.Log("ERR_Deselect");

    }

    //Ԫ�ر������ȷ��
    public void OnClick(BaseEventData eventData)
    {
        Debug.Log("OnClick");
        bool iRet = false;
        PointerEventData pointerEventData = eventData as PointerEventData;
        AxisEventData axisEventData = eventData as AxisEventData;
        BaseEventData baseEventData = eventData as BaseEventData;
        if (pointerEventData!=null)
        {
            GameObject targetObject = pointerEventData.pointerCurrentRaycast.gameObject;
            iRet = ElementOnClick(targetObject);
        }
        else if (axisEventData!=null)
        {
            GameObject targetObject = axisEventData.selectedObject;
            iRet = ElementOnClick(targetObject);
        }
        else if (baseEventData != null)
        {
            GameObject targetObject = baseEventData.selectedObject;
            iRet = ElementOnClick(targetObject);
        }

        if (iRet == false) Debug.Log("ERR_OnClick");
    }


    //���ÿɽ���Ԫ�ر�ѡ������ʾЧ����������ʾ�����Ƿ�ɹ�,isSelected������˳���Ԫ�ص�ѡ��
    public bool SetElementDisplay(GameObject targetObject,bool isSelected)
    {
        bool iRet = true;
        if(targetObject == null) { iRet = false; return iRet; }
        Transform parentObj = targetObject.transform.parent;
        if (parentObj == null) { iRet = false; return iRet; }
        //����image��ʾЧ��
        Image tImage = targetObject.GetComponent<Image>();
        if (tImage == null) { iRet = false; return iRet; }
        if (isSelected) tImage.color = new Color(tImage.color.r, tImage.color.g, tImage.color.b, 0.3f);
        else tImage.color = new Color(tImage.color.r, tImage.color.g, tImage.color.b, 0f);
        //����������ʾЧ��(�������ø������������������textmeshpor����¼���һ����textmeshpor�����������)
        GameObject lastTMP = null;
        for(int i = 0;i< parentObj.childCount;i++)
        {
            Transform child = parentObj.GetChild(i);
            if (child.GetComponent<TextMeshProUGUI>() != null) 
            {
                lastTMP = child.gameObject;
                Color color;
                ColorUtility.TryParseHtmlString("#0084FF",out color);
                if (isSelected) child.GetComponent<TextMeshProUGUI>().color = color;
                else child.GetComponent<TextMeshProUGUI>().color = Color.white;
            }
        }
        //���ø�����ĳߴ�(image��������������ߴ����ʽ�����游����ĳߴ�)
        if (lastTMP == null) { iRet = false; return iRet; }
        StartCoroutine( SetRectTransform(parentObj.gameObject, lastTMP));
        return iRet;
    }

    public bool ElementOnClick(GameObject targetObject)
    {
        bool iRet = true;
        if(targetObject == showEnemyHP_Trigger.gameObject)
        {
            if (showEnemyHP_Value.text == showEnemyHP_Value_StrArr[0]) { showEnemyHP_Value.text = showEnemyHP_Value_StrArr[1]; showEnemyHP_Val = true; }
            else { showEnemyHP_Value.text = showEnemyHP_Value_StrArr[0]; showEnemyHP_Val = false; }
            StartCoroutine(SetRectTransform(targetObject.transform.parent.gameObject, showEnemyHP_Value.gameObject));
        }
        else if(targetObject == showDamage_Trigger.gameObject)
        {
            if (showDamage_Value.text == showDamage_Value_StrArr[0]) { showDamage_Value.text = showDamage_Value_StrArr[1]; showDamage_Val = true; }
            else { showDamage_Value.text = showDamage_Value_StrArr[0]; showDamage_Val = false; }
            StartCoroutine(SetRectTransform(targetObject.transform.parent.gameObject, showDamage_Value.gameObject));
        }
        EventSystem.current.SetSelectedGameObject(targetObject);
        return iRet;
    }

    //����Ӧ����Ԫ�ش�С
    IEnumerator SetRectTransform(GameObject tragGameObject, GameObject sizeGameObject)
    {
        yield return null;
        float totalWidth = 0f;
        totalWidth = sizeGameObject.GetComponent<RectTransform>().anchoredPosition.x + sizeGameObject.GetComponent<RectTransform>().rect.size.x;
        tragGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(totalWidth, tragGameObject.GetComponent<RectTransform>().rect.size.y);
    }

     


}
*/