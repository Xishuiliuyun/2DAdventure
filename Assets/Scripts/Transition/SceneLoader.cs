using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using System.IO;
using UnityEditor;
/*using UnityEditor.SearchService;*/
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    public bool playerIsDead;
    public Transform playerTransform;
    public Rigidbody2D rb;
    public PlayerStateBar playerStateBar;
    private float fadeDuration;
    public bool playerMove;

    public GameSceneSO menuScene;
    public GameSceneSO firstScene;
    public Vector3 menuPos;
    public Vector3 firstPos;

    public SceneLoadEventSO loadEvent;
    public VoidEvenSO afterSceneLoadedEvent;
    public FadeEventSO fadeEvent;
    public VoidEvenSO newGameEvent;

    private GameSceneSO sceneToLoad;
    private Vector3 postionToGo;
    private bool fadeScreen;
    private bool isLoading;

    public GameSceneSO currentLoadedScene;


    //处理House传送点的属性用
    private bool isHouseScene;
    private AttributeType houseAbilityType;
    private float value;
    private GameSceneSO sourceSceneSO;
    private Vector3 sourcePos;
    private string UID;

    //WEBGL运行环境参数
    public string RUNTIME_ENVIRONMENT_WEBGL;
#if UNITY_WEBGL && !UNITY_EDITOR//WEBGL网页环境
    [DllImport("__Internal")]
    private static extern int GetDeviceType();
