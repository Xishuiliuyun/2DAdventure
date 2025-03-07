using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Character : MonoBehaviour,ISaveable
{
    public UIManager uiManager;
    public HPControl HPControl;

    [Header("基本参数")]
    public float maxHealth;//最大生命值
    public float currentHealth;//当前生命值
    public float maxEnergy;//最大能量值
    public float currentEnergy;//当前能量值
    public float energyRecoverySpeed;//能量恢复速度

    [Header("受伤无敌")]
    public float invulnerableDuration;//持续时间
    private float invulnerableCounter;//计数器
    public bool invulnerable;//是否无敌

    public VoidEvenSO newGameEvent;
    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Vector3,bool> OnTakeDamage;
    public UnityEvent onDie;

    private Ability playerAbility;
    //显示伤害事件
    public UnityEvent<Vector3, float> OnShowDamage;

    private void Awake()
    {
        //uiManager = GetComponent<UIManager>();
        if (this.gameObject.CompareTag("Player")) playerAbility = GetComponent<Ability>();
    }

    private void OnEnable()
    {
        OnHealthChange?.Invoke(this);
        //newGameEvent.OnEventRised += NewGame;
        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        //newGameEvent.OnEventRised -= NewGame;
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    //此事件方法已弃用，在别的流程中处理了
    private void NewGame()
    {
        if(this.gameObject.CompareTag("Player"))
        {
            //新游戏时玩家能力初始化
            playerAbility = GetComponent<Ability>();
            maxHealth = playerAbility.hp;
            currentHealth = maxHealth;
            OnHealthChange?.Invoke(this);

            maxEnergy = playerAbility.energy;
            currentEnergy = maxEnergy;
            uiManager.EnergyChange(this);

            //新游戏时设置
            uiManager.ChangeKeyNumber(playerAbility.keyNumber);//设置钥匙数量
            uiManager.message.enabled = false;//设置提示信息不显示
        }
        else
        {
            currentHealth = maxHealth;
            OnHealthChange?.Invoke(this);
        }
    }

    private void Update()
    {
        if(invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
        }
        if (invulnerableCounter <= 0)
        {
            invulnerable = false;
        }
        if (this.gameObject.CompareTag("Player"))
        {
            if (currentEnergy < maxEnergy)
            {
                currentEnergy += energyRecoverySpeed * Time.deltaTime;
                if (currentEnergy > maxEnergy) currentEnergy = maxEnergy;
                uiManager.EnergyChange(this);
            }
        }
        
    }

    //角色落水，死亡，血量归零
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            if(currentHealth>0)
            {
                currentHealth = 0;
                onDie?.Invoke();
                OnHealthChange?.Invoke(this);
                //Debug.Log("inWater");
            }
        }
    }

    public void TakeDamage(Attack attacker)//受伤
    {
        //Debug.Log("takeDamageHappen");
        if (invulnerable)
            return;

        //显示伤害
        OnShowDamage?.Invoke(transform.position,attacker.damage);

        if (currentHealth - attacker.damage > 0)//触发受伤
        {
            currentHealth -= attacker.damage;
            TriggerInvulnerable();
            //启动受伤事件
            OnTakeDamage?.Invoke(attacker.hitPos, attacker.isSpike);
        }
        else//死亡
        {
            currentHealth = 0;
            Debug.Log("死亡");
            onDie?.Invoke();
        }
        OnHealthChange?.Invoke(this);
    }

    private void TriggerInvulnerable()//触发无敌
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

    public bool UseEnergy(float cost)
    {

        if (cost > currentEnergy) return false;
        else
        {
            currentEnergy -= cost;
            //处理能量条显示
            uiManager.EnergyChange(this);
            return true;
        } 
    }

    public float changeAbilityValue(AttributeType type)
    {
        playerAbility = GetComponent<Ability>();
        float currentValue = 0;
        switch(type)
        {
            case AttributeType.HP:
                //maxHealth变化前修改血条长度
                uiManager.ChangeHPFrame(Constants.PLAYER_INIT_HP, playerAbility.hp);
                maxHealth = playerAbility.hp;
                currentValue = playerAbility.hp;
                OnHealthChange?.Invoke(this);
                break;
            case AttributeType.Att:
                break;
            case AttributeType.Energy:
                uiManager.ChangeEnergyFrame(Constants.PLAYER_INIT_ENERGY, playerAbility.energy);
                maxEnergy = playerAbility.energy;
                currentValue = playerAbility.energy;
                uiManager.EnergyChange(this);
                break;
            case AttributeType.Exp:
                break;
            case AttributeType.Def:
                break;
            default:
                break;
        }
        return currentValue;
    }


    public DataDefinition GetDataID()
    {
        return GetComponent<DataDefinition>();
    }

    //存档
    public void GetSaveData(Data data)
    {
        //Player
        if(this.gameObject.CompareTag("Player"))
        {
            PlayerData playerData = new PlayerData();
            SetPlayerData(playerData);
            if (data.playerDataDict.ContainsKey(GetDataID().ID))
            {
                data.playerDataDict[GetDataID().ID] = playerData;
            }
            else
            {
                data.playerDataDict.Add(GetDataID().ID, playerData);
            }
        }
        else//Enemy
        {
            EnemyData enemyData = new EnemyData { sceneName = SceneManager.GetSceneAt(1).name };
            SetEnemyData(enemyData);
            if (data.enemyDataDict.ContainsKey(GetDataID().ID))
            {
                data.enemyDataDict[GetDataID().ID] = enemyData;
            }
            else
            {
                data.enemyDataDict.Add(GetDataID().ID, enemyData);
            }
        }
    }

    //读档
    public void LoadData(Data data)
    {
        //Player
        if (this.gameObject.CompareTag("Player"))
        {
            if(data.playerDataDict.ContainsKey(GetDataID().ID))
            {
                GetPlayerData(data.playerDataDict[GetDataID().ID]);
            }
        }
        else//Enemy
        {
            if (data.enemyDataDict.ContainsKey(GetDataID().ID))
            {
                GetEnemyData(data.enemyDataDict[GetDataID().ID]);
            }
        }
    }

    //设置存档数据
    public void SetPlayerData(PlayerData playerData)
    {
        playerData.pos = new MyVector3(transform.position);
        playerData.scale = new MyVector3(transform.localScale);
        playerData.isDead = playerAbility.GetComponent<PlayerController>().isDead;
        playerData.currentHP = currentHealth;
        playerData.currentEnergy = currentEnergy;
        playerData.hp = playerAbility.hp;
        playerData.att = playerAbility.att;
        playerData.def = playerAbility.def;
        playerData.energy = playerAbility.energy;
        playerData.exp = playerAbility.exp;
        playerData.jumpTimes = playerAbility.jumpTimes;
        playerData.slipWall = playerAbility.slipWall;
        playerData.climbWall = playerAbility.climbWall;
        playerData.slid = playerAbility.slid;
        playerData.keyNumber = playerAbility.keyNumber;
    }
    //读取存档数据
    public void GetPlayerData(PlayerData playerData)
    {
        gameObject.layer = 0;//读取数据前设置物理图层，避免怪物检测到player导致状态异常
        transform.position = playerData.pos.GetValue();
        transform.localScale = playerData.scale.GetValue();
        playerAbility.GetComponent<PlayerController>().isDead = playerData.isDead;
        currentHealth = playerData.currentHP;
        maxHealth = playerData.hp;
        uiManager.ChangeHPFrame(Constants.PLAYER_INIT_HP, playerData.hp);//添加了修改血条长度
        OnHealthChange?.Invoke(this);
        currentEnergy = playerData.currentEnergy;
        maxEnergy = playerData.energy;
        uiManager.ChangeEnergyFrame(Constants.PLAYER_INIT_ENERGY, playerData.energy);//添加了修改能量条长度
        uiManager.EnergyChange(this);
        playerAbility.hp = playerData.hp;
        playerAbility.att = playerData.att;
        playerAbility.def = playerData.def;
        playerAbility.energy = playerData.energy;
        playerAbility.exp = playerData.exp;
        playerAbility.jumpTimes = playerData.jumpTimes;
        playerAbility.slipWall = playerData.slipWall;
        playerAbility.climbWall = playerData.climbWall;
        playerAbility.slid = playerData.slid;
        //取消移动设备的滑铲虚拟按键显示
        if(playerAbility.slid == false)
        {
            if (Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_ANDROID || SceneLoader.instance.RUNTIME_ENVIRONMENT_WEBGL == Constants.RUNTIME_ENVIRONMENT_WEBGL_MOBILE)
            { playerAbility.RTButton.SetActive(false); }
        }
        else
        {
            if (Constants.RUNTIME_ENVIRONMENT == Constants.RUNTIME_ENVIRONMENT_ANDROID || SceneLoader.instance.RUNTIME_ENVIRONMENT_WEBGL == Constants.RUNTIME_ENVIRONMENT_WEBGL_MOBILE)
            { playerAbility.RTButton.SetActive(true); }
        }
        playerAbility.keyNumber = playerData.keyNumber;
        uiManager.ChangeKeyNumber(playerAbility.keyNumber);
        StartCoroutine(SetPlayerLayerLater());
    }
    //设置存档数据
    public void SetEnemyData(EnemyData enemyData)
    {
        enemyData.pos = new MyVector3(transform.position);
        enemyData.scale = new MyVector3(transform.localScale);
        enemyData.state = GetComponent<Enemy>().curState;
        enemyData.waitTimeCounter = GetComponent<Enemy>().waitTimeCounter;
        enemyData.lostTimeCounter = GetComponent<Enemy>().lostTimeCounter;
        enemyData.currentHP = currentHealth;
        enemyData.enemyName = GetComponent<Enemy>().enemyName;
        //enemyData.isDead = false;//死亡状态在怪物死亡时才进行修改，后续该实例会被注销，无法修改状态，状态会被保存
    }
    //读取存档数据
    public void GetEnemyData(EnemyData enemyData)
    {
        transform.position = enemyData.pos.GetValue();
        transform.localScale = enemyData.scale.GetValue();
        //StartCoroutine(SwitchEnemyState(enemyData.state));
        GetComponent<Enemy>().SwitchState(enemyData.state);
        //GetComponent<Enemy>().curState = enemyData.state;
        GetComponent<Enemy>().waitTimeCounter = enemyData.waitTimeCounter;
        GetComponent<Enemy>().lostTimeCounter = enemyData.lostTimeCounter;
        currentHealth = enemyData.currentHP;
        OnHealthChange?.Invoke(this);
        HPControl.SetHealthDelayImage();
        if (enemyData.isDead) DataManager.instance.AddToDelList(this.gameObject);
    }

    //已弃用，延迟一帧后再设置状态会导致存档时数据异常
    IEnumerator SwitchEnemyState(NPCState state)
    {
        yield return null;
        GetComponent<Enemy>().SwitchState(state);
    }

    IEnumerator SetPlayerLayerLater()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.layer = LayerMask.NameToLayer("Player");
    }
}
