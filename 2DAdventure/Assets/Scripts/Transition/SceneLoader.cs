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

    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;
    public VoidEventSO NewGameEvent;
    public VoidEventSO backToMenuEvent;


    [Header("广播")]
    public VoidEventSO afterSceneLoadedEvent;

    public FadeEventSO fadeEvent;

    public SceneLoadEventSO unloadedSceneEvent;

    [Header("场景")]
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
        //接口方法注册 
        ISaveable saveable = this;
        saveable.RegisterSaveData();

    }

    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        NewGameEvent.OnEventRaised -= NewGame;
        backToMenuEvent.OnEventRaised -= OnBackToMeneEvent;
        //接口方法注销
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    private void OnBackToMeneEvent()
    {
        sceneToLoad = menuScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, menuPosition, true);
    }

    /// <summary>
    /// 初始加载场景
    /// </summary>
    private void NewGame()
    {
        sceneToLoad = firstLoadScene;
        //场景加载事件
        //OnLoadRequestEvent(sceneToLoad, firstPosition, true);
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, firstPosition, true);
    }

    /// <summary>
    /// 场景加载事件请求
    /// </summary>
    /// <param name="locationToLoad"></param>
    /// <param name="posToGo"></param>
    /// <param name="fadeScreen"></param>
    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)
    {
        //避免反复传送
        if (isLoading)
            return;
        isLoading = true;

        //存储传递进来的变量
        sceneToLoad = locationToLoad;
        positionToGo = posToGo;
        this.fadeScreen = fadeScreen;

        //卸载当前场景
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
            //TODO: 实现渐入
            fadeEvent.FadeIn(fadeDuration);
        }

        //等待场景完全变黑
        yield return new WaitForSeconds(fadeDuration);

        //广播事件调整血条显示
        unloadedSceneEvent.RaiseLoadRequestEvent(sceneToLoad, positionToGo, true);

        yield return currentLoadedScene.sceneReference.UnLoadScene();

        //关闭人物
        playerTrans.gameObject.SetActive(false);
        
        //加载新场景
        LoadNewScene();
    }
    /// <summary>
    /// 加载新的场景
    /// </summary>
    private void LoadNewScene()
    {
        var loadingOption =  sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadingOption.Completed += OnLoadCompleted;
        
    }

    /// <summary>
    /// 场景加载结束后
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
