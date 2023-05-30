using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json;

[DefaultExecutionOrder(-100)]
public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [Header("�¼�����")]
    public VoidEventSO saveDataEvent;
    public VoidEventSO loadDataEvent;
    //�����б�
    private List<ISaveable> saveableList = new List<ISaveable>();

    private Data saveData;
    //�洢·��
    private string jsonFolder;


    private void Awake()
    {
        //����ģʽͨ��д��
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        saveData = new Data();

        jsonFolder = Application.persistentDataPath + "/SAVE DATA/";

        ReadSaveData();
    }

    private void OnEnable()
    {
        saveDataEvent.OnEventRaised += Save;
        loadDataEvent.OnEventRaised += Load;

    }

    private void OnDisable()
    {
        saveDataEvent.OnEventRaised -= Save;
        loadDataEvent.OnEventRaised -= Load;
    }

    private void Update()
    {
        if(Keyboard.current.pKey.wasPressedThisFrame)
        {
            Load();
        }
    }

    //�۲���ģʽ
    public void RegisterSaveData(ISaveable saveable)
    {
        if(!saveableList.Contains(saveable))
        {
            saveableList.Add(saveable);
        }
    }

    public void UnRegisterSaveData(ISaveable saveable)
    {
        saveableList.Remove(saveable);
    }
    /// <summary>
    /// ���ݱ���
    /// </summary>
    public void Save()
    {
        foreach (var saveable in saveableList)
        {
            saveable.GetSaveData(saveData);
        }

        //���ݱ���·��
        var resultPath = jsonFolder + "data.sav";
        //���л�
        var jsonData = JsonConvert.SerializeObject(saveData);
        //����·��
        if(!File.Exists(resultPath))
        {
            Directory.CreateDirectory(jsonFolder);
        }
        //д���ļ�
        File.WriteAllText(resultPath, jsonData);

        //foreach (var item in saveData.characterPosDict)
        //{
        //    Debug.Log(item.Key + "  " + item.Value);
        //}
    }

    public void Load()
    {
        foreach (var saveable in saveableList)
        {
            saveable.LoadData(saveData);
        }
    }
    /// <summary>
    /// ���ݶ�ȡ
    /// </summary>
    private void ReadSaveData()
    {
        var resultPath = jsonFolder + "data.sav";

        if (File.Exists(resultPath))
        {
            var stringData = File.ReadAllText(resultPath);

            //�����л�
            var jsonData = JsonConvert.DeserializeObject<Data>(stringData);

            saveData = jsonData;
        }
    }
}
