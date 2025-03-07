using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json;
using System.IO;
using static UnityEngine.Analytics.IAnalytic;
using UnityEditor;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Runtime.InteropServices;
//using static UnityEngine.Rendering.VirtualTexturing.Debugging;
//using System.Diagnostics;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    public SaveType saveType;
    public VoidEvenSO saveDataEvent;
    public Character player;
    public PlayerController playerController;
    private PlayerInputController playerInputCtrl;

    public GeneralSettings generalSettings;
    public VolumeSettings volumeSettings;
    public ScreenSettings screenSettings;

    //记录所有实现了ISaveable接口的实例的列表
    private List<ISaveable> saveableList = new List<ISaveable>();
    //记录所有instance实例化物体的列表
    //private List<PrefabData> gameobjectList = new List<PrefabData>();//(此类型已弃用)
    public List<CollectionData> collectionList = new List<CollectionData>();
    //储存所有运行是状态相关的数据
    public Data saveData ;

    //储存设置相关的数据
    public SettingsData settingsData ;
    //储存按键设置相关数据
    public ButtonSettingsData buttonSettingsData;

    //待删除的游戏对象
    public List<GameObject> delGameObjectList = new List<GameObject>();

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);


        saveData = new Data();
        
    }

    private void Start()
    {
        
        //通过GetComponent方式获取playerInputCtrl失败了，可能是awake执行顺序相关导致的，此处已弃用
        playerInputCtrl = player.GetComponent<PlayerController>().inputControl;
        //playerInputCtrl = playerController.inputControl;
        if (playerInputCtrl == null) Debug.Log("playerInputCtrl_isNullERR");

        buttonSettingsData = new ButtonSettingsData();
        settingsData = new SettingsData();
        settingsData.generalSettingsData = new GeneralSettingsData();
        settingsData.volumeSettingsData = new VolumeSettingsData();
        settingsData.screensettingsData = new ScreenSettingsData();

        //读取设置相关数据
        GetSettingsValue();
        //加载设置项
        //SetSettingsValue();
        //PrintButtonSettings();
    }


    private void OnEnable()
    {
        saveDataEvent.OnEventRised += Save;
    }
    private void OnDisable()
    {
        saveDataEvent.OnEventRised -= Save;
    }

    //测试读取存档用
    /*private void Update()
    {
        if(Keyboard.current.lKey.wasPressedThisFrame)
        {
            Load();
        }
    }*/

    public void RegisterSaveData(ISaveable saveable)
    {
        if(!saveableList.Contains(saveable))
        {
            saveableList.Add(saveable);
        }
    }

    public void UnRegisterSaveData(ISaveable saveable)
    {
        saveableList.Remove(saveable);
    }

    public void Save()
    {
        foreach(ISaveable saveable in saveableList)
        {
            saveable.GetSaveData(saveData);
        }
        //记录场景信息
        saveData.currentScene = SceneManager.GetSceneAt(1).name;
        //场景中固定物体的序列化存储
        string json = JsonConvert.SerializeObject(saveData,Formatting.Indented);
        string path = Path.Combine(Application.persistentDataPath, Constants.SAVE_DATA_FILE_NAME);
        System.IO.File.WriteAllText(path, json);
        //instance实例化物体的序列化存储
        string json2 = JsonConvert.SerializeObject(collectionList, Formatting.Indented);
        string path2 = Path.Combine(Application.persistentDataPath, Constants.COLLECTION_DATA_FILE_NAME);
        System.IO.File.WriteAllText(path2, json2);
        SyncFiles();
    }

    //WEBGL项目主动保存文件数据用
    public void SyncFiles()
    {
        if (Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_WEBGL)
        {
            // 调用 JavaScript 函数同步文件
            Application.ExternalEval(@"
            FS.syncfs(false, function (err) {
                if (err) {
                    console.error('Failed to sync files:', err);
                } else {
                    //console.log('Files synced successfully');
                }
            });
        ");
        }
        else return;
    }

    public async void Load()
    {
        //场景中固定物体的数据读取设置
        string path = Path.Combine(Application.persistentDataPath, Constants.SAVE_DATA_FILE_NAME);
        if (!FileCheck(path)) Debug.Log("FileNotExist");
        string json = System.IO.File.ReadAllText(path);
        Data data = JsonConvert.DeserializeObject<Data>(json);
        foreach (ISaveable saveable in saveableList)
        {
            saveable.LoadData(data);
        }
        DestroyAfterLoad();
        //未保存阵亡的enemy的重新instance
        //当前以查找遍历的方式确认场景是否有enemy缺失，也可以用加载场景后删除全部enemy，后续根据数据instance实例的方式实现
        GameObject[] enemyArr = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (KeyValuePair<string, EnemyData> enemyData in data.enemyDataDict)
        {
            //该enemy在当前场景
            if(enemyData.Value.sceneName == SceneManager.GetSceneAt(1).name)
            {
                if (enemyData.Value.isDead == false)
                {
                    bool enemyExist = false;
                    foreach (GameObject enemy in enemyArr)
                    {
                        if (enemy.GetComponent<DataDefinition>().ID == enemyData.Key) { enemyExist = true; break; }
                    }
                    if (!enemyExist)//场景中缺少未死亡的enemy，将其实例化
                    {
                        string path22 = Constants.PREFAB_PATH + enemyData.Value.enemyName + ".prefab";
                        //此方式动态获取资源只可以在编辑器模式下使用(已弃用)
                        //GameObject prefabGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(Constants.PREFAB_PATH + enemyData.Value.enemyName + ".prefab");
                        //GameObject enemy = Instantiate(prefabGameObject, enemyData.Value.pos.GetValue(), Quaternion.Euler(0, 0, 0));
                        (bool flag, GameObject prefabGameObject) = await LoadResourceAsync<GameObject>(enemyData.Value.enemyName);
                        if (!flag) { Debug.Log("LoadResourceAsyncERR"); return; }
                        GameObject enemy = Instantiate(prefabGameObject, enemyData.Value.pos.GetValue(), Quaternion.Euler(0, 0, 0));
                        enemy.GetComponent<Character>().GetEnemyData(enemyData.Value);
                        enemy.GetComponent<DataDefinition>().ID = enemyData.Key;
                    }
                }
            }
        }


        //场景中instance实例化物体的数据读取设置
        string path2 = Path.Combine(Application.persistentDataPath, Constants.COLLECTION_DATA_FILE_NAME);
        if (!FileCheck(path2)) Debug.Log("FileNotExist");
        string json2 = System.IO.File.ReadAllText(path2);
        List<CollectionData> data2 = JsonConvert.DeserializeObject<List<CollectionData>>(json2);
        string currentScene = SceneManager.GetSceneAt(1).name;
        for (var i = 0; i < data2.Count; i++)
        {
            CollectionData collecData = data2[i];
            
            if (currentScene == collecData.sceneName)
            {
                //此方式动态获取资源只可以在编辑器模式下使用(已弃用)
                //GameObject prefabGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(Constants.PREFAB_PATH + collecData.prefabName + ".prefab");
                //Instantiate(prefabGameObject, collecData.pos.GetValue(), Quaternion.Euler(0, 0, 0));
                (bool flag, GameObject prefabGameObject) = await LoadResourceAsync<GameObject>(collecData.prefabName);
                if (!flag) { Debug.Log("LoadResourceAsyncERR");return; }
                Instantiate(prefabGameObject, collecData.pos.GetValue(), Quaternion.Euler(0, 0, 0));
                data2.Remove(collecData);
                i--;
            }
        }
        //已加载的instance物体数据，从列表移除，这样就不会重复写入相同的数据了
        collectionList = data2;
    }

    //资源加载用
    public async Task<(bool flag,T resource)> LoadResourceAsync<T>(string assetPath) where T : Object
    {
        try
        {
            // 使用Addressables.LoadAssetAsync<T>加载资源
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetPath);
            T loadedResource = await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                //Addressables.Release(handle);
                return (true, loadedResource);
            }
            else
            {
                Debug.LogError($"Failed to load resource from path: {assetPath}. Error: {handle.OperationException}");
                Addressables.Release(handle);
                return (false,null);
            }
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Exception occurred while loading resource from path: {assetPath}. Exception: {e.Message}");
            return (false, null);
        }
        
    }

    public void AddToDelList(GameObject gameObject)
    {
        delGameObjectList.Add(gameObject);
    }

    public void DestroyAfterLoad()
    {
        foreach (GameObject gameObject in delGameObjectList)
        {
            Destroy(gameObject);
        }
        delGameObjectList.Clear();
    }

    //(已弃用，还是需要记录怪物等东西的信息)场景加载完成后执行，记录一下Player坐标和当前场景
    public void SaveAfterSceneLoad()
    {
        player.GetSaveData(saveData);
        saveData.currentScene = SceneManager.GetSceneAt(1).name;
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        string path = Path.Combine(Application.persistentDataPath, Constants.SAVE_DATA_FILE_NAME);
        System.IO.File.WriteAllText(path, json);
        SyncFiles();
    }

    //死亡后设置enemy状态
    public bool SetEnemyDead(string UID)
    {
        bool iResult;
        if (saveData.enemyDataDict.ContainsKey(UID))
        {
            saveData.enemyDataDict[UID].isDead = true;
            iResult = true;
        }
        else { iResult = false; Debug.Log("存档设置enemy死亡状态失败"); }
        return iResult;
    }


    //将instance的物体的数据添加到列表
    public void AddCollectionToList(CollectionData collectionData)
    {
        //如果存档中已经存在，无需处理，只需在初始记录一次就够了
        if (collectionList.Contains(collectionData)) return;
        else
        {
            collectionList.Add(collectionData);
        }
    }
    //从列表移除instance的物体的数据
    public void RemoveCollectionFromList(CollectionData collectionData)
    {
        collectionList.Remove(collectionData);
    }
    //清除会重复的instance的物体的数据
    public void ClearCollectionList()
    {
        collectionList.Clear();
    }


    public string GetSceneName()
    {
        return saveData.currentScene;
    }
    public Vector3 GetPlayerPos()
    {
        return saveData.playerDataDict[player.GetComponent<DataDefinition>().ID].pos.GetValue();
    }

    public void SetPlayerData()
    {
        PlayerData playerData = new PlayerData();
        player.SetPlayerData(playerData);
        if (saveData.playerDataDict.ContainsKey(player.GetComponent<DataDefinition>().ID))
        {
            saveData.playerDataDict[player.GetComponent<DataDefinition>().ID] = playerData;
        }
        else
        {
            saveData.playerDataDict.Add(player.GetComponent<DataDefinition>().ID, playerData);
        }
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        string path = Path.Combine(Application.persistentDataPath, Constants.SAVE_DATA_FILE_NAME);
        System.IO.File.WriteAllText(path, json);
        SyncFiles();
    }
    public void SetSceneData()
    {
        saveData.currentScene = SceneManager.GetSceneAt(1).name;
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        string path = Path.Combine(Application.persistentDataPath, Constants.SAVE_DATA_FILE_NAME);
        System.IO.File.WriteAllText(path, json);
        SyncFiles();
    }

    public bool FileCheck(string path)
    {
        bool iRet = false;
        if (File.Exists(path)) iRet = true;
        else iRet = false;
        return iRet;
    }


    //清除数据变量及数据文件中的数据
    public void ClearData()
    {
        //saveableList.Clear();
        collectionList.Clear();
        delGameObjectList.Clear();
        saveData.currentScene = null;
        saveData.playerDataDict.Clear();
        saveData.enemyDataDict.Clear();
        saveData.interactableDataDict.Clear();
        saveData.houseDataDict.Clear();

        string path = Path.Combine(Application.persistentDataPath, Constants.SAVE_DATA_FILE_NAME);
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        string path2 = Path.Combine(Application.persistentDataPath, Constants.COLLECTION_DATA_FILE_NAME);
        string json2 = JsonConvert.SerializeObject(collectionList, Formatting.Indented);
        /*if (FileCheck(path))*/System.IO.File.WriteAllText(path, json);//无论是否存在文件，都应该生成写入空白数据
        /*if (FileCheck(path2))*/System.IO.File.WriteAllText(path2, json2);//无论是否存在文件，都应该生成写入空白数据
        SyncFiles();

    }

    //读文件覆写当前saveData、collectionList中的数据
    public void OverwriteData()
    {
        string path = Path.Combine(Application.persistentDataPath, Constants.SAVE_DATA_FILE_NAME);
        if (!FileCheck(path)) Debug.Log("FileNotExist");
        string json = System.IO.File.ReadAllText(path);
        saveData = JsonConvert.DeserializeObject<Data>(json);

        string path2 = Path.Combine(Application.persistentDataPath, Constants.COLLECTION_DATA_FILE_NAME);
        if (!FileCheck(path2)) Debug.Log("FileNotExist");
        string json2 = System.IO.File.ReadAllText(path2);
        collectionList = JsonConvert.DeserializeObject<List<CollectionData>>(json2);

    }


    //输出绑定的键盘按键_测试用(已弃用)
    public void PrintButtonSettings()
    {//SaveBindingOverridesAsJson 方法只会保存那些在运行时被修改过的绑定,如果你没有在运行时改变任何绑定，那么这个方法返回的 JSON 字符串将是空的。
        PlayerInputController playerInput = new PlayerInputController();
        string actionMapJson = playerInput.SaveBindingOverridesAsJson();
        string path = Path.Combine(Application.persistentDataPath, Constants.BUTTON_SETTINGS_FILE_NAME);
        System.IO.File.WriteAllText(path, actionMapJson);
        Debug.Log("PrintButtonSettings_success");
        SyncFiles();
    }

    //加载按键设置文件
    public void LoadButtonSettingFile()
    {
        string path = Path.Combine(Application.persistentDataPath, Constants.BUTTON_SETTINGS_FILE_NAME);
        if (!File.Exists(path)) //不存在按键设置文件
        {
            InitButtonSettings();
            Debug.Log("BtnSettingFileNotExist  InitButtonSettings");
        }
        else//存在按键设置文件
        {
            string json = System.IO.File.ReadAllText(path);
            buttonSettingsData = JsonConvert.DeserializeObject<ButtonSettingsData>(json);
            if (buttonSettingsData == null) { Debug.Log("LoadButtonSettingFile_ERR");return; }
            //加载所有按键设置项
            SetButtonSetting();
        }

    }

    //初始化所有按键设置(初始化应该只需要执行一次，有按键的存档数据后应该就不用再使用了，可能重置全部按键时也可以使用)
    public void InitButtonSettings()
    {
        //键盘按键初始化
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.MoveUp1, new BindInfoData { settingName = "移动(上)", actionName = "Move", currentPath = "<Keyboard>/w", defaultPath = "<Keyboard>/w", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.MoveUp2, new BindInfoData { settingName = "移动(上)", actionName = "Move", currentPath = "<Keyboard>/upArrow", defaultPath = "<Keyboard>/upArrow", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.MoveDown1, new BindInfoData { settingName = "移动(下)", actionName = "Move", currentPath = "<Keyboard>/s", defaultPath = "<Keyboard>/s", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.MoveDown2, new BindInfoData { settingName = "移动(下)", actionName = "Move", currentPath = "<Keyboard>/downArrow", defaultPath = "<Keyboard>/downArrow", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.MoveLeft1, new BindInfoData { settingName = "移动(左)", actionName = "Move", currentPath = "<Keyboard>/a", defaultPath = "<Keyboard>/a", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.MoveLeft2, new BindInfoData { settingName = "移动(左)", actionName = "Move", currentPath = "<Keyboard>/leftArrow", defaultPath = "<Keyboard>/leftArrow", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.MoveRight1, new BindInfoData { settingName = "移动(右)", actionName = "Move", currentPath = "<Keyboard>/d", defaultPath = "<Keyboard>/d", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.MoveRight2, new BindInfoData { settingName = "移动(右)", actionName = "Move", currentPath = "<Keyboard>/rightArrow", defaultPath = "<Keyboard>/rightArrow", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.Attack1, new BindInfoData { settingName = "攻击", actionName = "Attack", currentPath = "<Keyboard>/j", defaultPath = "<Keyboard>/j", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.Attack2, new BindInfoData { settingName = "攻击", actionName = "Attack", currentPath = "<Mouse>/leftButton", defaultPath = "<Mouse>/leftButton", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.Jump, new BindInfoData { settingName = "跳跃", actionName = "Jump", currentPath = "<Keyboard>/space", defaultPath = "<Keyboard>/space", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.Confirm_E, new BindInfoData { settingName = "交互1", actionName = "Confirm_E", currentPath = "<Keyboard>/e", defaultPath = "<Keyboard>/e", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.Confirm_F, new BindInfoData { settingName = "交互2", actionName = "Confirm_F", currentPath = "<Keyboard>/f", defaultPath = "<Keyboard>/f", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.Slid, new BindInfoData { settingName = "滑铲", actionName = "Slid", currentPath = "<Keyboard>/leftShift", defaultPath = "<Keyboard>/leftShift", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.ESC, new BindInfoData { settingName = "暂停", actionName = "Pause", currentPath = "<Keyboard>/escape", defaultPath = "<Keyboard>/escape", bindingIndex = -1 });
        //buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.Back, new BindInfoData { settingName = "返回", actionName = "Back", currentPath = "<Keyboard>/escape", defaultPath = "<Keyboard>/escape", bindingIndex = -1 });

        //手柄按键初始化
        buttonSettingsData.Keys_GamepadDict.Add(Keys_Gamepad.Attack, new BindInfoData { settingName = "攻击", actionName = "Attack", currentPath = "<Gamepad>/buttonWest", defaultPath = "<Gamepad>/buttonWest", bindingIndex = -1 });
        buttonSettingsData.Keys_GamepadDict.Add(Keys_Gamepad.Jump, new BindInfoData { settingName = "跳跃", actionName = "Jump", currentPath = "<Gamepad>/buttonSouth", defaultPath = "<Gamepad>/buttonSouth", bindingIndex = -1 });
        buttonSettingsData.Keys_GamepadDict.Add(Keys_Gamepad.Confirm_E, new BindInfoData { settingName = "交互1", actionName = "Confirm_E", currentPath = "<Gamepad>/buttonEast", defaultPath = "<Gamepad>/buttonEast", bindingIndex = -1 });
        buttonSettingsData.Keys_GamepadDict.Add(Keys_Gamepad.Confirm_F, new BindInfoData { settingName = "交互2", actionName = "Confirm_F", currentPath = "<Gamepad>/buttonNorth", defaultPath = "<Gamepad>/buttonNorth", bindingIndex = -1 });
        buttonSettingsData.Keys_GamepadDict.Add(Keys_Gamepad.Slid, new BindInfoData { settingName = "滑铲", actionName = "Slid", currentPath = "<Gamepad>/rightTrigger", defaultPath = "<Gamepad>/rightTrigger", bindingIndex = -1 });
        buttonSettingsData.Keys_GamepadDict.Add(Keys_Gamepad.ESC, new BindInfoData { settingName = "暂停", actionName = "Pause", currentPath = "<Gamepad>/start", defaultPath = "<Gamepad>/start", bindingIndex = -1 });
        //buttonSettingsData.Keys_GamepadDict.Add(Keys_Gamepad.Back, new BindInfoData { settingName = "返回", actionName = "Back", currentPath = "<Gamepad>/buttonEast", defaultPath = "<Gamepad>/buttonEast", bindingIndex = -1});
        SaveButtonSettingsData();
    }
    //保存按键设置
    public void SaveButtonSettingsData()
    {
        string buttonSettingsData1 = JsonConvert.SerializeObject(buttonSettingsData, Formatting.Indented);
        string path = Path.Combine(Application.persistentDataPath, Constants.BUTTON_SETTINGS_FILE_NAME);
        System.IO.File.WriteAllText(path, buttonSettingsData1);
        SyncFiles();
    }
    //读取按键设置
    public void SetButtonSetting()
    {
        Dictionary<Keys_Keyboard, BindInfoData> KeyboardDict = buttonSettingsData.Keys_KeyboardDict;
        foreach (var key in KeyboardDict)
        {
            if (key.Value.bindingIndex == -1) return;//此按键没进行过修改，无需读取数据重设
            InputAction action = playerInputCtrl.FindAction(key.Value.actionName);
            if (action == null) { Debug.Log("SetButtonSetting_ERR"); continue; }
            action.ApplyBindingOverride(key.Value.bindingIndex, key.Value.currentPath);
        }
        Dictionary<Keys_Gamepad, BindInfoData> GamepadDict = buttonSettingsData.Keys_GamepadDict;
        foreach (var key in GamepadDict)
        {
            if (key.Value.bindingIndex == -1) return;//此按键没进行过修改，无需读取数据重设
            InputAction action = playerInputCtrl.FindAction(key.Value.actionName);
            if (action == null) { Debug.Log("SetButtonSetting_ERR"); continue; }
            action.ApplyBindingOverride(key.Value.bindingIndex, key.Value.currentPath);
        }
    }

    //初始化设置
    public void InitSettingsData()
    {
        settingsData.generalSettingsData = new GeneralSettingsData { showEnemyHP = false,showDamage = false,isScreenShock = true,handleShock = 1,language = Language.Chinese };
        settingsData.volumeSettingsData = new VolumeSettingsData {masterVolume = 0.8f,BGMVolume = 0.9f, FXVolume = 0.7f };
        settingsData.screensettingsData = new ScreenSettingsData { fullScreen = true,dynamicBlur = false,verticalSynchronization = false,brightness = 0.6f,contrastRatio = 0.5f,resolvingPower = ResolvingPower.Type1 };
        SaveSettingsData();
        generalSettings.SetSettingsValue();
        volumeSettings.SetSettingsValue();
        screenSettings.SetSettingsValue();
    }

    public void LoadSettingsFile()
    {
        string path = Path.Combine(Application.persistentDataPath, Constants.SETTINGS_FILE_NAME);
        if (!File.Exists(path)) //不存在设置文件
        {
            InitSettingsData();
            Debug.Log("SettingsFileNotExist  InitSettingsData");
        }
        else//存在设置文件
        {
            string json = System.IO.File.ReadAllText(path);
            settingsData = JsonConvert.DeserializeObject<SettingsData>(json);
            //加载设置项
            generalSettings.SetSettingsValue();
            volumeSettings.SetSettingsValue();
            screenSettings.SetSettingsValue();
        }
    }

    //初始读取设置相关数据
    public void GetSettingsValue()
    {
        LoadButtonSettingFile();//加载按键设置文件
        LoadSettingsFile();
    }

    //保存设置项
    public void SaveSettingsData()
    {
        string settingsData1 = JsonConvert.SerializeObject(settingsData, Formatting.Indented);
        string path = Path.Combine(Application.persistentDataPath, Constants.SETTINGS_FILE_NAME);
        System.IO.File.WriteAllText(path, settingsData1);
        SyncFiles();
    }
}
