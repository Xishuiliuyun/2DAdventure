using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//ģ�巿�䴫�ͼ����ã������ݱ����
//�����·������³����ķ�ʽ�������ã�����ģ��ʽ���øо������̫���ˣ����Ҳ�����ʹ��

public class TeleportPoint_House : MonoBehaviour, IInteractable,ISaveable
{
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO sceneToGo;
    public Vector3 positionToGo;

    [Header("���䴫�͵�����")]
    private Ability playerAbility;
    public AttributeType houseType;
    public float value;
    public GameSceneSO sourceSceneSO;

    public bool isDone;
    public bool doorIsOpen;
    public HouseData houseData;
    private void Awake()
    {
        houseData = new HouseData();
    }


    private void OnEnable()
    {
        doorIsOpen = isDone;
        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }


    //������ʾ������ť���������ٽ��д��͵����ͣ�TagҪ���ó�Interactable
    public void TriggerAction()
    {
        if(doorIsOpen)
        {
            isDone = true;
            //������Ҫ��UIDҲ����SceneLoader
            //���÷��������������
            SceneLoader.instance.SetHouseAbilityType(houseType, value, sourceSceneSO, transform.position, GetComponent<DataDefinition>().ID);
            //���س���
            loadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
        }
        else
        {
            //��ӷ��ݽ���ʱ��Ҫ����Կ�׵Ĺ���
            if (playerAbility.keyNumber > 0)
            {
                Debug.Log("teleport");
                playerAbility.keyNumber--;
                isDone = true;
                if (!UIManager.instance.ChangeKeyNumber(playerAbility.keyNumber)) Debug.Log("KEY Number ERR");
                //������Ҫ��UIDҲ����SceneLoader
                //���÷��������������
                SceneLoader.instance.SetHouseAbilityType(houseType, value, sourceSceneSO, transform.position, GetComponent<DataDefinition>().ID);
                //���س���
                loadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
            }
            else
            {
                UIManager.instance.ShowMessage("��ҪԿ��");
                //Debug.Log("key is not enough");
            }
        }
    }

    //���ڲ���ʾ������ť����������򼴽��д��͵����ͣ�TagҪ���ó�Teleport
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            playerAbility = collision.GetComponent<Ability>();
        }
    }

    public DataDefinition GetDataID()
    {
        return GetComponent<DataDefinition>();
    }

    public void SetHouseUNInteractable()
    {
        isDone = true;
        doorIsOpen = true;
    }
    public void SetHouseInteractable()
    {
        isDone = false;
        doorIsOpen = false;
    }


    public void GetSaveData(Data data)
    {
        houseData.isDone = isDone;
        if(data.houseDataDict.ContainsKey(GetDataID().ID)) houseData.houseInteractableDataDict = data.houseDataDict[GetDataID().ID].houseInteractableDataDict;
        if (data.houseDataDict.ContainsKey(GetDataID().ID))
        {
            data.houseDataDict[GetDataID().ID] = houseData;
        }
        else data.houseDataDict.Add(GetDataID().ID, houseData);
    }

    public void LoadData(Data data)
    {
        if (data.houseDataDict.ContainsKey(GetDataID().ID))
        {
            if (data.houseDataDict[GetDataID().ID].isDone)
            {
                SetHouseUNInteractable();
            }
            else
            {
                SetHouseInteractable();
            }
        }
    }
}
