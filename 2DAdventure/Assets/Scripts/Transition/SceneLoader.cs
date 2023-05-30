using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour, ISaveable
{
    public Transform playerTrans;
    public Vector3 firstPosition;
    public Vector3 menuPosition;

    [Header("�¼�����")]
    public SceneLoadEventSO loadEventSO;
    public VoidEventSO NewGameEvent;
    public VoidEventSO backToMenuEvent;


    [Header("�㲥")]
    public VoidEventSO afterSceneLoadedEvent;

    public FadeEventSO fadeEvent;

    public SceneLoadEventSO unloadedSceneEvent;

    [Header("����")]
    public GameSceneSO firstLoadScene;

    public GameSceneSO menuScene;

    [SerializeField] private GameSceneSO currentLoadedScene;
    private GameSceneSO sceneToLoad;
    private Vector3 positionToGo;
    private bool fadeScreen;
    private bool isLoading;

    public float fadeDuration;


    private void Awake()
    {
        //Addressables.LoadSceneAsync(firstLoadScene.sceneReference, LoadSceneMode.Additive);
        //currentLoadedScene = firstLoadScene;
        //currentLoadedScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);
        

    }

    private void Start()
    {
        //NewGame();
        loadEventSO.RaiseLoadRequestEvent(menuScene, menuPosition, true);
    }

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        NewGameEvent.OnEventRaised += NewGame;
        backToMenuEvent.OnEventRaised += OnBackToMeneEvent;
        //�ӿڷ���ע�� 
        ISaveable saveable = this;
        saveable.RegisterSaveData();

    }

    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        NewGameEvent.OnEventRaised -= NewGame;
        backToMenuEvent.OnEventRaised -= OnBackToMeneEvent;
        //�ӿڷ���ע��
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    private void OnBackToMeneEvent()
    {
        sceneToLoad = menuScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, menuPosition, true);
    }

    /// <summary>
    /// ��ʼ���س���
    /// </summary>
    private void NewGame()
    {
        sceneToLoad = firstLoadScene;
        //���������¼�
        //OnLoadRequestEvent(sceneToLoad, firstPosition, true);
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, firstPosition, true);
    }

    /// <summary>
    /// ���������¼�����
    /// </summary>
    /// <param name="locationToLoad"></param>
    /// <param name="posToGo"></param>
    /// <param name="fadeScreen"></param>
    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)
    {
        //���ⷴ������
        if (isLoading)
            return;
        isLoading = true;

        //�洢���ݽ����ı���
        sceneToLoad = locationToLoad;
        positionToGo = posToGo;
        this.fadeScreen = fadeScreen;

        //ж�ص�ǰ����
        if (currentLoadedScene != null)
        {
            StartCoroutine(UnLoadPreviousScens());
        }
        else
        {
            LoadNewScene();
        }

    }

    private IEnumerator UnLoadPreviousScens()
    {
        if(fadeScreen)
        {
            //TODO: ʵ�ֽ���
            fadeEvent.FadeIn(fadeDuration);
        }

        //�ȴ�������ȫ���
        yield return new WaitForSeconds(fadeDuration);

        //�㲥�¼�����Ѫ����ʾ
        unloadedSceneEvent.RaiseLoadRequestEvent(sceneToLoad, positionToGo, true);

        yield return currentLoadedScene.sceneReference.UnLoadScene();

        //�ر�����
        playerTrans.gameObject.SetActive(false);
        
        //�����³���
        LoadNewScene();
    }
    /// <summary>
    /// �����µĳ���
    /// </summary>
    private void LoadNewScene()
    {
        var loadingOption =  sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadingOption.Completed += OnLoadCompleted;
        
    }

    /// <summary>
    /// �������ؽ�����
    /// </summary>
    /// <param name="obj"></param>
    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> obj)
    {
        currentLoadedScene = sceneToLoad;

        playerTrans.position = positionToGo;

        playerTrans.gameObject.SetActive(true);
        if(fadeScreen)
        {
            //TODO
            fadeEvent.FadeOut(fadeDuration);
        }
        
        isLoading = false;

        if(currentLoadedScene.sceneType != E_SceneType.Menu)
        {
            afterSceneLoadedEvent.RaiseEvent();
        }
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    public void GetSaveData(Data data)
    {
        data.SaveGameScene(currentLoadedScene);
    }

    public void LoadData(Data data)
    {
        var playerID = playerTrans.GetComponent<DataDefination>().ID;
        if(data.characterPosDict.ContainsKey(playerID))
        {
            positionToGo = data.characterPosDict[playerID].ToVector3();
            sceneToLoad = data.GetSavedScene();

            OnLoadRequestEvent(sceneToLoad, positionToGo, true);
        }
    }
}
