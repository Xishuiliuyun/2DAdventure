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

    //��¼����ʵ����ISaveable�ӿڵ�ʵ�����б�
    private List<ISaveable> saveableList = new List<ISaveable>();
    //��¼����instanceʵ����������б�
    //private List<PrefabData> gameobjectList = new List<PrefabData>();//(������������)
    public List<CollectionData> collectionList = new List<CollectionData>();
    //��������������״̬��ص�����
    public Data saveData ;

    //����������ص�����
    public SettingsData settingsData ;
    //���水�������������
    public ButtonSettingsData buttonSettingsData;

    //��ɾ������Ϸ����
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
        
        //ͨ��GetComponent��ʽ��ȡplayerInputCtrlʧ���ˣ�������awakeִ��˳����ص��µģ��˴�������
        playerInputCtrl = player.GetComponent<PlayerController>().inputControl;
        //playerInputCtrl = playerController.inputControl;
        if (playerInputCtrl == null) Debug.Log("playerInputCtrl_isNullERR");

        buttonSettingsData = new ButtonSettingsData();
        settingsData = new SettingsData();
        settingsData.generalSettingsData = new GeneralSettingsData();
        settingsData.volumeSettingsData = new VolumeSettingsData();
        settingsData.screensettingsData = new ScreenSettingsData();

        //��ȡ�����������
        GetSettingsValue();
        //����������
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

    //���Զ�ȡ�浵��
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
        //��¼������Ϣ
        saveData.currentScene = SceneManager.GetSceneAt(1).name;
        //�����й̶���������л��洢
        string json = JsonConvert.SerializeObject(saveData,Formatting.Indented);
        string path = Path.Combine(Application.persistentDataPath, Constants.SAVE_DATA_FILE_NAME);
        System.IO.File.WriteAllText(path, json);
        //instanceʵ������������л��洢
        string json2 = JsonConvert.SerializeObject(collectionList, Formatting.Indented);
        string path2 = Path.Combine(Application.persistentDataPath, Constants.COLLECTION_DATA_FILE_NAME);
        System.IO.File.WriteAllText(path2, json2);
        SyncFiles();
    }

    //WEBGL��Ŀ���������ļ�������
    public void SyncFiles()
    {
        if (Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_WEBGL)
        {
            // ���� JavaScript ����ͬ���ļ�
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
        //�����й̶���������ݶ�ȡ����
        string path = Path.Combine(Application.persistentDataPath, Constants.SAVE_DATA_FILE_NAME);
        if (!FileCheck(path)) Debug.Log("FileNotExist");
        string json = System.IO.File.ReadAllText(path);
        Data data = JsonConvert.DeserializeObject<Data>(json);
        foreach (ISaveable saveable in saveableList)
        {
            saveable.LoadData(data);
        }
        DestroyAfterLoad();
        //δ����������enemy������instance
        //��ǰ�Բ��ұ����ķ�ʽȷ�ϳ����Ƿ���enemyȱʧ��Ҳ�����ü��س�����ɾ��ȫ��enemy��������������instanceʵ���ķ�ʽʵ��
        GameObject[] enemyArr = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (KeyValuePair<string, EnemyData> enemyData in data.enemyDataDict)
        {
            //��enemy�ڵ�ǰ����
            if(enemyData.Value.sceneName == SceneManager.GetSceneAt(1).name)
            {
                if (enemyData.Value.isDead == false)
                {
                    bool enemyExist = false;
                    foreach (GameObject enemy in enemyArr)
                    {
                        if (enemy.GetComponent<DataDefinition>().ID == enemyData.Key) { enemyExist = true; break; }
                    }
                    if (!enemyExist)//������ȱ��δ������enemy������ʵ����
                    {
                        string path22 = Constants.PREFAB_PATH + enemyData.Value.enemyName + ".prefab";
                        //�˷�ʽ��̬��ȡ��Դֻ�����ڱ༭��ģʽ��ʹ��(������)
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


        //������instanceʵ������������ݶ�ȡ����
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
                //�˷�ʽ��̬��ȡ��Դֻ�����ڱ༭��ģʽ��ʹ��(������)
                //GameObject prefabGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(Constants.PREFAB_PATH + collecData.prefabName + ".prefab");
                //Instantiate(prefabGameObject, collecData.pos.GetValue(), Quaternion.Euler(0, 0, 0));
                (bool flag, GameObject prefabGameObject) = await LoadResourceAsync<GameObject>(collecData.prefabName);
                if (!flag) { Debug.Log("LoadResourceAsyncERR");return; }
                Instantiate(prefabGameObject, collecData.pos.GetValue(), Quaternion.Euler(0, 0, 0));
                data2.Remove(collecData);
                i--;
            }
        }
        //�Ѽ��ص�instance�������ݣ����б��Ƴ��������Ͳ����ظ�д����ͬ��������
        collectionList = data2;
    }

    //��Դ������
    public async Task<(bool flag,T resource)> LoadResourceAsync<T>(string assetPath) where T : Object
    {
        try
        {
            // ʹ��Addressables.LoadAssetAsync<T>������Դ
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

    //(�����ã�������Ҫ��¼����ȶ�������Ϣ)����������ɺ�ִ�У���¼һ��Player����͵�ǰ����
    public void SaveAfterSceneLoad()
    {
        player.GetSaveData(saveData);
        saveData.currentScene = SceneManager.GetSceneAt(1).name;
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        string path = Path.Combine(Application.persistentDataPath, Constants.SAVE_DATA_FILE_NAME);
        System.IO.File.WriteAllText(path, json);
        SyncFiles();
    }

    //����������enemy״̬
    public bool SetEnemyDead(string UID)
    {
        bool iResult;
        if (saveData.enemyDataDict.ContainsKey(UID))
        {
            saveData.enemyDataDict[UID].isDead = true;
            iResult = true;
        }
        else { iResult = false; Debug.Log("�浵����enemy����״̬ʧ��"); }
        return iResult;
    }


    //��instance�������������ӵ��б�
    public void AddCollectionToList(CollectionData collectionData)
    {
        //����浵���Ѿ����ڣ����账��ֻ���ڳ�ʼ��¼һ�ξ͹���
        if (collectionList.Contains(collectionData)) return;
        else
        {
            collectionList.Add(collectionData);
        }
    }
    //���б��Ƴ�instance�����������
    public void RemoveCollectionFromList(CollectionData collectionData)
    {
        collectionList.Remove(collectionData);
    }
    //������ظ���instance�����������
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


    //������ݱ����������ļ��е�����
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
        /*if (FileCheck(path))*/System.IO.File.WriteAllText(path, json);//�����Ƿ�����ļ�����Ӧ������д��հ�����
        /*if (FileCheck(path2))*/System.IO.File.WriteAllText(path2, json2);//�����Ƿ�����ļ�����Ӧ������д��հ�����
        SyncFiles();

    }

    //���ļ���д��ǰsaveData��collectionList�е�����
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


    //����󶨵ļ��̰���_������(������)
    public void PrintButtonSettings()
    {//SaveBindingOverridesAsJson ����ֻ�ᱣ����Щ������ʱ���޸Ĺ��İ�,�����û��������ʱ�ı��κΰ󶨣���ô����������ص� JSON �ַ������ǿյġ�
        PlayerInputController playerInput = new PlayerInputController();
        string actionMapJson = playerInput.SaveBindingOverridesAsJson();
        string path = Path.Combine(Application.persistentDataPath, Constants.BUTTON_SETTINGS_FILE_NAME);
        System.IO.File.WriteAllText(path, actionMapJson);
        Debug.Log("PrintButtonSettings_success");
        SyncFiles();
    }

    //���ذ��������ļ�
    public void LoadButtonSettingFile()
    {
        string path = Path.Combine(Application.persistentDataPath, Constants.BUTTON_SETTINGS_FILE_NAME);
        if (!File.Exists(path)) //�����ڰ��������ļ�
        {
            InitButtonSettings();
            Debug.Log("BtnSettingFileNotExist  InitButtonSettings");
        }
        else//���ڰ��������ļ�
        {
            string json = System.IO.File.ReadAllText(path);
            buttonSettingsData = JsonConvert.DeserializeObject<ButtonSettingsData>(json);
            if (buttonSettingsData == null) { Debug.Log("LoadButtonSettingFile_ERR");return; }
            //�������а���������
            SetButtonSetting();
        }

    }

    //��ʼ�����а�������(��ʼ��Ӧ��ֻ��Ҫִ��һ�Σ��а����Ĵ浵���ݺ�Ӧ�þͲ�����ʹ���ˣ���������ȫ������ʱҲ����ʹ��)
    public void InitButtonSettings()
    {
        //���̰�����ʼ��
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.MoveUp1, new BindInfoData { settingName = "�ƶ�(��)", actionName = "Move", currentPath = "<Keyboard>/w", defaultPath = "<Keyboard>/w", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.MoveUp2, new BindInfoData { settingName = "�ƶ�(��)", actionName = "Move", currentPath = "<Keyboard>/upArrow", defaultPath = "<Keyboard>/upArrow", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.MoveDown1, new BindInfoData { settingName = "�ƶ�(��)", actionName = "Move", currentPath = "<Keyboard>/s", defaultPath = "<Keyboard>/s", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.MoveDown2, new BindInfoData { settingName = "�ƶ�(��)", actionName = "Move", currentPath = "<Keyboard>/downArrow", defaultPath = "<Keyboard>/downArrow", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.MoveLeft1, new BindInfoData { settingName = "�ƶ�(��)", actionName = "Move", currentPath = "<Keyboard>/a", defaultPath = "<Keyboard>/a", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.MoveLeft2, new BindInfoData { settingName = "�ƶ�(��)", actionName = "Move", currentPath = "<Keyboard>/leftArrow", defaultPath = "<Keyboard>/leftArrow", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.MoveRight1, new BindInfoData { settingName = "�ƶ�(��)", actionName = "Move", currentPath = "<Keyboard>/d", defaultPath = "<Keyboard>/d", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.MoveRight2, new BindInfoData { settingName = "�ƶ�(��)", actionName = "Move", currentPath = "<Keyboard>/rightArrow", defaultPath = "<Keyboard>/rightArrow", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.Attack1, new BindInfoData { settingName = "����", actionName = "Attack", currentPath = "<Keyboard>/j", defaultPath = "<Keyboard>/j", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.Attack2, new BindInfoData { settingName = "����", actionName = "Attack", currentPath = "<Mouse>/leftButton", defaultPath = "<Mouse>/leftButton", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.Jump, new BindInfoData { settingName = "��Ծ", actionName = "Jump", currentPath = "<Keyboard>/space", defaultPath = "<Keyboard>/space", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.Confirm_E, new BindInfoData { settingName = "����1", actionName = "Confirm_E", currentPath = "<Keyboard>/e", defaultPath = "<Keyboard>/e", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.Confirm_F, new BindInfoData { settingName = "����2", actionName = "Confirm_F", currentPath = "<Keyboard>/f", defaultPath = "<Keyboard>/f", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.Slid, new BindInfoData { settingName = "����", actionName = "Slid", currentPath = "<Keyboard>/leftShift", defaultPath = "<Keyboard>/leftShift", bindingIndex = -1 });
        buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.ESC, new BindInfoData { settingName = "��ͣ", actionName = "Pause", currentPath = "<Keyboard>/escape", defaultPath = "<Keyboard>/escape", bindingIndex = -1 });
        //buttonSettingsData.Keys_KeyboardDict.Add(Keys_Keyboard.Back, new BindInfoData { settingName = "����", actionName = "Back", currentPath = "<Keyboard>/escape", defaultPath = "<Keyboard>/escape", bindingIndex = -1 });

        //�ֱ�������ʼ��
        buttonSettingsData.Keys_GamepadDict.Add(Keys_Gamepad.Attack, new BindInfoData { settingName = "����", actionName = "Attack", currentPath = "<Gamepad>/buttonWest", defaultPath = "<Gamepad>/buttonWest", bindingIndex = -1 });
        buttonSettingsData.Keys_GamepadDict.Add(Keys_Gamepad.Jump, new BindInfoData { settingName = "��Ծ", actionName = "Jump", currentPath = "<Gamepad>/buttonSouth", defaultPath = "<Gamepad>/buttonSouth", bindingIndex = -1 });
        buttonSettingsData.Keys_GamepadDict.Add(Keys_Gamepad.Confirm_E, new BindInfoData { settingName = "����1", actionName = "Confirm_E", currentPath = "<Gamepad>/buttonEast", defaultPath = "<Gamepad>/buttonEast", bindingIndex = -1 });
        buttonSettingsData.Keys_GamepadDict.Add(Keys_Gamepad.Confirm_F, new BindInfoData { settingName = "����2", actionName = "Confirm_F", currentPath = "<Gamepad>/buttonNorth", defaultPath = "<Gamepad>/buttonNorth", bindingIndex = -1 });
        buttonSettingsData.Keys_GamepadDict.Add(Keys_Gamepad.Slid, new BindInfoData { settingName = "����", actionName = "Slid", currentPath = "<Gamepad>/rightTrigger", defaultPath = "<Gamepad>/rightTrigger", bindingIndex = -1 });
        buttonSettingsData.Keys_GamepadDict.Add(Keys_Gamepad.ESC, new BindInfoData { settingName = "��ͣ", actionName = "Pause", currentPath = "<Gamepad>/start", defaultPath = "<Gamepad>/start", bindingIndex = -1 });
        //buttonSettingsData.Keys_GamepadDict.Add(Keys_Gamepad.Back, new BindInfoData { settingName = "����", actionName = "Back", currentPath = "<Gamepad>/buttonEast", defaultPath = "<Gamepad>/buttonEast", bindingIndex = -1});
        SaveButtonSettingsData();
    }
    //���水������
    public void SaveButtonSettingsData()
    {
        string buttonSettingsData1 = JsonConvert.SerializeObject(buttonSettingsData, Formatting.Indented);
        string path = Path.Combine(Application.persistentDataPath, Constants.BUTTON_SETTINGS_FILE_NAME);
        System.IO.File.WriteAllText(path, buttonSettingsData1);
        SyncFiles();
    }
    //��ȡ��������
    public void SetButtonSetting()
    {
        Dictionary<Keys_Keyboard, BindInfoData> KeyboardDict = buttonSettingsData.Keys_KeyboardDict;
        foreach (var key in KeyboardDict)
        {
            if (key.Value.bindingIndex == -1) return;//�˰���û���й��޸ģ������ȡ��������
            InputAction action = playerInputCtrl.FindAction(key.Value.actionName);
            if (action == null) { Debug.Log("SetButtonSetting_ERR"); continue; }
            action.ApplyBindingOverride(key.Value.bindingIndex, key.Value.currentPath);
        }
        Dictionary<Keys_Gamepad, BindInfoData> GamepadDict = buttonSettingsData.Keys_GamepadDict;
        foreach (var key in GamepadDict)
        {
            if (key.Value.bindingIndex == -1) return;//�˰���û���й��޸ģ������ȡ��������
            InputAction action = playerInputCtrl.FindAction(key.Value.actionName);
            if (action == null) { Debug.Log("SetButtonSetting_ERR"); continue; }
            action.ApplyBindingOverride(key.Value.bindingIndex, key.Value.currentPath);
        }
    }

    //��ʼ������
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
        if (!File.Exists(path)) //�����������ļ�
        {
            InitSettingsData();
            Debug.Log("SettingsFileNotExist  InitSettingsData");
        }
        else//���������ļ�
        {
            string json = System.IO.File.ReadAllText(path);
            settingsData = JsonConvert.DeserializeObject<SettingsData>(json);
            //����������
            generalSettings.SetSettingsValue();
            volumeSettings.SetSettingsValue();
            screenSettings.SetSettingsValue();
        }
    }

    //��ʼ��ȡ�����������
    public void GetSettingsValue()
    {
        LoadButtonSettingFile();//���ذ��������ļ�
        LoadSettingsFile();
    }

    //����������
    public void SaveSettingsData()
    {
        string settingsData1 = JsonConvert.SerializeObject(settingsData, Formatting.Indented);
        string path = Path.Combine(Application.persistentDataPath, Constants.SETTINGS_FILE_NAME);
        System.IO.File.WriteAllText(path, settingsData1);
        SyncFiles();
    }
}
