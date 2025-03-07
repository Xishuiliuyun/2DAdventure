using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
/*using UnityEditor.Rendering.LookDev;*/
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public static CameraControl instance;
    public GameObject mainCamera;
    public CinemachineVirtualCamera virtualCamera;
    public Transform screenManagerTran;
    public Transform playerTran;
    private CinemachineConfiner2D confiner2D;
    public CinemachineImpulseSource impulseSource;
    public VoidEvenSO cameraShakeEvent;
    public VoidEvenSO afterSceneLoadedEvent;
    //设置屏幕是否抖动用
    public BoolEvenSO isScreenShockEvenSO;
    public bool isScreenShock;

    //通过获取场景中的BackGround物体下的所有子物体进行场景视差实现
    //BackGround下的子物体从上到下排列，越靠近下方的物体每帧相对相机移动距离越远，最上层的Ksy和Mountain每帧相对相机移动距离较少
    [Header("实现视差用参数")]
    public float gapSpeed;//视差倍率，用于控制不同背景层速度相差的倍率,值越大速度相差越大
    public Transform cameraTransform;//玩家位置
    public GameObject backGround_Sky;//背景物体
    public GameObject backGround_Mountain;//背景物体
    public GameObject backGround_BackGroundTree;//背景物体
    public GameObject backGround_BackTree;//背景物体
    public GameObject backGround_FrontTree;//背景物体
    private Vector2 lastPos;//最后一次相机位置(上一帧位置)


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);


        confiner2D = GetComponent<CinemachineConfiner2D>();
        SetCameraView();
        StartCoroutine(GetShowDamageLater());
        //lastPos = playerTransform.position;
    }

    /*private void Start()
    {
        SetCameraView();
    }*/

    private void OnEnable()
    {
        cameraShakeEvent.OnEventRised += OnCameraShakeEvent;
        afterSceneLoadedEvent.OnEventRised += OnafterSceneLoadedEvent;
        isScreenShockEvenSO.OnEventRised += OnIsScreenShockEven;
    }

    private void OnDisable()
    {
        cameraShakeEvent.OnEventRised -= OnCameraShakeEvent;
        afterSceneLoadedEvent.OnEventRised -= OnafterSceneLoadedEvent;
        isScreenShockEvenSO.OnEventRised -= OnIsScreenShockEven;
    }

    private void Update()
    {
        if ((lastPos != null) && ((backGround_Sky != null) || (backGround_Mountain != null) || (backGround_BackGroundTree != null) || (backGround_BackTree != null) || (backGround_FrontTree != null)))
        {
            Vector2 amountToMove = new Vector2(cameraTransform.position.x - lastPos.x, cameraTransform.position.y - lastPos.y);
            backGround_Sky.transform.position += new Vector3(amountToMove.x*0.4f, 0, 0f);
            backGround_Mountain.transform.position += new Vector3(amountToMove.x * 0.3f, 0, 0f);
            backGround_BackGroundTree.transform.position += new Vector3(amountToMove.x * 0.2f, 0, 0f);
            //backGround_BackTree.transform.position += new Vector3(amountToMove.x * 0.1f, 0, 0f);
            //backGround_FrontTree.transform.position += new Vector3(amountToMove.x * 0.1f, 0, 0f);
            lastPos = cameraTransform.position;
        }
    }


    private void OnafterSceneLoadedEvent()
    {
        GetNewCameraBounds();
        SetCameraView();
    }

    /*private void Start()
    {
        GetNewCameraBounds();
    }*/

    //初始化切换场景后实现场景视差所需的参数
    public void SetCameraView()
    {
        lastPos = cameraTransform.position;//初始位置
        backGround_Sky = GameObject.FindGameObjectWithTag("BackGround_Sky");
        backGround_Mountain = GameObject.FindGameObjectWithTag("BackGround_Mountain");
        backGround_BackGroundTree = GameObject.FindGameObjectWithTag("BackGround_BackGroundTree");
        backGround_BackTree = GameObject.FindGameObjectWithTag("BackGround_BackTree");
        backGround_FrontTree = GameObject.FindGameObjectWithTag("BackGround_FrontTree");
    }


    public void GetNewCameraBounds()
    {
        var obj = GameObject.FindGameObjectWithTag("Bounds");
        if (obj == null)
            return;
        Debug.Log(obj.name);
        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();
        confiner2D.InvalidateCache();

    }

    public void OnIsScreenShockEven(bool falg)
    {
        isScreenShock = falg;
    }

    void OnCameraShakeEvent()
    {
        if (!isScreenShock) return;
        impulseSource.GenerateImpulse();
    }

    //延迟获取当前设置值，避免初始化时空引用
    IEnumerator GetShowDamageLater()
    {
        yield return new WaitForSeconds(0.1f);
        isScreenShock = DataManager.instance.settingsData.generalSettingsData.isScreenShock;
    }

    //相机处理后处理，设置相机跟随目标和pos  isMenu用于确认是否切换至Menu场景，Menu场景相机锚定在screenManagerTran节点上
    public void SceneCameraProcess(bool isMenu)
    {
        if(isMenu)
        {
            CinemachineFramingTransposer framingTransposer;
            virtualCamera.LookAt = screenManagerTran;
            virtualCamera.Follow = screenManagerTran;
            //设置死区和软区参数
            framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (framingTransposer == null) { Debug.Log("transposerIsNullERR"); }
            else
            {
                framingTransposer.m_DeadZoneWidth = 0;
                framingTransposer.m_DeadZoneHeight = 0;
                framingTransposer.m_SoftZoneWidth = 0;
                framingTransposer.m_SoftZoneHeight = 0;
            }
            transform.position = screenManagerTran.transform.position;
            mainCamera.transform.position = screenManagerTran.transform.position;

        }
        else
        {
            CinemachineFramingTransposer framingTransposer;
            virtualCamera.LookAt = playerTran;
            virtualCamera.Follow = playerTran;
            //设置死区和软区参数
            framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (framingTransposer == null) { Debug.Log("transposerIsNullERR"); }
            else
            {
                framingTransposer.m_DeadZoneWidth = Constants.DEAD_ZONE_WIDTH;
                framingTransposer.m_DeadZoneHeight = Constants.DEAD_ZONE_HEIGHT;
                framingTransposer.m_SoftZoneWidth = Constants.SOFT_ZONE_WIDTH;
                framingTransposer.m_SoftZoneHeight = Constants.SOFT_ZONE_HEIGHT;
            }
            transform.position = playerTran.transform.position;
            mainCamera.transform.position = playerTran.transform.position;
        }
    }



}
