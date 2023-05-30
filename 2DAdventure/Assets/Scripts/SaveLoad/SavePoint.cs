using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour, IInteractable
{
    [Header("广播")]
    public VoidEventSO saveDataEvent;

    [Header("组件获取")]
    public SpriteRenderer spriteRenderer;
    public GameObject lightObj;

    [Header("图片素材")]
    public Sprite darkSprite;
    public Sprite lightSprite;


    public bool isDone;



    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? lightSprite : darkSprite;
        lightObj.SetActive(isDone);
    }

    /// <summary>
    /// 存档点触碰事件
    /// </summary>
    public void TriggerAction()
    {
        if(!isDone)
        {
            isDone = true;
            spriteRenderer.sprite = lightSprite;
            lightObj.SetActive(true);

            //TODO : 数据保存
            saveDataEvent.RaiseEvent();
            //单次互动
            this.gameObject.tag = "Untagged";
        }
    }
}
