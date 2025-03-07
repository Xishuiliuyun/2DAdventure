using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//模板房间传送及设置，及数据保存等
//后续新方将以新场景的方式进行设置，此种模板式设置感觉耦合性太高了，而且不方便使用

public class TeleportPoint_House : MonoBehaviour, IInteractable,ISaveable
{
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO sceneToGo;
    public Vector3 positionToGo;

    [Header("房间传送点属性")]
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


    //用于显示交互按钮，交互后再进行传送的类型，Tag要设置成Interactable
    public void TriggerAction()
    {
        if(doorIsOpen)
        {
            isDone = true;
            //估计需要把UID也传到SceneLoader
            //设置房间能力添加属性
            SceneLoader.instance.SetHouseAbilityType(houseType, value, sourceSceneSO, transform.position, GetComponent<DataDefinition>().ID);
            //加载场景
            loadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
        }
        else
        {
            //添加房屋进入时需要消耗钥匙的功能
            if (playerAbility.keyNumber > 0)
            {
                Debug.Log("teleport");
                playerAbility.keyNumber--;
                isDone = true;
                if (!UIManager.instance.ChangeKeyNumber(playerAbility.keyNumber)) Debug.Log("KEY Number ERR");
                //估计需要把UID也传到SceneLoader
                //设置房间能力添加属性
                SceneLoader.instance.SetHouseAbilityType(houseType, value, sourceSceneSO, transform.position, GetComponent<DataDefinition>().ID);
                //加载场景
                loadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
            }
            else
            {
                UIManager.instance.ShowMessage("需要钥匙");
                //Debug.Log("key is not enough");
            }
        }
    }

    //用于不显示交互按钮，进入该区域即进行传送的类型，Tag要设置成Teleport
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
