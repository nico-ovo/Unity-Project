using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeCanvas : MonoBehaviour
{
    [Header("事件监听")]
    public FadeEventSO fadeEvent;

    public Image fadeImage;

    private void OnEnable()
    {
        fadeEvent.OnEventRaised += OnFadeEvent;
    }

    private void OnDisable()
    {
        fadeEvent.OnEventRaised -= OnFadeEvent;
    }

    /// <summary>
    /// 颜色渐变
    /// </summary>
    /// <param name="target">目标颜色</param>
    /// <param name="duration">持续时间</param>
    private void OnFadeEvent(Color target, float duration, bool fadeIn)
    {
        fadeImage.DOBlendableColor(target, duration);
    }
}
