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

    //部分参数当前无用
    //HP血量 Att攻击力 Def防御力 Energy能量值 Exp经验值
    [Header("基础属性")]
    public float hp;
    public float att;
    public float def;
    public float energy;
    public float exp;


    [Header("能力/属性")]
    public int jumpTimes;//设置可跳跃次数
    public bool slipWall;//滑墙
    public bool climbWall;//爬墙
    public bool slid;//滑铲

    [Header("道具数量")]
    public int keyNumber;//钥匙数量

    private void Awake()
    {
        character = GetComponent<Character>(); 
        playerController = GetComponent<PlayerController>();
    }

    public void InitAbility()//初始化角色技能能力，新游戏时使用
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

        //取消移动设备的滑铲虚拟按键显示
        if (slid == false)
        {
            if (Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_ANDROID || SceneLoader.instance.RUNTIME_ENVIRONMENT_WEBGL == Constants.RUNTIME_ENVIRONMENT_WEBGL_MOBILE)
            { RTButton.SetActive(false); }
        }

        //调试用
        //jumpTimes = 2;
        //slipWall = true;
        //climbWall = true;
        //slid = true;
        //keyNumber = 10;
    }

    //添加能力
    public bool LearnAbility(AbilityType abType)
    {
        bool result = true;
        switch(abType)
        {
            case AbilityType.slipWall:
                slipWall = true;
                UIManager.instance.ShowMessage("习得了爬墙能力");
                result = true;
                break;
            case AbilityType.climbWall:
                climbWall = true;
                UIManager.instance.ShowMessage("习得了攀爬能力");
                result = true;
                break;
            case AbilityType.slid:
                slid = true;
                //需要根据输入设备显示不同的提示信息，提示玩家操作的按键
                UIManager.instance.ShowMessage("习得了滑铲能力");
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
    //添加能力，修改跳跃次数时用
    public bool LearnAbility(AbilityType abType,int jpTimes)
    {
        bool result = true;
        if (abType != AbilityType.jumpTimes || jpTimes<= jumpTimes) { result = false; return result; }
        jumpTimes = jpTimes;
        if(jpTimes == 2) UIManager.instance.ShowMessage("习得了二段跳",5f);
        else if(jpTimes == 3) UIManager.instance.ShowMessage("习得了三段跳");
        return result;
    }

}
