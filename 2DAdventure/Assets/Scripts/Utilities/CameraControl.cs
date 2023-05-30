using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("�¼�����")]
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
        //ע��
        cameraShakeEvent.OnEventRaised += OnCameraShakeEvent;

        afterSceneLoadedEvent.OnEventRaised += OnAfterSceneLoadedEvent;
    }


    private void OnDisable()
    {
        //ע��
        cameraShakeEvent.OnEventRaised -= OnCameraShakeEvent;

        afterSceneLoadedEvent.OnEventRaised -= OnAfterSceneLoadedEvent;
    }


    private void OnCameraShakeEvent()
    {
        //�������������
        impulseSource.GenerateImpulse();
    }
    
    //�������ؽ�����ִ�еķ���
    private void OnAfterSceneLoadedEvent()
    {
        GetNewCameraBounds();
    }

    //TODO: �����л������
    //private void Start()
    //{
    //    GetNewCameraBounds();
    //}

    private void GetNewCameraBounds()
    {
        //���ҹ�����Bounds������
        var obj = GameObject.FindGameObjectWithTag("Bounds");
        if (obj == null)
            return;
        //���ҵ������Collider2D����confiner2D�����³����ı߿��滻�ɳ����ı߿�
        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();
        //�������
        confiner2D.InvalidateCache();
        
    }
}
