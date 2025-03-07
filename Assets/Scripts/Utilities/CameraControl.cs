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
    //������Ļ�Ƿ񶶶���
    public BoolEvenSO isScreenShockEvenSO;
    public bool isScreenShock;

    //ͨ����ȡ�����е�BackGround�����µ�������������г����Ӳ�ʵ��
    //BackGround�µ���������ϵ������У�Խ�����·�������ÿ֡�������ƶ�����ԽԶ�����ϲ��Ksy��Mountainÿ֡�������ƶ��������
    [Header("ʵ���Ӳ��ò���")]
    public float gapSpeed;//�Ӳ�ʣ����ڿ��Ʋ�ͬ�������ٶ����ı���,ֵԽ���ٶ����Խ��
    public Transform cameraTransform;//���λ��
    public GameObject backGround_Sky;//��������
    public GameObject backGround_Mountain;//��������
    public GameObject backGround_BackGroundTree;//��������
    public GameObject backGround_BackTree;//��������
    public GameObject backGround_FrontTree;//��������
    private Vector2 lastPos;//���һ�����λ��(��һ֡λ��)


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

    //��ʼ���л�������ʵ�ֳ����Ӳ�����Ĳ���
    public void SetCameraView()
    {
        lastPos = cameraTransform.position;//��ʼλ��
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

    //�ӳٻ�ȡ��ǰ����ֵ�������ʼ��ʱ������
    IEnumerator GetShowDamageLater()
    {
        yield return new WaitForSeconds(0.1f);
        isScreenShock = DataManager.instance.settingsData.generalSettingsData.isScreenShock;
    }

    //���������������������Ŀ���pos  isMenu����ȷ���Ƿ��л���Menu������Menu�������ê����screenManagerTran�ڵ���
    public void SceneCameraProcess(bool isMenu)
    {
        if(isMenu)
        {
            CinemachineFramingTransposer framingTransposer;
            virtualCamera.LookAt = screenManagerTran;
            virtualCamera.Follow = screenManagerTran;
            //������������������
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
            //������������������
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
