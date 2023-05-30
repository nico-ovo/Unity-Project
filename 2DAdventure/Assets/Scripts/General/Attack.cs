using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("攻击属性")]
    //伤害
    public int damage;
    //攻击范围
    public float attackRange;
    //攻击频率
    public float attackRate;

    private void OnTriggerStay2D(Collider2D other)
    {
        //通过oher来访问被攻击对象
        //如果对象身上没有这个代码就不会执行takedamage，以免报错
        other.GetComponent<Character>()?.TakeDamage(this);
    }
}
