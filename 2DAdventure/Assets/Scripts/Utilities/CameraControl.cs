using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("事件监听")]
    public VoidEventSO afterSceneLoadedEvent;


    private CinemachineConfiner2D confiner2D;

    public CinemachineImpulseSource impulseSource;

    public VoidEventSO cameraShakeEvent;

    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable()
    {
        //注册
        cameraShakeEvent.OnEventRaised += OnCameraShakeEvent;

        afterSceneLoadedEvent.OnEventRaised += OnAfterSceneLoadedEvent;
    }


    private void OnDisable()
    {
        //注销
        cameraShakeEvent.OnEventRaised -= OnCameraShakeEvent;

        afterSceneLoadedEvent.OnEventRaised -= OnAfterSceneLoadedEvent;
    }


    private void OnCameraShakeEvent()
    {
        //播放摄像机的震动
        impulseSource.GenerateImpulse();
    }
    
    //场景加载结束后执行的方法
    private void OnAfterSceneLoadedEvent()
    {
        GetNewCameraBounds();
    }

    //TODO: 场景切换后更改
    //private void Start()
    //{
    //    GetNewCameraBounds();
    //}

    private void GetNewCameraBounds()
    {
        //查找挂载了Bounds的物体
        var obj = GameObject.FindGameObjectWithTag("Bounds");
        if (obj == null)
            return;
        //将找到物体的Collider2D赋给confiner2D，用新场景的边框替换旧场景的边框
        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();
        //清除缓存
        confiner2D.InvalidateCache();
        
    }
}
