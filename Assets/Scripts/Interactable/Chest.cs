using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.SceneManagement;

public class Chest : MonoBehaviour,IInteractable,ISaveable
{
    public Sprite openChestSprite1_1;
    public Sprite openChestSprite1_2;
    public Sprite openChestSprite2_1;
    public Sprite openChestSprite2_2;
    public Sprite closeChestSprite1_1;
    public Sprite closeChestSprite1_2;
    public Sprite closeChestSprite2_1;
    public Sprite closeChestSprite2_2;
    private GameObject objChest1_1;
    private GameObject objChest1_2;
    private GameObject objChest2_1;
    private GameObject objChest2_2;
    public bool isDone;

    public GameObject objInChest_PrefabSO;
    private GameObject objInChest;

    private void Awake()
    {
        if (transform.Find("Chest1_1") != null) { objChest1_1 = transform.Find("Chest1_1").gameObject; }
        if (transform.Find("Chest1_2") != null) { objChest1_2 = transform.Find("Chest1_2").gameObject; }
        if (transform.Find("Chest2_1") != null) { objChest2_1 = transform.Find("Chest2_1").gameObject; }
        if (transform.Find("Chest2_2") != null) { objChest2_2 = transform.Find("Chest2_2").gameObject; }
    }

    private void OnEnable()
    {
        if (isDone) { SetChestOpen(); }
        else { SetChestClose(); }

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
        Debug.Log("open chest");
        if (!isDone) 
        { 
            OpenChest();
            GetComponent<AudioDefination>()?.PlayAudioClip();
        }
    }

    public void OpenChest()
    {
        SetChestOpenSprite();
        isDone = true;
        this.gameObject.tag = "UNInteractable";
        GetComponent<BoxCollider2D>().enabled = false;
        //œ‘ æ±¶œ‰ƒ⁄»›ŒÔ
        objInChest = Instantiate(objInChest_PrefabSO, transform.position, Quaternion.Euler(0, 0, 0));
        /*var script = objInChest.GetComponent<PrefabSet>();
        DataManager.instance.AddGameObjectToList(script.prefabData);*/
    }

    public void SetChestOpen()
    {
        SetChestOpenSprite();
        isDone = true;
        this.gameObject.tag = "UNInteractable";
        GetComponent<BoxCollider2D>().enabled = false;
    }

    public void SetChestClose()
    {
        SetChestCloseSprite();
        isDone = false;
        this.gameObject.tag = "Interactable";
        GetComponent<BoxCollider2D>().enabled = true;
    }

    private void SetChestOpenSprite()
    {
        objChest1_1.GetComponent<SpriteRenderer>().sprite = openChestSprite1_1;
        objChest1_2.GetComponent<SpriteRenderer>().sprite = openChestSprite1_2;
        objChest2_1.GetComponent<SpriteRenderer>().sprite = openChestSprite2_1;
        objChest2_2.GetComponent<SpriteRenderer>().sprite = openChestSprite2_2;
    }

    private void SetChestCloseSprite()
    {
        objChest1_1.GetComponent<SpriteRenderer>().sprite = closeChestSprite1_1;
        objChest1_2.GetComponent<SpriteRenderer>().sprite = closeChestSprite1_2;
        objChest2_1.GetComponent<SpriteRenderer>().sprite = closeChestSprite2_1;
        objChest2_2.GetComponent<SpriteRenderer>().sprite = closeChestSprite2_2;
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
                SetChestOpen();
            }
            else
            {
                SetChestClose();
            }
        }
    }
}
