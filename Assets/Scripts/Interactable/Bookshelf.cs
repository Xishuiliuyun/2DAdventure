using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bookshelf : MonoBehaviour, IInteractable,ISaveable
{
    public AttributeType bookshelfType;
    public float value;
    private Ability playerAbility;
    private Character character;
    public bool isDone;


    private void OnEnable()
    {
        if (isDone) SetBookshelfUNInteractable();
        else SetBookshelfInteractable();
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
        if(!isDone)
        {
            switch (bookshelfType)
            {
                case AttributeType.HP:
                    UpHP();
                    break;
                case AttributeType.Att:
                    UpAtt();
                    break;
                case AttributeType.Energy:
                    UpEnergy();
                    break;
                case AttributeType.Exp:
                    UpExp();
                    break;
                case AttributeType.Def:
                    UpDef();
                    break;
                default:
                    break;
            }
            isDone = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerAbility = collision.GetComponent<Ability>();
            character = collision.GetComponent<Character>();
        }
    }

    private float UpHP()
    {
        float currentValue = 0;
        playerAbility.hp += value;
        currentValue = character.changeAbilityValue(bookshelfType);
        if (currentValue != playerAbility.hp) Debug.Log("UpHP ERROR");
        UIManager.instance.ShowMessage("血量上限增加"+ value.ToString()+"点");
        this.gameObject.tag = "UNInteractable";
        GetComponent<BoxCollider2D>().enabled = false;
        return playerAbility.hp;
    }

    private float UpAtt()
    {
        return 0;
    }

    private float UpEnergy()
    {
        float currentValue = 0;
        playerAbility.energy += value;
        currentValue = character.changeAbilityValue(bookshelfType);
        if (currentValue != playerAbility.energy) Debug.Log("UpEnergy ERROR");
        UIManager.instance.ShowMessage("能量上限增加" + value.ToString() + "点");
        this.gameObject.tag = "UNInteractable";
        GetComponent<BoxCollider2D>().enabled = false;
        return playerAbility.energy;
    }

    private float UpExp()
    {
        return 0;
    }

    private float UpDef()
    {
        return 0;
    }

    public void SetBookshelfUNInteractable()
    {
        this.gameObject.tag = "UNInteractable";
        GetComponent<BoxCollider2D>().enabled = false;
        isDone = true;
    }

    public void SetBookshelfInteractable()
    {
        this.gameObject.tag = "Interactable";
        GetComponent<BoxCollider2D>().enabled = true;
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
                SetBookshelfUNInteractable();
            }
            else
            {
                SetBookshelfInteractable();
            }
        }
    }
}
