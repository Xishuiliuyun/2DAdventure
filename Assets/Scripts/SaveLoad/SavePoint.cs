using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour,IInteractable,ISaveable
{
    public Sprite savePointDark;
    public Sprite savePointLight;
    public SpriteRenderer savePointSprite;
    public GameObject lightObj;
    public bool isDone;
    public VoidEvenSO saveDataEvent;

    private void OnEnable()
    {
        //需考虑保存逻辑后再决定保存点的逻辑
        if(isDone) { SetSavePointUNInteractable(); }
        else { SetSavePointInteractable(); }
        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    public void TriggerAction()
    {
        Debug.Log("SavePoint");
        SetSavePointUNInteractable();
        saveDataEvent.RaiseEvent();
        //显示提示信息
        UIManager.instance.ShowMessage("存档完成");
    }

    //当前设置可重复存档，仅设置灯光和贴图不一致即可
    public void SetSavePointUNInteractable()
    {
        savePointSprite.sprite = savePointLight;
        //this.gameObject.tag = "UNInteractable";
        //GetComponent<BoxCollider2D>().enabled = false;
        lightObj.SetActive(true);
        isDone = true;
    }

    public void SetSavePointInteractable()
    {
        savePointSprite.sprite = savePointDark;
        this.gameObject.tag = "SavePoint";
        GetComponent<BoxCollider2D>().enabled = true;
        lightObj.SetActive(false);
        isDone = false;
    }

    public DataDefinition GetDataID()
    {
        return GetComponent<DataDefinition>();
    }

    public void GetSaveData(Data data)
    {
        if (data.interactableDataDict.ContainsKey(GetDataID().ID))
        {
            data.interactableDataDict[GetDataID().ID] = isDone;
        }
        else data.interactableDataDict.Add(GetDataID().ID, isDone);
    }

    public void LoadData(Data data)
    {
        if (data.interactableDataDict.ContainsKey(GetDataID().ID))
        {
            if (data.interactableDataDict[GetDataID().ID])
            {
                SetSavePointUNInteractable();
            }
            else
            {
                SetSavePointInteractable();
            }
        }
    }
}