#endif


    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(this.gameObject);

        playerIsDead = false;
        //Addressables.LoadSceneAsync(firstScene.sceneReference, LoadSceneMode.Additive);
        /*currentLoadedScene = firstScene;
        currentLoadedScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);*/
    }

    private void Start()
    {
        if(Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_ANDROID) BanLeftBtnAtt();

//已禁用，功能在InitalLoad中实现
/*#if UNITY_WEBGL && !UNITY_EDITOR//WEBGL网页环境
        Debug.Log(GetDeviceType());
        Debug.Log(RUNTIME_ENVIRONMENT_WEBGL);
        if(RUNTIME_ENVIRONMENT_WEBGL == null || RUNTIME_ENVIRONMENT_WEBGL == "")
        {
            if(GetDeviceType() == 0 || GetDeviceType() == 1)
            {
                RUNTIME_ENVIRONMENT_WEBGL = Constants.RUNTIME_ENVIRONMENT_WEBGL_MOBILE;
                BanLeftBtnAtt();
            }
            else
            {
                RUNTIME_ENVIRONMENT_WEBGL = Constants.RUNTIME_ENVIRONMENT_WEBGL_PC;
            }
        }
        
        Debug.Log(RUNTIME_ENVIRONMENT_WEBGL);
#endif*/
        //加载进入菜单场景
        loadEvent.RaiseLoadRequestEvent(menuScene, menuPos, true);
    }

    private void OnEnable()
    {
        loadEvent.LoadRequestEvent += OnLoadRequestEvent;
        newGameEvent.OnEventRised += NewGame;
    }

    private void OnDisable()
    {
        loadEvent.LoadRequestEvent -= OnLoadRequestEvent;
        newGameEvent.OnEventRised -= NewGame;
    }

    /*private void FixedUpdate()
    {
        Debug.Log(isLoading);
        if (isLoading)
        {
            rb.velocity = new Vector2(100 * Time.deltaTime, 0);
        }
    }*/

    

    public void NewGame()
    {
        Debug.Log("NewGame");
        //清除存档中的内容
        DataManager.instance.ClearData();
        //OnLoadRequestEvent(firstScene, firstPos, true);
        //初始化角色能力
        playerTransform.gameObject.GetComponent<Ability>().InitAbility();
        //写入Player数据
        DataManager.instance.SetPlayerData();
        //写入场景数据
        DataManager.instance.SetSceneData();
        loadEvent.RaiseLoadRequestEvent(firstScene, firstPos, true);
        /*if (sceneToLoad == firstScene)
        {
            Debug.Log("缓步移动至场景边界");
            playerTransform.gameObject.GetComponent<Rigidbody2D>().velocityX = 100;
        }*/
    }

    public void ContinueGame()
    {
        Debug.Log("ContinueGame");
        //替换saveData、collectionList中的数据？
        DataManager.instance.OverwriteData();
        //读取存档，确认场景
        string sceneName = DataManager.instance.GetSceneName();
        //加载需引用的场景资源
        string path = Constants.GAME_SCENE_SO_PATH + sceneName + ".asset";
        //LoadGameSceneSO(path);//LoadAssetAsync异步加载，当前不用此方式，且当前使用此方式未成功，不知什么地方有问题
        //此方式动态获取资源只可以在编辑器模式下使用(已弃用)
        //GameSceneSO gameSceneSO = AssetDatabase.LoadAssetAtPath<GameSceneSO>(path);
        Addressables.LoadAssetAsync<GameSceneSO>(sceneName).Completed += (handel) =>
        {
            if (handel.Status == AsyncOperationStatus.Succeeded)
            {
                //切换至对应场景
                loadEvent.RaiseLoadRequestEvent(handel.Result, DataManager.instance.GetPlayerPos(), true);
            }
            else { Debug.Log("LoadAssetAsyncERR");return; }
        };
        
    }

    //LoadAssetAsync异步加载，当前不用此方式
    public void LoadGameSceneSO(string path)
    {
        Addressables.LoadAssetAsync<GameSceneSO>("Forest2.asset").Completed += OnGameSceneSOAssetLoaded;
        var h = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Boar.prefab");
        var h2 = Addressables.LoadAssetAsync<GameObject>("Boar.prefab");
    }
    public void OnGameSceneSOAssetLoaded(AsyncOperationHandle<GameSceneSO> handle)
    {
        if(handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {//资源异步加载成功
            Debug.Log("场景资源异步加载成功");
            //sceneToLoad = handle.Result;
        }
        //切换至对应场景
        loadEvent.RaiseLoadRequestEvent(sceneToLoad, DataManager.instance.GetPlayerPos(), true);
    }

    /// <summary>
    /// 加载新场景
    /// </summary>
    /// <param name="locationToGo">新场景</param>
    /// <param name="posToGo">新场景Player坐标</param>
    /// <param name="fadeScreen">是否渐入渐出</param>
    private void OnLoadRequestEvent(GameSceneSO locationToGo, Vector3 posToGo, bool fadeScreen)
    {
        if (isLoading) return;
        isLoading = true;
        sceneToLoad = locationToGo;
        postionToGo = posToGo;
        this.fadeScreen = fadeScreen;
        playerTransform.gameObject.GetComponent<PlayerController>().inputControl.GamePlay.Disable();
        playerTransform.gameObject.GetComponent<PlayerController>().inputControl.UI.Pause.Disable();
        playerTransform.gameObject.GetComponent<PlayerController>().inputControl.UI.Back.Disable();
        playerStateBar.gameObject.SetActive(false);
        //需要根据条件设置一下存档的内容
        if (isHouseScene) 
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                UnityEngine.SceneManagement.Scene scene = SceneManager.GetSceneAt(i);
                string sceneName = scene.name;
                if(sceneName == "House1")
                {
                    //表示从房间传送出去
                    isHouseScene = false;
                    GameObject bookshelf = GameObject.Find("Bookshelf");
                    if(bookshelf!=null)
                    {
                        if (DataManager.instance.saveData.houseDataDict[UID].houseInteractableDataDict.ContainsKey(bookshelf.GetComponent<DataDefinition>().ID))
                        {
                            DataManager.instance.saveData.houseDataDict[UID].houseInteractableDataDict[bookshelf.GetComponent<DataDefinition>().ID] = bookshelf.GetComponent<Bookshelf>().isDone;
                        }
                        else
                        {
                            DataManager.instance.saveData.houseDataDict[UID].houseInteractableDataDict.Add(bookshelf.GetComponent<DataDefinition>().ID, bookshelf.GetComponent<Bookshelf>().isDone);
                        }
                    }
                    
                }
            }
            
        }
        //保存游戏  添加了通关游戏时，保存游戏数据
        if (currentLoadedScene != null && currentLoadedScene.sceneType != SceneType.Menu && !playerIsDead ||DataManager.instance.saveData.isGameOver == true) DataManager.instance.Save();
        Debug.Log("切换场景");
        if (currentLoadedScene != null)
        { StartCoroutine(UnLoadPreviousScene()); }
        else { LoadNewScene(); }
    }

    //卸载场景
    private IEnumerator UnLoadPreviousScene()
    {
        if (currentLoadedScene.sceneType == SceneType.Menu)
        {
            playerMove = true;
            fadeDuration = 5f;
        }
        if (fadeScreen)
        {
            //TODO:渐入
            fadeEvent.FadeIn(fadeDuration);
        }
        yield return new WaitForSeconds(fadeDuration);
        yield return currentLoadedScene.sceneReference.UnLoadScene();

        playerTransform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        playerTransform.Find("Sign").Find("SignSprite").gameObject.SetActive(false);
        LoadNewScene();
    }

    //加载新场景
    private void LoadNewScene()
    {
        playerMove = false;
        fadeDuration = 0.5f;
        var loadingOption = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive,true);
        if (sceneToLoad.sceneType == SceneType.Loaction) { playerStateBar.gameObject.SetActive(true); }
        loadingOption.Completed += OnLoadCompleted;
    }

    //场景加载完成后
    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> handle)
    {
        //如果是切换到Menu场景，在渐显前处理一下Camera
        //如果是切换到Menu场景，在渐显前读分辨率设置，处理一下Menu场景的标题和选项
        if (sceneToLoad.sceneType == SceneType.Menu)
        {
            CameraControl.instance.SceneCameraProcess(true);
            //Menu.instance.SetTitleAndBtn(DataManager.instance.settingsData.screensettingsData.resolvingPower);
        }
        else
        {
            CameraControl.instance.SceneCameraProcess(false);
        }
        //原场景切换后参数设置
        currentLoadedScene = sceneToLoad;

        //记录场景信息
        DataManager.instance.saveData.currentScene = SceneManager.GetSceneAt(1).name;
        //多个场景情况下设定当前激活的场景
        if (currentLoadedScene != null) SceneManager.SetActiveScene(SceneManager.GetSceneByName(DataManager.instance.GetSceneName()));

        //场景切换后数据加载
        //读取存档
        if(currentLoadedScene.sceneType != SceneType.Menu) DataManager.instance.Load();

        //如果是进入房间，需覆写一下房间可交互物体的状态
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            UnityEngine.SceneManagement.Scene scene = SceneManager.GetSceneAt(i);
            string sceneName = scene.name;
            if (sceneName == "House1")
            {
                GameObject bookshelf = GameObject.Find("Bookshelf");
                if (bookshelf != null)
                {
                    if (DataManager.instance.saveData.houseDataDict[UID].houseInteractableDataDict.Count != 0)
                    {
                        bookshelf.GetComponent<Bookshelf>().isDone = DataManager.instance.saveData.houseDataDict[UID].houseInteractableDataDict[bookshelf.GetComponent<DataDefinition>().ID];
                        if (bookshelf.GetComponent<Bookshelf>().isDone)
                        {
                            this.gameObject.tag = "UNInteractable";
                            bookshelf.GetComponent<BoxCollider2D>().enabled = false;
                            bookshelf.GetComponent<Bookshelf>().isDone = true;
                        }
                        else
                        {
                            bookshelf.tag = "Interactable";
                            bookshelf.GetComponent<BoxCollider2D>().enabled = true;
                            bookshelf.GetComponent<Bookshelf>().isDone = false;
                        }
                    }
                    else
                    {
                        bookshelf.tag = "Interactable";
                        bookshelf.GetComponent<BoxCollider2D>().enabled = true;
                        bookshelf.GetComponent<Bookshelf>().isDone = false;
                    }
                }
            }
        }

        //原场景切换后参数设置
        //currentLoadedScene = sceneToLoad;
        playerTransform.position = postionToGo;
        if (currentLoadedScene.sceneType == SceneType.Menu) //返回Menu场景时重置Player状态
        { 
            Debug.Log("ToMenu"); 
            playerTransform.localScale = new Vector3(1,1,1);
            BackToMenuInit();
        }

        //渐入渐出
        StartCoroutine(WaitToShow());
        if (fadeScreen)
        {
            //TODO:渐出
            fadeEvent.FadeOut(fadeDuration);
        }

        afterSceneLoadedEvent.RaiseEvent();//广播场景加载后的事件，当前仅获取摄像机边界下有此事件监听
        if (currentLoadedScene.sceneType != SceneType.Menu)//初始菜单界面不需要恢复Player移动
        {
            StartCoroutine(WaitFadeDurationTime());//携程控制渐出后才能控制角色行动
            //playerTransform.gameObject.GetComponent<PlayerController>().inputControl.GamePlay.Enable();
            UIManager.instance.SetSettingIconActive(true);//渐出前处理设置图标是否显示
        }
        else { UIManager.instance.SetSettingIconActive(false); }//渐出前处理设置图标是否显示;
        isLoading = false;

        //场景加载处理完成后，判断一下是否House传送点，进行部分处理设置
        if(isHouseScene)
        {
            //设置房间书架添加的能力
            GameObject bookshelf = GameObject.Find("Bookshelf");
            if (bookshelf != null)
            {
                bookshelf.GetComponent<Bookshelf>().bookshelfType = houseAbilityType;
                bookshelf.GetComponent<Bookshelf>().value = value;
            }
            //设置出门时传送回原场景
            GameObject teleport = GameObject.Find("Teleport");
            if (teleport != null)
            {
                teleport.GetComponent<TeleportPoint>().sceneToGo = sourceSceneSO;
                teleport.GetComponent<TeleportPoint>().positionToGo = sourcePos;
            }
            //传入房间的UID，读取当前房间内的所有可互动物体的互动情况
        }

        //更新场景及玩家信息
        if(!playerIsDead && currentLoadedScene.sceneType != SceneType.Menu) DataManager.instance.Save();
        playerIsDead = false;
    }

    private IEnumerator WaitFadeDurationTime()
    {
        yield return new WaitForSeconds(fadeDuration);
        playerTransform.gameObject.GetComponent<PlayerController>().inputControl.GamePlay.Enable();
        playerTransform.gameObject.GetComponent<PlayerController>().inputControl.UI.Pause.Enable();
    }

    //弃用gameObject.SetActive(true)方式解决Player切换场景时的角色有下落动作的问题
    //因为会调用Player上的awake、onenable等函数，可能会导致一些不可测问题
    //当前改为设置sprite的enable，用携程延后角色显示事件可解决切换场景时的角色有下落动作的问题
    private IEnumerator WaitToShow()
    {
        yield return new WaitForSeconds(0.1f);
        playerTransform.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        playerTransform.Find("Sign").Find("SignSprite").gameObject.SetActive(true);
    }

    //用于设置房间传送时设置场景内容的属性
    //abilityType房间添加的能力 vlu能力添加的值  sceneSO原场景 socPos
    public void SetHouseAbilityType(AttributeType abilityType,float vlu,GameSceneSO sceneSO,Vector3 socPos,string soUID)
    {
        isHouseScene = true;
        houseAbilityType = abilityType;
        value = vlu;
        sourceSceneSO = sceneSO;
        sourcePos = socPos;
        UID = soUID;
    }


    //返回主菜单
    public void BackToMenu()
    {
        //设置状态，用于场景切换时条件判断,只有在Player未死亡的状态下返回主菜单才保存
        playerIsDead = playerTransform.GetComponent<PlayerController>().isDead;
        //加载进入菜单场景
        loadEvent.RaiseLoadRequestEvent(menuScene, menuPos, true);
    }

    //返回主菜单时初始化项，在渐显前处理
    public void BackToMenuInit()
    {
        //初始化Player状态
        playerTransform.GetComponent<PlayerController>().PlayerRevive();
        //复活时应该恢复下面的操作权，但返回主菜单时不应恢复操作权
        playerTransform.GetComponent<PlayerController>().inputControl.GamePlay.Disable();
        playerTransform.GetComponent<PlayerController>().inputControl.UI.Pause.Disable();
        //隐藏GameOverPanel//PlayerRevive中已处理GameOverPanel
        //UIManager.instance.SetGaemOverPanel(false);
        //显示按键提示条(改为只在设置菜单才显示)
        //UIManager.instance.SettingTips.SetActive(true);
    }


    //禁用鼠标左键攻击(感觉禁不禁用关系不大，触屏不会触发戍边点击事件)
    public void BanLeftBtnAtt()
    {
        InputAction action = playerTransform.gameObject.GetComponent<PlayerController>().inputControl.FindAction("Attack");
        if (action != null)
        {
            action.Disable();
            action.ApplyBindingOverride(2, "");
            Debug.Log("禁用鼠标攻击");
            action.Enable();
        }
    }

}
