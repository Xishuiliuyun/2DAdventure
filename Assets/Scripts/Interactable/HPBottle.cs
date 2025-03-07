using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBottle : MonoBehaviour,IInteractable,ISaveable
{
    private Character character;
    public PlayerController playerController;
    public Sprite fullBottle;
    public Sprite emptyBottle;
    public bool isDone;
    public float healHP;//�ָ���Ѫ��
    private Animator anim;
    public SpriteRenderer spriteRend;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.enabled = false;//�����ʼ��ͼ����
        spriteRend = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (!isDone) HPBottleInteractable();
        else HPBottleUNInteractable(); 

        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }
    private void OnDisable()
    {
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    //�޷�ͨ��Inspector��ȡPlayer�������������޷��糡��������Դ���µ�
    //ͨ��collider��ȡPlayer
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.name == "Player")
        {
            character = collision.GetComponent<Character>();
            playerController = collision.GetComponent<PlayerController>();
        }
    }

    public void TriggerAction()
    {
        if(character.gameObject.GetComponent<PhysicsCheck>().isGround)
        {
            Debug.Log("heal HP");
            anim.enabled = true;
            playerController.inputControl.GamePlay.Disable();//������Ҳ���
            //anim.Play("HealPlayer");//���Ŷ���
            anim.SetTrigger("healPlayer");
            HPBottleUNInteractable();//����Ѫƿ���ɻ���
            HealPlayer();//�ظ����Ѫ��
            //StartCoroutine(HealPlayer());
        }
    }

    private void HPBottleUNInteractable()
    {
        spriteRend.sprite = emptyBottle;
        isDone = true;
        this.gameObject.tag = "UNInteractable";
        GetComponent<BoxCollider2D>().enabled = false;
    }

    private void HPBottleInteractable()
    {
        anim.enabled = false;
        spriteRend.sprite = fullBottle;
        isDone = false;
        this.gameObject.tag = "Interactable";
        GetComponent<BoxCollider2D>().enabled = true;
    }

    private void HealPlayer()
    {
        if (character.currentHealth + healHP >= character.maxHealth)
        {
            character.currentHealth = character.maxHealth;
        }
        else
        {
            character.currentHealth += healHP;
        }
        character.OnHealthChange?.Invoke(character);
    }

    //Я�̷�ʽ����������ɺ�ɾ������ʧ����
    /*private IEnumerator HealPlayer()
    {
        anim.Play("HealPlayer");
        //yield return new WaitForSeconds(1f);
        yield return null;
        Destroy(this.gameObject);
    }*/

    //����������ɺ���ú�����ʵ�ָù��ܣ���ǰ����ɾ�����壬��������ƿ��
    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }

    public void RestartPlayerControl()
    {
        //anim.StopPlayback();
        //anim.Play("");
        //anim.enabled = false;
        spriteRend.sprite = emptyBottle;
        playerController.inputControl.GamePlay.Enable();
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
                HPBottleUNInteractable();
            }
            else
            {
                HPBottleInteractable();
            }
        }
    }
}
