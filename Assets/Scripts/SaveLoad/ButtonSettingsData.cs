using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ButtonSettingsData 
{
    public Dictionary<Keys_Keyboard, BindInfoData> Keys_KeyboardDict = new Dictionary<Keys_Keyboard, BindInfoData>();
    public Dictionary<Keys_Gamepad, BindInfoData> Keys_GamepadDict = new Dictionary<Keys_Gamepad, BindInfoData>();

}

public class BindInfoData
{
    public string settingName;
    public string actionName;
    public string currentPath;
    public string defaultPath;
    //public int ctrlIndex;
    public int bindingIndex;
    //public KeyCode keyCode;
    //public KeyCode defaultKeyCode;
    //public GUID guid;//当前感觉使用也没有意义
}








