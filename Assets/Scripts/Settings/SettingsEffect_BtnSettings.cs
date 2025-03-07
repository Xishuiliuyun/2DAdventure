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
    public Button recoveryAllBtn;//恢复默认设置时使用

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
    //private bool isDeselectCancel;//是否鼠标点击取消按键绑定过程


    private void Start()
    {
        playerInputCtrl = playerController.inputControl;

        //绑定过程的显示效果相关
        image_backSequence = DOTween.Sequence();
        image_backSequence.Append(image_back.DOFade(0, 0.3f));
        image_backSequence.Append(image_back.DOFade(0.8f, 0.3f));
        image_backSequence.SetLoops(-1, LoopType.Restart);
        image_backSequence.SetUpdate(true);//设置暂停后仍播放动画
        image_backSequence.Pause();

        //获取初始按键设置相关信息(已弃用)
        /*if (isKeyboard || key_Keyboard!=Keys_Keyboard.Null)
        {
            //获取BindInfoData
            bindInfoData_Keyboard = DataManager.instance.buttonSettingsData.Keys_KeyboardDict[key_Keyboard];
            currentBindInfoData = bindInfoData_Keyboard;
            if (bindInfoData_Keyboard.currentPath != null)
            {
                //Action和path检查
                if (!CheckActionAndBind(bindInfoData_Keyboard.actionName, bindInfoData_Keyboard.defaultPath)) { Debug.Log("actionORbindIndexERR"); return; }
                //获取bindingIndex
                action = playerInputCtrl.FindAction(bindInfoData_Keyboard.actionName);
                bindingIndex = GetBindingIndex(bindInfoData_Keyboard.actionName, bindInfoData_Keyboard.defaultPath);
            }
            else
            {
                //Action和path检查
                if (!CheckActionAndBind(bindInfoData_Keyboard.actionName, bindInfoData_Keyboard.defaultPath)) { Debug.Log("actionORbindIndexERR"); return; }
                //获取bindingIndex
                action = playerInputCtrl.FindAction(bindInfoData_Keyboard.actionName);
                bindingIndex = GetBindingIndex(bindInfoData_Keyboard.actionName, bindInfoData_Keyboard.defaultPath);
            }
            bindInfoData_Keyboard.bindingIndex = bindingIndex;
            //添加恢复默认值事件
            GetComponent<Button>().onClick.AddListener(() => ReBindingBtn(bindInfoData_Keyboard.actionName, bindInfoData_Keyboard.defaultPath));
        }
        else if (isGamepad || key_Gamepad != Keys_Gamepad.Null)
        {
            //获取BindInfoData
            bindInfoData_Gamepad = DataManager.instance.buttonSettingsData.Keys_GamepadDict[key_Gamepad];
            currentBindInfoData = bindInfoData_Gamepad;
            if (bindInfoData_Gamepad.currentPath != null)
            {
                //Action和path检查
                if (!CheckActionAndBind(bindInfoData_Gamepad.actionName, bindInfoData_Gamepad.defaultPath)) { Debug.Log("actionORbindIndexERR"); return; }
                //获取bindingIndex
                action = playerInputCtrl.FindAction(bindInfoData_Gamepad.actionName);
                bindingIndex = GetBindingIndex(bindInfoData_Gamepad.actionName, bindInfoData_Gamepad.defaultPath);
            }
            else
            {
                //Action和path检查
                if (!CheckActionAndBind(bindInfoData_Gamepad.actionName, bindInfoData_Gamepad.defaultPath)) { Debug.Log("actionORbindIndexERR"); return; }
                //获取bindingIndex
                action = playerInputCtrl.FindAction(bindInfoData_Gamepad.actionName);
                bindingIndex = GetBindingIndex(bindInfoData_Gamepad.actionName, bindInfoData_Gamepad.defaultPath);
            }
            bindInfoData_Gamepad.bindingIndex = bindingIndex;
            //添加恢复默认值事件
            GetComponent<Button>().onClick.AddListener(() => ReBindingBtn(bindInfoData_Gamepad.actionName, bindInfoData_Gamepad.defaultPath));
        }*/

        //获取初始按键设置相关信息
        if (isKeyboard || key_Keyboard != Keys_Keyboard.Null)
        {
            isKeyboard = true;
            //获取BindInfoData
            bindInfoData_Keyboard = DataManager.instance.buttonSettingsData.Keys_KeyboardDict[key_Keyboard];
            if (bindInfoData_Keyboard == null) { Debug.Log("ReadBindInfoDataERR"); return; }
            currentBindInfoData = bindInfoData_Keyboard;
            action = playerInputCtrl.FindAction(bindInfoData_Keyboard.actionName);
            if (bindInfoData_Keyboard.bindingIndex >= 0)//已获取过按键设置信息
            {
                bindingIndex = bindInfoData_Keyboard.bindingIndex;
                //覆写当前按键设置
                action.ApplyBindingOverride(bindingIndex, bindInfoData_Keyboard.currentPath);
                //Debug.Log(action.bindings[bindingIndex].overridePath);
                //Debug.Log(action.bindings[bindingIndex].path);
            }
            else//未获取过按键设置信息
            {
                //Action和path检查
                //if (!CheckActionAndBind(bindInfoData_Keyboard.actionName, bindInfoData_Keyboard.defaultPath)) { Debug.Log("actionORbindIndexERR"); return; }
                //获取bindingIndex
                bindingIndex = GetBindingIndex(bindInfoData_Keyboard.actionName, bindInfoData_Keyboard.defaultPath);
                bindInfoData_Keyboard.bindingIndex = bindingIndex;
            }
            //添加恢复默认值事件
            GetComponent<Button>().onClick.AddListener(() => ReBindingBtn());
        }
        else if (isGamepad || key_Gamepad != Keys_Gamepad.Null)
        {
            isGamepad = true;
            //获取BindInfoData
            bindInfoData_Gamepad = DataManager.instance.buttonSettingsData.Keys_GamepadDict[key_Gamepad];
            if (bindInfoData_Gamepad == null) { Debug.Log("ReadBindInfoDataERR"); return; }
            currentBindInfoData = bindInfoData_Gamepad;
            action = playerInputCtrl.FindAction(bindInfoData_Gamepad.actionName);
            if (bindInfoData_Gamepad.bindingIndex >= 0)//已获取过按键设置信息
            {
                bindingIndex = bindInfoData_Gamepad.bindingIndex;
                //覆写当前按键设置
                action.ApplyBindingOverride(bindingIndex, bindInfoData_Gamepad.currentPath);
                //Debug.Log(action.bindings[bindingIndex].overridePath);
                //Debug.Log(action.bindings[bindingIndex].path);
            }
            else//未获取过按键设置信息
            {
                bindingIndex = GetBindingIndex(bindInfoData_Gamepad.actionName, bindInfoData_Gamepad.defaultPath);
                bindInfoData_Gamepad.bindingIndex = bindingIndex;
            }
            //添加恢复默认值事件
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

        action.Disable();//禁用action
        //执行按键重绑定功能
        PerformInteractiveRebind(bindingIndex);

        //刷新显示内容
        StartCoroutine(SetRectTransform(settings, settingsValue.gameObject));
    }

    //按键重绑定功能
    public void PerformInteractiveRebind(int index)
    {
        //如果有正在执行的按键重绑定，将其取消
        rebindOperation?.Cancel(); // Will null out m_RebindOperation.
        //清除按键重绑定相关的资源占用
        void CleanUp()
        {
            rebindOperation?.Dispose();
            rebindOperation = null;
        }

        //记录修改前的按键path
        if(action.bindings[bindingIndex].overridePath !=null)
        {
            pathBeforeChange = action.bindings[bindingIndex].overridePath;
        }
        else
        {
            pathBeforeChange = action.bindings[bindingIndex].path;
        }
        Debug.Log("InfoBeforeRebinding  "+ "MapName:"+ action.actionMap.name+ "   ActionName:" + action.name +"  ActionIndex:" + bindingIndex+"  OverridePath:" + action.bindings[bindingIndex].overridePath);//输出即将重绑定的按键信息
        rebindOperation = action.PerformInteractiveRebinding(index)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .WithCancelingThrough("<Keyboard>/escape")//设定多种设备的默认返回按键为取消重绑定流程
            .OnCancel(operation => {
                Debug.Log("ReBindingBtn Cancel");
                //取消ButtonSettings对当前脚本的引用
                buttonSettings.btnSettingScript = null;
                //关闭显示效果
                BindShoweEffect(false);
                //刷新按钮显示相关内容
                SetSettingsValue();
                //清除资源
                CleanUp();
            })
            .OnComplete(operation =>
            {
                Debug.Log("ReBindingBtn Complete");
                //取消ButtonSettings对当前脚本的引用
                buttonSettings.btnSettingScript = null;
                //比对字典中的数据，确认是否有重复按键(有重复按键撤销此次按键绑定)
                if(CheckBtnRepeat(action.bindings[bindingIndex].overridePath))
                {//按键设置重复
                    //关闭显示效果
                    BindShoweEffect(false);
                    //弹出提示信息
                    Debug.Log("按键设置重复");
                    string str = "输入的按键和设置“" + sameBtnInfo.settingName+ "”的按键一致，请重新设置按键";
                    UIManager.instance.ShowPromptBox(true, str, "提示", true, 1);
                    UIManager.instance.confirmBtn.onClick.AddListener(UIManager.instance.ClosePromptBox);
                    UIManager.instance.confirmBtn.onClick.AddListener(UIManager.instance.RecoverAlpha);
                    UIManager.instance.confirmBtn.onClick.AddListener(() => { EventSystem.current.SetSelectedGameObject(image_back.gameObject); });
                    UIManager.instance.cancelBtn.onClick.AddListener(UIManager.instance.ClosePromptBox);
                    UIManager.instance.cancelBtn.onClick.AddListener(UIManager.instance.RecoverAlpha);
                    UIManager.instance.cancelBtn.onClick.AddListener(() => { EventSystem.current.SetSelectedGameObject(image_back.gameObject); });
                    //恢复被修改的按键
                    if (pathBeforeChange != null)
                    {
                        Debug.Log("恢复被修改的按键");
                        action.ApplyBindingOverride(bindingIndex, pathBeforeChange);
                    }
                    else { Debug.Log("恢复被修改的按键ERR"); }
                }
                else
                {
                    Debug.Log("InfoAfterRebinding  " + "MapName:" + action.actionMap.name + "   ActionName:" + action.name + "  ActionIndex:" + bindingIndex + "  OverridePath:" + action.bindings[bindingIndex].overridePath);//输出重绑定后的按键信息
                    //记录修改数据
                    //string actionMapJson = action.SaveBindingOverridesAsJson();
                    currentBindInfoData.currentPath = action.bindings[bindingIndex].overridePath;
                    Debug.Log(currentBindInfoData.currentPath);
                    //保存绑定设置
                    DataManager.instance.SaveButtonSettingsData();
                    //关闭显示效果
                    BindShoweEffect(false);
                }

                //刷新按钮显示相关内容
                SetSettingsValue();
                //清除资源
                CleanUp();
            });

        //设置ButtonSettings对当前脚本的引用
        buttonSettings.btnSettingScript = this;
        //显示提示窗相关(鼠标点击后执行取消按键重绑定的流程)
        Debug.Log("BindShoweEffect");
        BindShoweEffect(true);

        rebindOperation.Start();
    }

    //获取bindingIndex
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

    //绑定过程的显示效果
    public void BindShoweEffect(bool isShow)
    {
        //Sequence image_backSequence = DOTween.Sequence();
        if (isShow)
        {
            coverPanel.gameObject.SetActive(true);
            //settingsName.gameObject.SetActive(false);
            settingsValue.gameObject.SetActive(false);
            //image闪烁
            image_backSequence.Play();
        }
        else if(!isShow)
        {
            coverPanel.gameObject.SetActive(false);
            settingsName.gameObject.SetActive(true);
            settingsValue.gameObject.SetActive(true);
            image_backSequence.Pause();
            //设置为默认选项
            //EventSystem.current.SetSelectedGameObject(image_back.gameObject);
            //恢复默认透明度
            image_back.color = new Color(image_back.color.r, image_back.color.g, image_back.color.b, 0.2f);
        }
    }

    //实际已经不会被触发，可弃用
    public void CoverPanelClick()
    {
        Debug.Log("CoverPanelClick");
        rebindOperation.Cancel();
        //BindShoweEffect(false);
    }

    //检查输入的Action和Index是否异常(感觉无作用，已弃用)
    public bool CheckActionAndBind(string actionName, string path)
    {
        bool iRet = false;
        //此判断逻辑已弃用，使用path的方式获取bindingIndex
        //int count = playerInputCtrl.FindAction(actionName).bindings.Count;
        //if (count < bindIndex + 1 || count<=0) iRet = false;
        //else iRet = true;
        int ret =  GetBindingIndex(actionName, path);
        if (ret < 0) iRet = false;
        else iRet = true;
        return iRet;
    }

    //恢复默认按钮  flag为true时不进行按键冲突判定，恢复默认设置时使用
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
        //如果恢复默认按钮时原按键被占，需另行处置
        if(CheckBtnRepeat(currentBindInfoData.defaultPath))
        {//恢复默认按钮时原按键被占
            //弹出提示信息
            Debug.Log("恢复默认按钮时原按键被占");
            string str = "恢复默认按钮时和设置“" + sameBtnInfo.settingName + "”的按键冲突，请修改“" + sameBtnInfo.settingName+ "”的按键后重试";
            UIManager.instance.ShowPromptBox(true, str, "提示", true, 1);
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


    //初始化按键设置的UI
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

    //同步修改按键值
    public void SetSettingsValue()
    {
        settingsValue.text = InputControlPath.ToHumanReadableString(action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        StartCoroutine(SetRectTransform(settings, settingsValue.gameObject));
    }

    //检查是否与已设定按键重复
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

    //自适应调整元素大小
    IEnumerator SetRectTransform(GameObject tragGameObject, GameObject sizeGameObject)
    {
        yield return null;//延迟一帧后执行修改目标物体的尺寸，避免修改UI元素后尺寸未更新导致的尺寸计算异常
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

    //点击coverPanel后重新设置当前image_back为被选择项
    IEnumerator GetCurrentAndSetSelectedObj()
    {
        yield return null;
        if(EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

    }


}
