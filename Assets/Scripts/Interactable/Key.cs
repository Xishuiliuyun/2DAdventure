using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class Key : MonoBehaviour,ISaveable
{
    public float distance;//�ƶ�����
    public float duration;//�ƶ�ʱ��
    private BoxCollider2D coll;
    private SpriteRenderer sr;
    private Tweener tween_DOBlendableColor;
    private Tweener tween_DOMoveY;
    public bool canTouch;//����մ�ʱ�����޷�������ʰȡ
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
                UIManager.instance.ShowMessage("���һ��Կ��");
                coll.enabled = false;
                tween_DOBlendableColor = sr.DOBlendableColor(Color.clear, duration);
                tween_DOBlendableColor.OnComplete(DestroyAfterDOTween);
            }
        }
    }

    private void DestroyAfterDOTween()
    {
        //�ӳ�һ֡�����٣����DOTwenn��������
        //����ˣ�ԭ�������ٺ�DOMoveY����ִ�У����Ե��µľ���
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
        //����ʱ������ӵ�ɾ���б��У�ɾ��ԭ�����д��ڵ�����Կ�ף�����ͨ�����ݼ�¼�����instanceԿ��
        DataManager.instance.RemoveCollectionFromList(collectionData);
        DataManager.instance.AddToDelList(this.gameObject);
    }
}
