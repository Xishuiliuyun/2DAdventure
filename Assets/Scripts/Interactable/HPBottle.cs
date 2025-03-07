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
    public float healHP;//恢复的血量
    private Animator anim;
    public SpriteRenderer spriteRend;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.enabled = false;//处理初始贴图错误
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

    //无法通过Inspector获取Player，可能是由于无法跨场景引用资源导致的
    //通过collider获取Player
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
            playerController.inputControl.GamePlay.Disable();//禁用玩家操作
            //anim.Play("HealPlayer");//播放动画
            anim.SetTrigger("healPlayer");
            HPBottleUNInteractable();//设置血瓶不可互动
            HealPlayer();//回复玩家血量
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

    //携程方式动画播放完成后删除物体失败了
    /*private IEnumerator HealPlayer()
    {
        anim.Play("HealPlayer");
        //yield return new WaitForSeconds(1f);
        yield return null;
        Destroy(this.gameObject);
    }*/

    //动画播放完成后调用函数可实现该功能，当前不用删除物体，保留破碎瓶子
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
