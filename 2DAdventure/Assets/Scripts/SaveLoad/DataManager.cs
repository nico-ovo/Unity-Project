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

    [Header("事件监听")]
    public VoidEventSO saveDataEvent;
    public VoidEventSO loadDataEvent;
    //创建列表
    private List<ISaveable> saveableList = new List<ISaveable>();

    private Data saveData;
    //存储路径
    private string jsonFolder;


    private void Awake()
    {
        //单例模式通常写法
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

    //观察者模式
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
    /// 数据保存
    /// </summary>
    public void Save()
    {
        foreach (var saveable in saveableList)
        {
            saveable.GetSaveData(saveData);
        }

        //数据保存路径
        var resultPath = jsonFolder + "data.sav";
        //序列化
        var jsonData = JsonConvert.SerializeObject(saveData);
        //创建路径
        if(!File.Exists(resultPath))
        {
            Directory.CreateDirectory(jsonFolder);
        }
        //写入文件
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
    /// 数据读取
    /// </summary>
    private void ReadSaveData()
    {
        var resultPath = jsonFolder + "data.sav";

        if (File.Exists(resultPath))
        {
            var stringData = File.ReadAllText(resultPath);

            //反序列化
            var jsonData = JsonConvert.DeserializeObject<Data>(stringData);

            saveData = jsonData;
        }
    }
}
