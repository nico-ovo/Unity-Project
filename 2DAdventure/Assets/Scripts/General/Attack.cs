using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("��������")]
    //�˺�
    public int damage;
    //������Χ
    public float attackRange;
    //����Ƶ��
    public float attackRate;

    private void OnTriggerStay2D(Collider2D other)
    {
        //ͨ��oher�����ʱ���������
        //�����������û���������Ͳ���ִ��takedamage�����ⱨ��
        other.GetComponent<Character>()?.TakeDamage(this);
    }
}
