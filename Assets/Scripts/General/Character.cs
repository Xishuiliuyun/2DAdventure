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

    [Header("��������")]
    public float maxHealth;//�������ֵ
    public float currentHealth;//��ǰ����ֵ
    public float maxEnergy;//�������ֵ
    public float currentEnergy;//��ǰ����ֵ
    public float energyRecoverySpeed;//�����ָ��ٶ�

    [Header("�����޵�")]
    public float invulnerableDuration;//����ʱ��
    private float invulnerableCounter;//������
    public bool invulnerable;//�Ƿ��޵�

    public VoidEvenSO newGameEvent;
    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Vector3,bool> OnTakeDamage;
    public UnityEvent onDie;

    private Ability playerAbility;
    //��ʾ�˺��¼�
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

    //���¼����������ã��ڱ�������д�����
    private void NewGame()
    {
        if(this.gameObject.CompareTag("Player"))
        {
            //����Ϸʱ���������ʼ��
            playerAbility = GetComponent<Ability>();
            maxHealth = playerAbility.hp;
            currentHealth = maxHealth;
            OnHealthChange?.Invoke(this);

            maxEnergy = playerAbility.energy;
            currentEnergy = maxEnergy;
            uiManager.EnergyChange(this);

            //����Ϸʱ����
            uiManager.ChangeKeyNumber(playerAbility.keyNumber);//����Կ������
            uiManager.message.enabled = false;//������ʾ��Ϣ����ʾ
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

    //��ɫ��ˮ��������Ѫ������
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

    public void TakeDamage(Attack attacker)//����
    {
        //Debug.Log("takeDamageHappen");
        if (invulnerable)
            return;

        //��ʾ�˺�
        OnShowDamage?.Invoke(transform.position,attacker.damage);

        if (currentHealth - attacker.damage > 0)//��������
        {
            currentHealth -= attacker.damage;
            TriggerInvulnerable();
            //���������¼�
            OnTakeDamage?.Invoke(attacker.hitPos, attacker.isSpike);
        }
        else//����
        {
            currentHealth = 0;
            Debug.Log("����");
            onDie?.Invoke();
        }
        OnHealthChange?.Invoke(this);
    }

    private void TriggerInvulnerable()//�����޵�
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
            //������������ʾ
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
                //maxHealth�仯ǰ�޸�Ѫ������
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

    //�浵
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

    //����
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

    //���ô浵����
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
    //��ȡ�浵����
    public void GetPlayerData(PlayerData playerData)
    {
        gameObject.layer = 0;//��ȡ����ǰ��������ͼ�㣬��������⵽player����״̬�쳣
        transform.position = playerData.pos.GetValue();
        transform.localScale = playerData.scale.GetValue();
        playerAbility.GetComponent<PlayerController>().isDead = playerData.isDead;
        currentHealth = playerData.currentHP;
        maxHealth = playerData.hp;
        uiManager.ChangeHPFrame(Constants.PLAYER_INIT_HP, playerData.hp);//������޸�Ѫ������
        OnHealthChange?.Invoke(this);
        currentEnergy = playerData.currentEnergy;
        maxEnergy = playerData.energy;
        uiManager.ChangeEnergyFrame(Constants.PLAYER_INIT_ENERGY, playerData.energy);//������޸�����������
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
        //ȡ���ƶ��豸�Ļ������ⰴ����ʾ
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
    //���ô浵����
    public void SetEnemyData(EnemyData enemyData)
    {
        enemyData.pos = new MyVector3(transform.position);
        enemyData.scale = new MyVector3(transform.localScale);
        enemyData.state = GetComponent<Enemy>().curState;
        enemyData.waitTimeCounter = GetComponent<Enemy>().waitTimeCounter;
        enemyData.lostTimeCounter = GetComponent<Enemy>().lostTimeCounter;
        enemyData.currentHP = currentHealth;
        enemyData.enemyName = GetComponent<Enemy>().enemyName;
        //enemyData.isDead = false;//����״̬�ڹ�������ʱ�Ž����޸ģ�������ʵ���ᱻע�����޷��޸�״̬��״̬�ᱻ����
    }
    //��ȡ�浵����
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

    //�����ã��ӳ�һ֡��������״̬�ᵼ�´浵ʱ�����쳣
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
