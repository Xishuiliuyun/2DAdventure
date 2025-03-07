using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.IO;

public class SettingsEffect_BtnSettings : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public PlayerController playerController;
    public PlayerInputController playerInputCtrl;
    public ButtonSettings buttonSettings;
    public SettingsTips settingsTips;
    public GameObject settings;
    public Image image_back;
    public Image RecoveryImage;
    public TextMeshProUGUI settingsName;
    public TextMeshProUGUI settingsValue;
    public Button recoveryBtn;
    public GameObject coverPanel;
    Sequence image_backSequence;
    public Button recoveryAllBtn;//�ָ�Ĭ������ʱʹ��

    public bool isKeyboard;
    public bool isGamepad;
    public Keys_Keyboard key_Keyboard;
    public Keys_Gamepad key_Gamepad;
    private BindInfoData bindInfoData_Keyboard;
    private BindInfoData bindInfoData_Gamepad;
    private BindInfoData currentBindInfoData;
    private InputAction action;
    private int bindingIndex;
    private InputActionRebindingExtensions.RebindingOperation rebindOperation;
    private string pathBeforeChange;
    private BindInfoData sameBtnInfo;

    //public bool settingsVal;
    //public EventTrigger settingsTrigger;

    private bool beSelected;
    //private bool isDeselectCancel;//�Ƿ������ȡ�������󶨹���


    private void Start()
    {
        playerInputCtrl = playerController.inputControl;

        //�󶨹��̵���ʾЧ�����
        image_backSequence = DOTween.Sequence();
        image_backSequence.Append(image_back.DOFade(0, 0.3f));
        image_backSequence.Append(image_back.DOFade(0.8f, 0.3f));
        image_backSequence.SetLoops(-1, LoopType.Restart);
        image_backSequence.SetUpdate(true);//������ͣ���Բ��Ŷ���
        image_backSequence.Pause();

        //��ȡ��ʼ�������������Ϣ(������)
        /*if (isKeyboard || key_Keyboard!=Keys_Keyboard.Null)
        {
            //��ȡBindInfoData
            bindInfoData_Keyboard = DataManager.instance.buttonSettingsData.Keys_KeyboardDict[key_Keyboard];
            currentBindInfoData = bindInfoData_Keyboard;
            if (bindInfoData_Keyboard.currentPath != null)
            {
                //Action��path���
                if (!CheckActionAndBind(bindInfoData_Keyboard.actionName, bindInfoData_Keyboard.defaultPath)) { Debug.Log("actionORbindIndexERR"); return; }
                //��ȡbindingIndex
                action = playerInputCtrl.FindAction(bindInfoData_Keyboard.actionName);
                bindingIndex = GetBindingIndex(bindInfoData_Keyboard.actionName, bindInfoData_Keyboard.defaultPath);
            }
            else
            {
                //Action��path���
                if (!CheckActionAndBind(bindInfoData_Keyboard.actionName, bindInfoData_Keyboard.defaultPath)) { Debug.Log("actionORbindIndexERR"); return; }
                //��ȡbindingIndex
                action = playerInputCtrl.FindAction(bindInfoData_Keyboard.actionName);
                bindingIndex = GetBindingIndex(bindInfoData_Keyboard.actionName, bindInfoData_Keyboard.defaultPath);
            }
            bindInfoData_Keyboard.bindingIndex = bindingIndex;
            //��ӻָ�Ĭ��ֵ�¼�
            GetComponent<Button>().onClick.AddListener(() => ReBindingBtn(bindInfoData_Keyboard.actionName, bindInfoData_Keyboard.defaultPath));
        }
        else if (isGamepad || key_Gamepad != Keys_Gamepad.Null)
        {
            //��ȡBindInfoData
            bindInfoData_Gamepad = DataManager.instance.buttonSettingsData.Keys_GamepadDict[key_Gamepad];
            currentBindInfoData = bindInfoData_Gamepad;
            if (bindInfoData_Gamepad.currentPath != null)
            {
                //Action��path���
                if (!CheckActionAndBind(bindInfoData_Gamepad.actionName, bindInfoData_Gamepad.defaultPath)) { Debug.Log("actionORbindIndexERR"); return; }
                //��ȡbindingIndex
                action = playerInputCtrl.FindAction(bindInfoData_Gamepad.actionName);
                bindingIndex = GetBindingIndex(bindInfoData_Gamepad.actionName, bindInfoData_Gamepad.defaultPath);
            }
            else
            {
                //Action��path���
                if (!CheckActionAndBind(bindInfoData_Gamepad.actionName, bindInfoData_Gamepad.defaultPath)) { Debug.Log("actionORbindIndexERR"); return; }
                //��ȡbindingIndex
                action = playerInputCtrl.FindAction(bindInfoData_Gamepad.actionName);
                bindingIndex = GetBindingIndex(bindInfoData_Gamepad.actionName, bindInfoData_Gamepad.defaultPath);
            }
            bindInfoData_Gamepad.bindingIndex = bindingIndex;
            //��ӻָ�Ĭ��ֵ�¼�
            GetComponent<Button>().onClick.AddListener(() => ReBindingBtn(bindInfoData_Gamepad.actionName, bindInfoData_Gamepad.defaultPath));
        }*/

        //��ȡ��ʼ�������������Ϣ
        if (isKeyboard || key_Keyboard != Keys_Keyboard.Null)
        {
            isKeyboard = true;
            //��ȡBindInfoData
            bindInfoData_Keyboard = DataManager.instance.buttonSettingsData.Keys_KeyboardDict[key_Keyboard];
            if (bindInfoData_Keyboard == null) { Debug.Log("ReadBindInfoDataERR"); return; }
            currentBindInfoData = bindInfoData_Keyboard;
            action = playerInputCtrl.FindAction(bindInfoData_Keyboard.actionName);
            if (bindInfoData_Keyboard.bindingIndex >= 0)//�ѻ�ȡ������������Ϣ
            {
                bindingIndex = bindInfoData_Keyboard.bindingIndex;
                //��д��ǰ��������
                action.ApplyBindingOverride(bindingIndex, bindInfoData_Keyboard.currentPath);
                //Debug.Log(action.bindings[bindingIndex].overridePath);
                //Debug.Log(action.bindings[bindingIndex].path);
            }
            else//δ��ȡ������������Ϣ
            {
                //Action��path���
                //if (!CheckActionAndBind(bindInfoData_Keyboard.actionName, bindInfoData_Keyboard.defaultPath)) { Debug.Log("actionORbindIndexERR"); return; }
                //��ȡbindingIndex
                bindingIndex = GetBindingIndex(bindInfoData_Keyboard.actionName, bindInfoData_Keyboard.defaultPath);
                bindInfoData_Keyboard.bindingIndex = bindingIndex;
            }
            //��ӻָ�Ĭ��ֵ�¼�
            GetComponent<Button>().onClick.AddListener(() => ReBindingBtn());
        }
        else if (isGamepad || key_Gamepad != Keys_Gamepad.Null)
        {
            isGamepad = true;
            //��ȡBindInfoData
            bindInfoData_Gamepad = DataManager.instance.buttonSettingsData.Keys_GamepadDict[key_Gamepad];
            if (bindInfoData_Gamepad == null) { Debug.Log("ReadBindInfoDataERR"); return; }
            currentBindInfoData = bindInfoData_Gamepad;
            action = playerInputCtrl.FindAction(bindInfoData_Gamepad.actionName);
            if (bindInfoData_Gamepad.bindingIndex >= 0)//�ѻ�ȡ������������Ϣ
            {
                bindingIndex = bindInfoData_Gamepad.bindingIndex;
                //��д��ǰ��������
                action.ApplyBindingOverride(bindingIndex, bindInfoData_Gamepad.currentPath);
                //Debug.Log(action.bindings[bindingIndex].overridePath);
                //Debug.Log(action.bindings[bindingIndex].path);
            }
            else//δ��ȡ������������Ϣ
            {
                bindingIndex = GetBindingIndex(bindInfoData_Gamepad.actionName, bindInfoData_Gamepad.defaultPath);
                bindInfoData_Gamepad.bindingIndex = bindingIndex;
            }
            //��ӻָ�Ĭ��ֵ�¼�
            GetComponent<Button>().onClick.AddListener(() => ReBindingBtn());
        }

        //Debug.Log(action.name + bindingIndex);
        SetSettingsValue();
        InitSettingsUI();
        recoveryBtn.onClick.AddListener(() => { Recovery(false); });
        recoveryAllBtn.onClick.AddListener(() => { Recovery(true); });
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
        //EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(GetCurrentAndSetSelectedObj());
        rebindOperation?.Cancel();
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

    public void ReBindingBtn()
    {
        //Debug.Log(actionName);
        /*for (int i = 0;i< playerInputCtrl.FindAction("Move").bindings.Count;i++)
        {
            Debug.Log(playerInputCtrl.FindAction("Move").bindings[i].effectivePath);
        }*/

        /*for (int i = 0; i < action.bindings.Count; i++)
        {
            Debug.Log(i + "    " + action.bindings[i].name + "    " + action.bindings[i].id);

        }*/

        /*foreach(var control in action.controls)
        {
            Debug.Log($"Bound Control: {control.displayName} ({control.name})");
        }*/

        //bindingIndex = GetBindingIndex(actionName, path);
        //Debug.Log(bindingIndex);

        action.Disable();//����action
        //ִ�а����ذ󶨹���
        PerformInteractiveRebind(bindingIndex);

        //ˢ����ʾ����
        StartCoroutine(SetRectTransform(settings, settingsValue.gameObject));
    }

    //�����ذ󶨹���
    public void PerformInteractiveRebind(int index)
    {
        //���������ִ�еİ����ذ󶨣�����ȡ��
        rebindOperation?.Cancel(); // Will null out m_RebindOperation.
        //��������ذ���ص���Դռ��
        void CleanUp()
        {
            rebindOperation?.Dispose();
            rebindOperation = null;
        }

        //��¼�޸�ǰ�İ���path
        if(action.bindings[bindingIndex].overridePath !=null)
        {
            pathBeforeChange = action.bindings[bindingIndex].overridePath;
        }
        else
        {
            pathBeforeChange = action.bindings[bindingIndex].path;
        }
        Debug.Log("InfoBeforeRebinding  "+ "MapName:"+ action.actionMap.name+ "   ActionName:" + action.name +"  ActionIndex:" + bindingIndex+"  OverridePath:" + action.bindings[bindingIndex].overridePath);//��������ذ󶨵İ�����Ϣ
        rebindOperation = action.PerformInteractiveRebinding(index)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .WithCancelingThrough("<Keyboard>/escape")//�趨�����豸��Ĭ�Ϸ��ذ���Ϊȡ���ذ�����
            .OnCancel(operation => {
                Debug.Log("ReBindingBtn Cancel");
                //ȡ��ButtonSettings�Ե�ǰ�ű�������
                buttonSettings.btnSettingScript = null;
                //�ر���ʾЧ��
                BindShoweEffect(false);
                //ˢ�°�ť��ʾ�������
                SetSettingsValue();
                //�����Դ
                CleanUp();
            })
            .OnComplete(operation =>
            {
                Debug.Log("ReBindingBtn Complete");
                //ȡ��ButtonSettings�Ե�ǰ�ű�������
                buttonSettings.btnSettingScript = null;
                //�ȶ��ֵ��е����ݣ�ȷ���Ƿ����ظ�����(���ظ����������˴ΰ�����)
                if(CheckBtnRepeat(action.bindings[bindingIndex].overridePath))
                {//���������ظ�
                    //�ر���ʾЧ��
                    BindShoweEffect(false);
                    //������ʾ��Ϣ
                    Debug.Log("���������ظ�");
                    string str = "����İ��������á�" + sameBtnInfo.settingName+ "���İ���һ�£����������ð���";
                    UIManager.instance.ShowPromptBox(true, str, "��ʾ", true, 1);
                    UIManager.instance.confirmBtn.onClick.AddListener(UIManager.instance.ClosePromptBox);
                    UIManager.instance.confirmBtn.onClick.AddListener(UIManager.instance.RecoverAlpha);
                    UIManager.instance.confirmBtn.onClick.AddListener(() => { EventSystem.current.SetSelectedGameObject(image_back.gameObject); });
                    UIManager.instance.cancelBtn.onClick.AddListener(UIManager.instance.ClosePromptBox);
                    UIManager.instance.cancelBtn.onClick.AddListener(UIManager.instance.RecoverAlpha);
                    UIManager.instance.cancelBtn.onClick.AddListener(() => { EventSystem.current.SetSelectedGameObject(image_back.gameObject); });
                    //�ָ����޸ĵİ���
                    if (pathBeforeChange != null)
                    {
                        Debug.Log("�ָ����޸ĵİ���");
                        action.ApplyBindingOverride(bindingIndex, pathBeforeChange);
                    }
                    else { Debug.Log("�ָ����޸ĵİ���ERR"); }
                }
                else
                {
                    Debug.Log("InfoAfterRebinding  " + "MapName:" + action.actionMap.name + "   ActionName:" + action.name + "  ActionIndex:" + bindingIndex + "  OverridePath:" + action.bindings[bindingIndex].overridePath);//����ذ󶨺�İ�����Ϣ
                    //��¼�޸�����
                    //string actionMapJson = action.SaveBindingOverridesAsJson();
                    currentBindInfoData.currentPath = action.bindings[bindingIndex].overridePath;
                    Debug.Log(currentBindInfoData.currentPath);
                    //���������
                    DataManager.instance.SaveButtonSettingsData();
                    //�ر���ʾЧ��
                    BindShoweEffect(false);
                }

                //ˢ�°�ť��ʾ�������
                SetSettingsValue();
                //�����Դ
                CleanUp();
            });

        //����ButtonSettings�Ե�ǰ�ű�������
        buttonSettings.btnSettingScript = this;
        //��ʾ��ʾ�����(�������ִ��ȡ�������ذ󶨵�����)
        Debug.Log("BindShoweEffect");
        BindShoweEffect(true);

        rebindOperation.Start();
    }

    //��ȡbindingIndex
    public int GetBindingIndex(string actionName,string path)
    {
        int iRet = -1;
        action = playerInputCtrl.FindAction(actionName);
        if (action == null) { Debug.Log("GetBindingIndexERR"); return -1; }
        for (int i = 0;i< action.bindings.Count;i++)
        {
            if (action.bindings[i].path == path)
            {
                iRet = i;
                break;
            }
        }
        if (iRet < 0) Debug.Log("GetBindingIndexERR");
        return iRet;
    }

    //�󶨹��̵���ʾЧ��
    public void BindShoweEffect(bool isShow)
    {
        //Sequence image_backSequence = DOTween.Sequence();
        if (isShow)
        {
            coverPanel.gameObject.SetActive(true);
            //settingsName.gameObject.SetActive(false);
            settingsValue.gameObject.SetActive(false);
            //image��˸
            image_backSequence.Play();
        }
        else if(!isShow)
        {
            coverPanel.gameObject.SetActive(false);
            settingsName.gameObject.SetActive(true);
            settingsValue.gameObject.SetActive(true);
            image_backSequence.Pause();
            //����ΪĬ��ѡ��
            //EventSystem.current.SetSelectedGameObject(image_back.gameObject);
            //�ָ�Ĭ��͸����
            image_back.color = new Color(image_back.color.r, image_back.color.g, image_back.color.b, 0.2f);
        }
    }

    //ʵ���Ѿ����ᱻ������������
    public void CoverPanelClick()
    {
        Debug.Log("CoverPanelClick");
        rebindOperation.Cancel();
        //BindShoweEffect(false);
    }

    //��������Action��Index�Ƿ��쳣(�о������ã�������)
    public bool CheckActionAndBind(string actionName, string path)
    {
        bool iRet = false;
        //���ж��߼������ã�ʹ��path�ķ�ʽ��ȡbindingIndex
        //int count = playerInputCtrl.FindAction(actionName).bindings.Count;
        //if (count < bindIndex + 1 || count<=0) iRet = false;
        //else iRet = true;
        int ret =  GetBindingIndex(actionName, path);
        if (ret < 0) iRet = false;
        else iRet = true;
        return iRet;
    }

    //�ָ�Ĭ�ϰ�ť  flagΪtrueʱ�����а�����ͻ�ж����ָ�Ĭ������ʱʹ��
    public void Recovery(bool flag)
    {
        //Debug.Log("Recovery");
        if(flag)
        {
            settingsTips.IsRecoveryBtn(true);
            action.RemoveBindingOverride(bindingIndex);
            currentBindInfoData.currentPath = currentBindInfoData.defaultPath;
            DataManager.instance.SaveButtonSettingsData();
            SetSettingsValue();
            StartCoroutine(SetIsRecoveryBtnVal());
            return;
        }
        //����ָ�Ĭ�ϰ�ťʱԭ������ռ�������д���
        if(CheckBtnRepeat(currentBindInfoData.defaultPath))
        {//�ָ�Ĭ�ϰ�ťʱԭ������ռ
            //������ʾ��Ϣ
            Debug.Log("�ָ�Ĭ�ϰ�ťʱԭ������ռ");
            string str = "�ָ�Ĭ�ϰ�ťʱ�����á�" + sameBtnInfo.settingName + "���İ�����ͻ�����޸ġ�" + sameBtnInfo.settingName+ "���İ���������";
            UIManager.instance.ShowPromptBox(true, str, "��ʾ", true, 1);
            UIManager.instance.confirmBtn.onClick.AddListener(UIManager.instance.ClosePromptBox);
            UIManager.instance.confirmBtn.onClick.AddListener(UIManager.instance.RecoverAlpha);
            UIManager.instance.confirmBtn.onClick.AddListener(() => { EventSystem.current.SetSelectedGameObject(recoveryBtn.gameObject); });
            UIManager.instance.cancelBtn.onClick.AddListener(UIManager.instance.ClosePromptBox);
            UIManager.instance.cancelBtn.onClick.AddListener(UIManager.instance.RecoverAlpha);
            UIManager.instance.cancelBtn.onClick.AddListener(() => { EventSystem.current.SetSelectedGameObject(image_back.gameObject); });
        }
        else
        {
            settingsTips.IsRecoveryBtn(true);
            action.RemoveBindingOverride(bindingIndex);
            currentBindInfoData.currentPath = currentBindInfoData.defaultPath;
            DataManager.instance.SaveButtonSettingsData();
        }
        SetSettingsValue();
        StartCoroutine(SetIsRecoveryBtnVal());
    }


    //��ʼ���������õ�UI
    public void InitSettingsUI()
    {
        if (isKeyboard || key_Keyboard != Keys_Keyboard.Null)
        {
            settingsName.text = bindInfoData_Keyboard.settingName;
            /*if (bindInfoData_Keyboard.keyCode != KeyCode.None) settingsValue.text = bindInfoData_Keyboard.keyCode.ToString();
            else settingsValue.text = bindInfoData_Keyboard.defaultKeyCode.ToString();*/
        }
        else if (isGamepad || key_Gamepad != Keys_Gamepad.Null)
        {
            settingsName.text = bindInfoData_Gamepad.settingName;
            /*if (bindInfoData_Gamepad.keyCode != KeyCode.None) settingsValue.text = bindInfoData_Gamepad.keyCode.ToString();
            else settingsValue.text = bindInfoData_Gamepad.defaultKeyCode.ToString();*/
        }
    }

    //ͬ���޸İ���ֵ
    public void SetSettingsValue()
    {
        settingsValue.text = InputControlPath.ToHumanReadableString(action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        StartCoroutine(SetRectTransform(settings, settingsValue.gameObject));
    }

    //����Ƿ������趨�����ظ�
    public bool CheckBtnRepeat(string path)
    {
        bool iRet = false;
        if(isKeyboard)
        {
            foreach(var kvp in DataManager.instance.buttonSettingsData.Keys_KeyboardDict)
            {
                if (kvp.Key == key_Keyboard) continue;
                if (kvp.Value.currentPath == path) 
                { 
                    iRet = true;
                    sameBtnInfo = kvp.Value;
                    break; 
                }
            }
        }
        else
        {
            foreach (var kvp in DataManager.instance.buttonSettingsData.Keys_GamepadDict)
            {
                if (kvp.Key == key_Gamepad) continue;
                if (kvp.Value.currentPath == path) 
                { 
                    iRet = true;
                    sameBtnInfo = kvp.Value;
                    break; 
                }
            }
        }
        return iRet;
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
        totalWidth = sizeGameObject.GetComponent<RectTransform>().anchoredPosition.x + sizeGameObject.GetComponent<RectTransform>().rect.size.x * sizeGameObject.transform.localScale.x;
        tragGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(totalWidth, tragGameObject.GetComponent<RectTransform>().rect.size.y);
        //RecoveryImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(totalWidth + 80, 0);
    }

    IEnumerator SetIsRecoveryBtnVal()
    {
        yield return new WaitForSeconds(0.1f);
        settingsTips.IsRecoveryBtn(false);
    }

    //���coverPanel���������õ�ǰimage_backΪ��ѡ����
    IEnumerator GetCurrentAndSetSelectedObj()
    {
        yield return null;
        if(EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

    }


}
