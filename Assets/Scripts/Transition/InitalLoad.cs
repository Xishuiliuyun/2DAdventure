using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

//�˽ű�������Ϸ��ʱ��ʼ��ȡ��������Ϸ����������ݣ����ñ��ʱ�߼��������ط�����
public class InitalLoad : MonoBehaviour
{
    public AssetReference persistentScene;
    public AssetReference enemyAsset;
    public GameSceneSO persistentSceneSO;
    public GameObject canvas;
    public TextMeshProUGUI progressText;

    //���������JS�ű��д�����������
    private bool funActive = false;//SendMessage�Ƿ��н��в�������
    private bool iIsTouchDevice;//��ǰ�豸�Ƿ�ɴ���(ͨ���÷�ʽ�ж��豸�Ƿ��ƶ���)

    //��Դ���ؽ����ò���
    private float progressScene;
    private float progressEnemy;


    private void Awake()
    {
        SettingsData settingsData;
        settingsData = new SettingsData();
        settingsData.generalSettingsData = new GeneralSettingsData();
        settingsData.volumeSettingsData = new VolumeSettingsData();
        settingsData.screensettingsData = new ScreenSettingsData();

        if(Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_WEBGL)
        {
            canvas.SetActive(true);
            //WEBGL��Ŀ����
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(1240, 768, true);
        }
        else
        {
            canvas.SetActive(false);
            string path = Path.Combine(Application.persistentDataPath, Constants.SETTINGS_FILE_NAME);
            if (!File.Exists(path)) //�����������ļ�
            {//�����������ļ�(��һ�δ���Ϸ)
             //Ĭ������1920*1080 ȫ��ģʽ����
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Screen.SetResolution(1920, 1080, true);
            }
            else//���������ļ�
            {
                string json = System.IO.File.ReadAllText(path);
                settingsData = JsonConvert.DeserializeObject<SettingsData>(json);
                if (settingsData.screensettingsData.fullScreen)
                {
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                }
                else
                {
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                }
                SetResolvingPower(settingsData.screensettingsData.resolvingPower, settingsData.screensettingsData.fullScreen);
            }
        }

        Debug.Log("BeforeLoadScenepersistentScene");
        if (persistentScene != null)
        {
            if(Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_WEBGL)
            {
                //��ǰ����enemy��Դ�ļ����������instanceʵ����enemyʱҪ�����ļ����µ��ӳ�����
                var handle1 = Addressables.LoadAssetAsync<GameObject>("Boar");
                StartCoroutine(MonitorProgress2(handle1));
                handle1.Completed += (handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Debug.Log("Dependencies downloaded successfully.");
                    }
                    else
                    {
                        Debug.LogError("Failed to download dependencies: " + handle.OperationException);
                    }
                };
            }
            var handle2 = Addressables.LoadSceneAsync(persistentScene);
            if (Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_WEBGL) StartCoroutine(MonitorProgress(handle2));
            handle2.Completed += OnInitalLoadComplet;
        }
        else
        {
            if (Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_WEBGL)
            {
                //��ǰ����enemy��Դ�ļ����������instanceʵ����enemyʱҪ�����ļ����µ��ӳ�����
                var handle1 = Addressables.LoadAssetAsync<GameObject>("Boar");
                StartCoroutine(MonitorProgress2(handle1));
                handle1.Completed += (handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Debug.Log("Dependencies downloaded successfully.");
                    }
                    else
                    {
                        Debug.LogError("Failed to download dependencies: " + handle.OperationException);
                    }
                };
            }
            var handle2 = Addressables.LoadSceneAsync(persistentSceneSO.sceneReference);
            if (Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_WEBGL) StartCoroutine(MonitorProgress(handle2));
            handle2.Completed += OnInitalLoadComplet;
        }


    }

    private void Start()
    {
        if (Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_WEBGL)
        {
            // ���� JavaScript ����
            Application.ExternalEval(@"
            const isTouch = 'ontouchstart' in window || navigator.maxTouchPoints > 0;
            const intValue = isTouch ? 1 : 0; // �� boolean ת��Ϊ int
            SendMessage('" + gameObject.name + @"', 'OnTouchInfoReceived', intValue);
        ");
        }
    }

    private void Update()
    {
        if(Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_WEBGL)
        {
            float progress = progressScene * 0.7f + progressEnemy * 0.3f;
            progressText.text = (progress * 100).ToString("F1") + "%";
        }
    }

    public void OnInitalLoadComplet(AsyncOperationHandle<SceneInstance> handle)
    {
        Debug.Log("OnInitalLoadComplet");
        //Debug.Log(SceneLoader.instance.RUNTIME_ENVIRONMENT_WEBGL);
        if(funActive)
        {
            if(SceneLoader.instance != null)
            {
                if (iIsTouchDevice)
                {
                    SceneLoader.instance.BanLeftBtnAtt();
                    SceneLoader.instance.RUNTIME_ENVIRONMENT_WEBGL = Constants.RUNTIME_ENVIRONMENT_WEBGL_MOBILE;
                }
                else SceneLoader.instance.RUNTIME_ENVIRONMENT_WEBGL = Constants.RUNTIME_ENVIRONMENT_WEBGL_PC;
            }
        }
    }

    //ʵ�ֽ��������JS�ű��д���������
    public void OnTouchInfoReceived(int isTouchDevice)
    {
        Debug.Log("OnTouchInfoReceived_Active" + "    isTouchDevice:" + isTouchDevice);
        funActive = true;
        iIsTouchDevice = isTouchDevice == 1;
    }

    public void SetResolvingPower(ResolvingPower resolvingPower,bool isFullScreen)
    {
        switch (resolvingPower)
        {
            case ResolvingPower.Type1:
                Screen.SetResolution(1920, 1080, isFullScreen);
                break;
            case ResolvingPower.Type2:
                Screen.SetResolution(1280, 720, isFullScreen);
                break;
            case ResolvingPower.Type3:
                Screen.SetResolution(1024, 768, isFullScreen);
                break;
            case ResolvingPower.Type4:
                Screen.SetResolution(800, 600, isFullScreen);
                break;
            case ResolvingPower.Type5:
                Screen.SetResolution(1440, 900, isFullScreen);
                break;
            case ResolvingPower.Type6:
                Screen.SetResolution(1920, 810, isFullScreen);
                break;
            case ResolvingPower.Type7:
                break;
            case ResolvingPower.Type8:
                break;
            case ResolvingPower.Type9:
                break;
            case ResolvingPower.Type10:
                break;
        }
    }

    IEnumerator MonitorProgress(AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance> handle)
    {
        while (!handle.IsDone)
        {
            float progress = handle.PercentComplete; // ��ȡ���ؽ��ȣ�0��1��
            progressScene = progress;
            //progressText.text = (progress*100).ToString("F1") + "%";
            //Debug.Log("Loading progress: " + (progress * 100) + "%");
            yield return null;
        }

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //GameObject asset = handle.Result;
            //Instantiate(asset);
            Debug.LogError("succeess to load asset: " + handle.OperationException);
        }
        else
        {
            Debug.LogError("Failed to load asset: " + handle.OperationException);
        }
    }

    IEnumerator MonitorProgress2(AsyncOperationHandle<UnityEngine.GameObject> handle)
    {
        while (!handle.IsDone)
        {
            float progress = handle.PercentComplete; // ��ȡ���ؽ��ȣ�0��1��
            progressEnemy = progress;
            //progressText.text = (progress * 100).ToString("F1") + "%";
            //Debug.Log("Loading progress: " + (progress * 100) + "%");
            yield return null;
        }

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //GameObject asset = handle.Result;
            //Instantiate(asset);
            Debug.LogError("succeess to load asset: " + handle.OperationException);
        }
        else
        {
            Debug.LogError("Failed to load asset: " + handle.OperationException);
        }
    }
}
