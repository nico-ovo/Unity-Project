using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, ISaveable 
{
    [Header("事件监听")]
    public VoidEventSO newGameEvent;

    [Header("基本属性")]

    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;

    [Header("受伤无敌")]
    //无敌时间
    public float invulnerableDuration;
    //计时器
    public float invulnerableCounter;
    //无敌状态
    public bool invulnerable;


    public UnityEvent<Character> OnHealthChange;

    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;


    //每开始一次新的游戏，生命值为满血
    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void NewGame()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }


    private void OnEnable()
    {
        newGameEvent.OnEventRaised += NewGame;

        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        newGameEvent.OnEventRaised -= NewGame;

        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }


    /// <summary>
    /// 受伤判定
    /// </summary>
    /// <param name="attacker"></param>
    public void TakeDamage(Attack attacker)
    {
        //如果是无敌状态，则直接返回，不触发伤害
        if (invulnerable)
            return;
        //Debug.Log(attacker.damage);

        //当当前血量受到伤害不会死亡时，才正常扣除生命值，否则归零，以免出现负生命值
        if(currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
            TriggerInvulnerable();
            //执行受伤
            OnTakeDamage?.Invoke(attacker.transform);
        }
        else
        {
            currentHealth = 0;
            //触发死亡
            OnDie?.Invoke();
        }

        OnHealthChange?.Invoke(this);
    }



    private void Update()
    {
        //无敌时间计时方法
        //计时方法应该时刻都在运行的，所以写在update函数中
        if(invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            //当无敌时间结束时，无敌状态解除
            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }

        //能量自动回复
        if(currentPower < maxPower)
        {
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }
    }

    /// <summary>
    /// 碰到水面死亡的方法
    /// </summary>
    /// <param name="other"> 玩家 </param>
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Water"))
        {
            if (currentHealth > 0)
            {//死亡，更新血量
                currentHealth = 0;
                OnHealthChange?.Invoke(this);
                OnDie.Invoke();
            }
        }
    }


    /// <summary>
    /// 触发受伤无敌
    /// </summary>
    private void TriggerInvulnerable()
    {
        //如果不是无敌状态，就变成无敌状态，同时计时器重置
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

    //能量消耗方法
    public void OnSlide(int cost)
    {
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    /// <summary>
    /// 数据保存
    /// </summary>
    /// <param name="data"></param>
    public void GetSaveData(Data data)
    {
        if(data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            data.characterPosDict[GetDataID().ID] = new SerializeVector3(transform.position);
            data.floatSavedData[GetDataID().ID + "health"] = this.currentHealth;
            data.floatSavedData[GetDataID().ID + "power"] = this.currentPower;
        }
        else
        {
            data.characterPosDict.Add(GetDataID().ID, new SerializeVector3(transform.position));
            data.floatSavedData.Add(GetDataID().ID + "health", this.currentHealth);
            data.floatSavedData.Add(GetDataID().ID + "power", this.currentPower);
        }
    }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="data"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void LoadData(Data data)
    {
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            transform.position = data.characterPosDict[GetDataID().ID].ToVector3();
            this.currentHealth = data.floatSavedData[GetDataID().ID + "health"];
            this.currentPower = data.floatSavedData[GetDataID().ID + "power"];

            //通知UI更新
            OnHealthChange?.Invoke(this);
        }
    }
}
