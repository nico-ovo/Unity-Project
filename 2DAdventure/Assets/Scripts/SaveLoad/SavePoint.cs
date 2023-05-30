using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour, IInteractable
{
    [Header("�㲥")]
    public VoidEventSO saveDataEvent;

    [Header("�����ȡ")]
    public SpriteRenderer spriteRenderer;
    public GameObject lightObj;

    [Header("ͼƬ�ز�")]
    public Sprite darkSprite;
    public Sprite lightSprite;


    public bool isDone;



    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? lightSprite : darkSprite;
        lightObj.SetActive(isDone);
    }

    /// <summary>
    /// �浵�㴥���¼�
    /// </summary>
    public void TriggerAction()
    {
        if(!isDone)
        {
            isDone = true;
            spriteRenderer.sprite = lightSprite;
            lightObj.SetActive(true);

            //TODO : ���ݱ���
            saveDataEvent.RaiseEvent();
            //���λ���
            this.gameObject.tag = "Untagged";
        }
    }
}
