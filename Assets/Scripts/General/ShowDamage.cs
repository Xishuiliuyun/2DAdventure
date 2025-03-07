using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ShowDamage : MonoBehaviour
{
    //public Transform targetTransform;
    //public TextMeshProUGUI damageText;
    public Transform targetTransform;
    public BoolEvenSO showDamageEvenSO;//�˺���ʾ�����¼�����
    public GameObject damageTextPrefab;
    public Vector2 showPosOffset;//�˺�������ʾλ��������˺������ƫ����
    public Vector2 posOffset;//��������ʾʱλ��ƫ����
    public float delayTime;//�ӳ�ʱ�����ִ���˺�������ʧ����
    public float durationTime;//�˺���ʾ����ʱ��
    public bool showDamage;//�Ƿ��˺���ʾ������
    private string fadeTweenId = "fadeTween_ShowDamage";
    //private bool isShowingDamage;//�Ƿ��Ѿ�����ʾ�˺��ˣ�ƫ��λ����
    private GameObject lastShowObj;
    private List<GameObject> showObjList = new List<GameObject>();
    private Vector3 showPos;

    private void Awake()
    {
        if (targetTransform.CompareTag("Player"))
        {
            StartCoroutine(GetShowDamageLater());
        }
        else showDamage = DataManager.instance.settingsData.generalSettingsData.showDamage;
    }

    private void OnEnable()
    {
        if (targetTransform.CompareTag("Player"))
        {
            StartCoroutine(GetShowDamageLater());
        }
        else showDamage = DataManager.instance.settingsData.generalSettingsData.showDamage;
        showDamageEvenSO.OnEventRised += OnShowDamageEven;
    }

    private void OnDisable()
    {
        showDamageEvenSO.OnEventRised -= OnShowDamageEven;
    }

    private void Update()
    {
        if (showDamage)
        {//����localScale
            foreach (GameObject showObj in showObjList)
            {
                if (showObj != null) showObj.transform.localScale = targetTransform.localScale;
            }
            /*if (targetTransform.CompareTag("Player"))
            {
                foreach(GameObject showObj in showObjList)
                {
                    if (showObj != null) showObj.transform.localScale = targetTransform.localScale;
                }
            }
            else if (targetTransform.CompareTag("Enemy"))
            {
                foreach (GameObject showObj in showObjList)
                {
                    if (showObj != null) showObj.transform.localScale = new Vector3(-targetTransform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
            }*/
        }
            
    }

    public void ShowDamageText(Vector3 characterPos,float damage)
    {
        if (!showDamage) return;
        if(lastShowObj == null)
        {//�����˺���������ʧ
            //lastShowPos = characterPos+ (Vector3)showPosOffset;
            showObjList.Clear();
            showPos = targetTransform.position + (Vector3)showPosOffset;
            SetDamageInfo(damage.ToString());
        }
        else
        {//������һ�ε��˺�����
            int posOffsetNum = 0;
            if (ExistEmptyInList(ref posOffsetNum))
            {
                showPos = targetTransform.position + (Vector3)showPosOffset + (Vector3)posOffset * posOffsetNum;
            }
            else showPos = targetTransform.position + (Vector3)showPosOffset + (Vector3)posOffset * showObjList.Count;
            SetDamageInfo(damage.ToString());
        }
    }

    public void SetDamageInfo(string str)
    {
        lastShowObj = Instantiate(damageTextPrefab, showPos, Quaternion.Euler(0, 0, 0),transform);
        bool flag = true;
        int showObjNum = 0;
        flag = ExistEmptyInList(ref showObjNum);
        if (flag)
        {
            showObjList[showObjNum] = lastShowObj;
        }
        else 
        {
            showObjList.Add(lastShowObj);
            showObjNum = showObjList.Count - 1;
        }
        TextMeshProUGUI damageText;
        damageText = lastShowObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (damageText == null) { UnityEngine.Debug.Log("ShowDamageTextNotFoundERR"); return; }
        damageText.text = str;
        damageText.DOFade(0, durationTime)
            .SetId(fadeTweenId)
            .SetDelay(delayTime)
            .OnComplete(() => {
                //posOffsetNum = showObjNum;
                if (showObjList.Count == 0) { UnityEngine.Debug.Log("showObjListERR"); return; }
                Destroy(showObjList[showObjNum]);
            });
    }

    public bool ExistEmptyInList(ref int showObjNum)
    {
        bool iRet = false;
        for (int i = 0; i < showObjList.Count; i++)
        {
            if (showObjList[i] == null)
            {
                showObjNum = i;
                iRet = true;
                break;
            }
        }
        return iRet;
    }

    public void OnShowDamageEven(bool flag)
    {
        showDamage = flag;
        if (!flag)
        {
            CleanAllDamage();
        }
    }

    public void CleanAllDamage()
    {
        DOTween.Kill(fadeTweenId, true);
        showObjList.Clear();
    }

    IEnumerator GetShowDamageLater()
    {
        yield return new WaitForSeconds(0.1f);
        showDamage = DataManager.instance.settingsData.generalSettingsData.showDamage;
    }

}
