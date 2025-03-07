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


    //����House���͵��������
    private bool isHouseScene;
    private AttributeType houseAbilityType;
    private float value;
    private GameSceneSO sourceSceneSO;
    private Vector3 sourcePos;
    private string UID;

    //WEBGL���л�������
    public string RUNTIME_ENVIRONMENT_WEBGL;
#if UNITY_WEBGL && !UNITY_EDITOR//WEBGL��ҳ����
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

//�ѽ��ã�������InitalLoad��ʵ��
/*#if UNITY_WEBGL && !UNITY_EDITOR//WEBGL��ҳ����
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
        //���ؽ���˵�����
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
        //����浵�е�����
        DataManager.instance.ClearData();
        //OnLoadRequestEvent(firstScene, firstPos, true);
        //��ʼ����ɫ����
        playerTransform.gameObject.GetComponent<Ability>().InitAbility();
        //д��Player����
        DataManager.instance.SetPlayerData();
        //д�볡������
        DataManager.instance.SetSceneData();
        loadEvent.RaiseLoadRequestEvent(firstScene, firstPos, true);
        /*if (sceneToLoad == firstScene)
        {
            Debug.Log("�����ƶ��������߽�");
            playerTransform.gameObject.GetComponent<Rigidbody2D>().velocityX = 100;
        }*/
    }

    public void ContinueGame()
    {
        Debug.Log("ContinueGame");
        //�滻saveData��collectionList�е����ݣ�
        DataManager.instance.OverwriteData();
        //��ȡ�浵��ȷ�ϳ���
        string sceneName = DataManager.instance.GetSceneName();
        //���������õĳ�����Դ
        string path = Constants.GAME_SCENE_SO_PATH + sceneName + ".asset";
        //LoadGameSceneSO(path);//LoadAssetAsync�첽���أ���ǰ���ô˷�ʽ���ҵ�ǰʹ�ô˷�ʽδ�ɹ�����֪ʲô�ط�������
        //�˷�ʽ��̬��ȡ��Դֻ�����ڱ༭��ģʽ��ʹ��(������)
        //GameSceneSO gameSceneSO = AssetDatabase.LoadAssetAtPath<GameSceneSO>(path);
        Addressables.LoadAssetAsync<GameSceneSO>(sceneName).Completed += (handel) =>
        {
            if (handel.Status == AsyncOperationStatus.Succeeded)
            {
                //�л�����Ӧ����
                loadEvent.RaiseLoadRequestEvent(handel.Result, DataManager.instance.GetPlayerPos(), true);
            }
            else { Debug.Log("LoadAssetAsyncERR");return; }
        };
        
    }

    //LoadAssetAsync�첽���أ���ǰ���ô˷�ʽ
    public void LoadGameSceneSO(string path)
    {
        Addressables.LoadAssetAsync<GameSceneSO>("Forest2.asset").Completed += OnGameSceneSOAssetLoaded;
        var h = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Boar.prefab");
        var h2 = Addressables.LoadAssetAsync<GameObject>("Boar.prefab");
    }
    public void OnGameSceneSOAssetLoaded(AsyncOperationHandle<GameSceneSO> handle)
    {
        if(handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {//��Դ�첽���سɹ�
            Debug.Log("������Դ�첽���سɹ�");
            //sceneToLoad = handle.Result;
        }
        //�л�����Ӧ����
        loadEvent.RaiseLoadRequestEvent(sceneToLoad, DataManager.instance.GetPlayerPos(), true);
    }

    /// <summary>
    /// �����³���
    /// </summary>
    /// <param name="locationToGo">�³���</param>
    /// <param name="posToGo">�³���Player����</param>
    /// <param name="fadeScreen">�Ƿ��뽥��</param>
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
        //��Ҫ������������һ�´浵������
        if (isHouseScene) 
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                UnityEngine.SceneManagement.Scene scene = SceneManager.GetSceneAt(i);
                string sceneName = scene.name;
                if(sceneName == "House1")
                {
                    //��ʾ�ӷ��䴫�ͳ�ȥ
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
        //������Ϸ  �����ͨ����Ϸʱ��������Ϸ����
        if (currentLoadedScene != null && currentLoadedScene.sceneType != SceneType.Menu && !playerIsDead ||DataManager.instance.saveData.isGameOver == true) DataManager.instance.Save();
        Debug.Log("�л�����");
        if (currentLoadedScene != null)
        { StartCoroutine(UnLoadPreviousScene()); }
        else { LoadNewScene(); }
    }

    //ж�س���
    private IEnumerator UnLoadPreviousScene()
    {
        if (currentLoadedScene.sceneType == SceneType.Menu)
        {
            playerMove = true;
            fadeDuration = 5f;
        }
        if (fadeScreen)
        {
            //TODO:����
            fadeEvent.FadeIn(fadeDuration);
        }
        yield return new WaitForSeconds(fadeDuration);
        yield return currentLoadedScene.sceneReference.UnLoadScene();

        playerTransform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        playerTransform.Find("Sign").Find("SignSprite").gameObject.SetActive(false);
        LoadNewScene();
    }

    //�����³���
    private void LoadNewScene()
    {
        playerMove = false;
        fadeDuration = 0.5f;
        var loadingOption = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive,true);
        if (sceneToLoad.sceneType == SceneType.Loaction) { playerStateBar.gameObject.SetActive(true); }
        loadingOption.Completed += OnLoadCompleted;
    }

    //����������ɺ�
    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> handle)
    {
        //������л���Menu�������ڽ���ǰ����һ��Camera
        //������л���Menu�������ڽ���ǰ���ֱ������ã�����һ��Menu�����ı����ѡ��
        if (sceneToLoad.sceneType == SceneType.Menu)
        {
            CameraControl.instance.SceneCameraProcess(true);
            //Menu.instance.SetTitleAndBtn(DataManager.instance.settingsData.screensettingsData.resolvingPower);
        }
        else
        {
            CameraControl.instance.SceneCameraProcess(false);
        }
        //ԭ�����л����������
        currentLoadedScene = sceneToLoad;

        //��¼������Ϣ
        DataManager.instance.saveData.currentScene = SceneManager.GetSceneAt(1).name;
        //�������������趨��ǰ����ĳ���
        if (currentLoadedScene != null) SceneManager.SetActiveScene(SceneManager.GetSceneByName(DataManager.instance.GetSceneName()));

        //�����л������ݼ���
        //��ȡ�浵
        if(currentLoadedScene.sceneType != SceneType.Menu) DataManager.instance.Load();

        //����ǽ��뷿�䣬�踲дһ�·���ɽ��������״̬
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

        //ԭ�����л����������
        //currentLoadedScene = sceneToLoad;
        playerTransform.position = postionToGo;
        if (currentLoadedScene.sceneType == SceneType.Menu) //����Menu����ʱ����Player״̬
        { 
            Debug.Log("ToMenu"); 
            playerTransform.localScale = new Vector3(1,1,1);
            BackToMenuInit();
        }

        //���뽥��
        StartCoroutine(WaitToShow());
        if (fadeScreen)
        {
            //TODO:����
            fadeEvent.FadeOut(fadeDuration);
        }

        afterSceneLoadedEvent.RaiseEvent();//�㲥�������غ���¼�����ǰ����ȡ������߽����д��¼�����
        if (currentLoadedScene.sceneType != SceneType.Menu)//��ʼ�˵����治��Ҫ�ָ�Player�ƶ�
        {
            StartCoroutine(WaitFadeDurationTime());//Я�̿��ƽ�������ܿ��ƽ�ɫ�ж�
            //playerTransform.gameObject.GetComponent<PlayerController>().inputControl.GamePlay.Enable();
            UIManager.instance.SetSettingIconActive(true);//����ǰ��������ͼ���Ƿ���ʾ
        }
        else { UIManager.instance.SetSettingIconActive(false); }//����ǰ��������ͼ���Ƿ���ʾ;
        isLoading = false;

        //�������ش�����ɺ��ж�һ���Ƿ�House���͵㣬���в��ִ�������
        if(isHouseScene)
        {
            //���÷��������ӵ�����
            GameObject bookshelf = GameObject.Find("Bookshelf");
            if (bookshelf != null)
            {
                bookshelf.GetComponent<Bookshelf>().bookshelfType = houseAbilityType;
                bookshelf.GetComponent<Bookshelf>().value = value;
            }
            //���ó���ʱ���ͻ�ԭ����
            GameObject teleport = GameObject.Find("Teleport");
            if (teleport != null)
            {
                teleport.GetComponent<TeleportPoint>().sceneToGo = sourceSceneSO;
                teleport.GetComponent<TeleportPoint>().positionToGo = sourcePos;
            }
            //���뷿���UID����ȡ��ǰ�����ڵ����пɻ�������Ļ������
        }

        //���³����������Ϣ
        if(!playerIsDead && currentLoadedScene.sceneType != SceneType.Menu) DataManager.instance.Save();
        playerIsDead = false;
    }

    private IEnumerator WaitFadeDurationTime()
    {
        yield return new WaitForSeconds(fadeDuration);
        playerTransform.gameObject.GetComponent<PlayerController>().inputControl.GamePlay.Enable();
        playerTransform.gameObject.GetComponent<PlayerController>().inputControl.UI.Pause.Enable();
    }

    //����gameObject.SetActive(true)��ʽ���Player�л�����ʱ�Ľ�ɫ�����䶯��������
    //��Ϊ�����Player�ϵ�awake��onenable�Ⱥ��������ܻᵼ��һЩ���ɲ�����
    //��ǰ��Ϊ����sprite��enable����Я���Ӻ��ɫ��ʾ�¼��ɽ���л�����ʱ�Ľ�ɫ�����䶯��������
    private IEnumerator WaitToShow()
    {
        yield return new WaitForSeconds(0.1f);
        playerTransform.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        playerTransform.Find("Sign").Find("SignSprite").gameObject.SetActive(true);
    }

    //�������÷��䴫��ʱ���ó������ݵ�����
    //abilityType������ӵ����� vlu������ӵ�ֵ  sceneSOԭ���� socPos
    public void SetHouseAbilityType(AttributeType abilityType,float vlu,GameSceneSO sceneSO,Vector3 socPos,string soUID)
    {
        isHouseScene = true;
        houseAbilityType = abilityType;
        value = vlu;
        sourceSceneSO = sceneSO;
        sourcePos = socPos;
        UID = soUID;
    }


    //�������˵�
    public void BackToMenu()
    {
        //����״̬�����ڳ����л�ʱ�����ж�,ֻ����Playerδ������״̬�·������˵��ű���
        playerIsDead = playerTransform.GetComponent<PlayerController>().isDead;
        //���ؽ���˵�����
        loadEvent.RaiseLoadRequestEvent(menuScene, menuPos, true);
    }

    //�������˵�ʱ��ʼ����ڽ���ǰ����
    public void BackToMenuInit()
    {
        //��ʼ��Player״̬
        playerTransform.GetComponent<PlayerController>().PlayerRevive();
        //����ʱӦ�ûָ�����Ĳ���Ȩ�����������˵�ʱ��Ӧ�ָ�����Ȩ
        playerTransform.GetComponent<PlayerController>().inputControl.GamePlay.Disable();
        playerTransform.GetComponent<PlayerController>().inputControl.UI.Pause.Disable();
        //����GameOverPanel//PlayerRevive���Ѵ���GameOverPanel
        //UIManager.instance.SetGaemOverPanel(false);
        //��ʾ������ʾ��(��Ϊֻ�����ò˵�����ʾ)
        //UIManager.instance.SettingTips.SetActive(true);
    }


    //��������������(�о��������ù�ϵ���󣬴������ᴥ�����ߵ���¼�)
    public void BanLeftBtnAtt()
    {
        InputAction action = playerTransform.gameObject.GetComponent<PlayerController>().inputControl.FindAction("Attack");
        if (action != null)
        {
            action.Disable();
            action.ApplyBindingOverride(2, "");
            Debug.Log("������깥��");
            action.Enable();
        }
    }

}
