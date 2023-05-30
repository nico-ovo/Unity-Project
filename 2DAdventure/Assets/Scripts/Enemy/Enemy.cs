using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(PhysicsCheck))]

public class Enemy : MonoBehaviour
{
    #region 组件获取
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public PhysicsCheck physicscheck;
    #endregion

    [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    public float currentSpeed;
    public Vector3 faceDir;
    public float hurtForce;

    public Transform attacker;

    public Vector3 spwanPoint;

    [Header("检测")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;

    [Header("计时器")]
    public float waitTime;
    public float waitTimeCounter;
    public bool wait;

    public float lostTime;
    public float lostTimeCounter;

    [Header("状态")]
    public bool isHurt;
    public bool isDead;

    private BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected BaseState skillState;


    protected virtual void Awake()
    {
        #region 获取组件使用权
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicscheck = GetComponent<PhysicsCheck>();
        #endregion
        currentSpeed = normalSpeed;
        //waitTimeCounter = waitTime;

        spwanPoint = transform.position;
    }


    private void OnEnable()
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }


    private void Update()
    {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);
        

        currentState.LogicUpdate();
        TimeCounter();
    }

    private void FixedUpdate()
    {
        currentState.PhysicsUpdate();

        if (!isHurt && !isDead && !wait)
            Move();
    }

    private void OnDisable()
    {
        currentState.OnExit();
    }


    public virtual void Move()
    {
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("SnailPremove") && !anim.GetCurrentAnimatorStateInfo(0).IsName("SnailRecover"))
            rb.velocity = new Vector2(currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);
    }

    /// <summary>
    /// 计时器
    /// </summary>
    public void TimeCounter()
    {
        if(wait)
        {
            waitTimeCounter -= Time.deltaTime;
            if(waitTimeCounter <= 0)
            {
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(faceDir.x, 1 ,1);
            }
        }

        if(!FoundPlayer() && lostTimeCounter > 0)
        {
            lostTimeCounter -= Time.deltaTime;
        }
        else if(FoundPlayer())
        {
            lostTimeCounter = lostTime;
        }
    }

    public virtual bool FoundPlayer()
    {
        return  Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize,0, faceDir, checkDistance, attackLayer);
        
    }

    public void SwitchState(E_NPCState state)
    {
        var newState = state switch
        {
            E_NPCState.Patrol => patrolState,
            E_NPCState.Chase => chaseState,
            E_NPCState.Skill => skillState,
            _ => null
        };

        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }


    public virtual Vector3 GetNewPoint()
    {
        return transform.position;
    }

    #region 事件执行方法
    public void OnTakeDamage(Transform attackTrans)
    {
        attacker = attackTrans;
        //转身面朝玩家
        if (attackTrans.position.x - transform.position.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        if (attackTrans.position.x - transform.position.x < 0)
            transform.localScale = new Vector3(1, 1, 1);

        //击退效果
        isHurt = true;
        anim.SetTrigger("hurt");
        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x, 0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y);
        //用刚体的方法来表示受到了一个顺时的力
        //rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        //isHurt = false;
        StartCoroutine(OnHurt(dir));
    }

    private IEnumerator OnHurt(Vector2 dir)
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.6f);
        isHurt = false;
    }

    public void Ondie()
    {
        gameObject.layer = 2;
        anim.SetBool("isDead", true);
        isDead = true;
    }

    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }
    #endregion


    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset + new Vector3(checkDistance * -transform.localScale.x, 0, 0), 0.2f);
    }
}
