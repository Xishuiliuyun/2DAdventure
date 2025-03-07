using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.EventSystems.EventTrigger;

public class Ability : MonoBehaviour
{
    public Character character;
    public PlayerController playerController;
    public GameObject RTButton;

    //���ֲ�����ǰ����
    //HPѪ�� Att������ Def������ Energy����ֵ Exp����ֵ
    [Header("��������")]
    public float hp;
    public float att;
    public float def;
    public float energy;
    public float exp;


    [Header("����/����")]
    public int jumpTimes;//���ÿ���Ծ����
    public bool slipWall;//��ǽ
    public bool climbWall;//��ǽ
    public bool slid;//����

    [Header("��������")]
    public int keyNumber;//Կ������

    private void Awake()
    {
        character = GetComponent<Character>(); 
        playerController = GetComponent<PlayerController>();
    }

    public void InitAbility()//��ʼ����ɫ��������������Ϸʱʹ��
    {
        playerController.isDead = false;
        hp = Constants.PLAYER_INIT_HP;
        character.maxHealth = hp;
        character.currentHealth = hp;
        att = 0;
        def= 0;
        energy = Constants.PLAYER_INIT_ENERGY;
        character.maxEnergy = energy;
        character.currentEnergy = energy;
        exp = 0;

        jumpTimes = 1;
        slipWall = false;
        climbWall = false;
        slid = false;

        //ȡ���ƶ��豸�Ļ������ⰴ����ʾ
        if (slid == false)
        {
            if (Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_ANDROID || SceneLoader.instance.RUNTIME_ENVIRONMENT_WEBGL == Constants.RUNTIME_ENVIRONMENT_WEBGL_MOBILE)
            { RTButton.SetActive(false); }
        }

        //������
        //jumpTimes = 2;
        //slipWall = true;
        //climbWall = true;
        //slid = true;
        //keyNumber = 10;
    }

    //�������
    public bool LearnAbility(AbilityType abType)
    {
        bool result = true;
        switch(abType)
        {
            case AbilityType.slipWall:
                slipWall = true;
                UIManager.instance.ShowMessage("ϰ������ǽ����");
                result = true;
                break;
            case AbilityType.climbWall:
                climbWall = true;
                UIManager.instance.ShowMessage("ϰ������������");
                result = true;
                break;
            case AbilityType.slid:
                slid = true;
                //��Ҫ���������豸��ʾ��ͬ����ʾ��Ϣ����ʾ��Ҳ����İ���
                UIManager.instance.ShowMessage("ϰ���˻�������");
                result = true;
                if (Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_ANDROID || SceneLoader.instance.RUNTIME_ENVIRONMENT_WEBGL == Constants.RUNTIME_ENVIRONMENT_WEBGL_MOBILE)
                { RTButton.SetActive(true); }
                break;
            default:
                result = false;
                break;
        }
        return result;
    }
    //����������޸���Ծ����ʱ��
    public bool LearnAbility(AbilityType abType,int jpTimes)
    {
        bool result = true;
        if (abType != AbilityType.jumpTimes || jpTimes<= jumpTimes) { result = false; return result; }
        jumpTimes = jpTimes;
        if(jpTimes == 2) UIManager.instance.ShowMessage("ϰ���˶�����",5f);
        else if(jpTimes == 3) UIManager.instance.ShowMessage("ϰ����������");
        return result;
    }

}
