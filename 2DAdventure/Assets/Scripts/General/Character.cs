using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, ISaveable 
{
    [Header("�¼�����")]
    public VoidEventSO newGameEvent;

    [Header("��������")]

    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;

    [Header("�����޵�")]
    //�޵�ʱ��
    public float invulnerableDuration;
    //��ʱ��
    public float invulnerableCounter;
    //�޵�״̬
    public bool invulnerable;


    public UnityEvent<Character> OnHealthChange;

    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;


    //ÿ��ʼһ���µ���Ϸ������ֵΪ��Ѫ
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
    /// �����ж�
    /// </summary>
    /// <param name="attacker"></param>
    public void TakeDamage(Attack attacker)
    {
        //������޵�״̬����ֱ�ӷ��أ��������˺�
        if (invulnerable)
            return;
        //Debug.Log(attacker.damage);

        //����ǰѪ���ܵ��˺���������ʱ���������۳�����ֵ��������㣬������ָ�����ֵ
        if(currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
            TriggerInvulnerable();
            //ִ������
            OnTakeDamage?.Invoke(attacker.transform);
        }
        else
        {
            currentHealth = 0;
            //��������
            OnDie?.Invoke();
        }

        OnHealthChange?.Invoke(this);
    }



    private void Update()
    {
        //�޵�ʱ���ʱ����
        //��ʱ����Ӧ��ʱ�̶������еģ�����д��update������
        if(invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            //���޵�ʱ�����ʱ���޵�״̬���
            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }

        //�����Զ��ظ�
        if(currentPower < maxPower)
        {
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }
    }

    /// <summary>
    /// ����ˮ�������ķ���
    /// </summary>
    /// <param name="other"> ��� </param>
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Water"))
        {
            if (currentHealth > 0)
            {//����������Ѫ��
                currentHealth = 0;
                OnHealthChange?.Invoke(this);
                OnDie.Invoke();
            }
        }
    }


    /// <summary>
    /// ���������޵�
    /// </summary>
    private void TriggerInvulnerable()
    {
        //��������޵�״̬���ͱ���޵�״̬��ͬʱ��ʱ������
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

    //�������ķ���
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
    /// ���ݱ���
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
    /// ���ݼ���
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

            //֪ͨUI����
            OnHealthChange?.Invoke(this);
        }
    }
}
