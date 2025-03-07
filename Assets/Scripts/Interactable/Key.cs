using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class Key : MonoBehaviour,ISaveable
{
    public float distance;//移动距离
    public float duration;//移动时间
    private BoxCollider2D coll;
    private SpriteRenderer sr;
    private Tweener tween_DOBlendableColor;
    private Tweener tween_DOMoveY;
    public bool canTouch;//宝箱刚打开时设置无法立即被拾取
    public float canTouchTime;

    public CollectionData collectionData;

    private void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(setCanTouch());
        tween_DOMoveY = transform.DOMoveY(transform.position.y + distance, duration).SetLoops(-1, LoopType.Yoyo);
        collectionData = new CollectionData
        {
            pos = new MyVector3(transform.position),
            sceneName = SceneManager.GetSceneAt(1).name,
            prefabName = Constants.KEY_PREFAB_NAME,
            num = 1
        };
    }
    private void OnEnable()
    {
        //coll = GetComponent<BoxCollider2D>();
        //sr = GetComponent<SpriteRenderer>();
        //tween_DOMoveY = transform.DOMoveY(transform.position.y + distance, duration).SetLoops(-1, LoopType.Yoyo);
        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    /*private void Update()
    {
        if(sr.color == Color.clear) Destroy(this.gameObject);
    }*/

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(canTouch)
            {
                collision.GetComponent<Ability>().keyNumber++;
                if (!UIManager.instance.ChangeKeyNumber(collision.GetComponent<Ability>().keyNumber)) Debug.Log("KEY Number ERR");
                UIManager.instance.ShowMessage("获得一把钥匙");
                coll.enabled = false;
                tween_DOBlendableColor = sr.DOBlendableColor(Color.clear, duration);
                tween_DOBlendableColor.OnComplete(DestroyAfterDOTween);
            }
        }
    }

    private void DestroyAfterDOTween()
    {
        //延迟一帧后销毁，解决DOTwenn警告问题
        //搞错了，原来是销毁后DOMoveY还在执行，所以导致的警告
        if(tween_DOMoveY!=null) tween_DOMoveY.Kill();
        //DOTween.Kill(gameObject);
        Invoke("DestroyObject", Time.deltaTime);
    }

    private void DestroyObject()
    {
        DataManager.instance.RemoveCollectionFromList(collectionData);
        Destroy(gameObject);
    }

    public IEnumerator setCanTouch()
    {
        canTouch = false;
        yield return new WaitForSeconds(canTouchTime);
        canTouch = true;
    }

    public DataDefinition GetDataID()
    {
        return GetComponent<DataDefinition>();
    }

    public void GetSaveData(Data data)
    {
        DataManager.instance.AddCollectionToList(collectionData);
    }

    public void LoadData(Data data)
    {
        //加载时将其添加到删除列表中，删除原场景中存在的所有钥匙，后续通过数据记录的情况instance钥匙
        DataManager.instance.RemoveCollectionFromList(collectionData);
        DataManager.instance.AddToDelList(this.gameObject);
    }
}
