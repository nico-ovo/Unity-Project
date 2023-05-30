using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    //获取组件
    private CapsuleCollider2D coll;

    private PlayerControler playerControler;

    private Rigidbody2D rb;
    [Header("检测参数")]

    public bool manual;
    public bool isPlayer;

    //脚底位移差值
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;

    public float checkRaduis;

    public LayerMask groundLayer;

    [Header("状态")]
    public bool isGround;
    public bool touchLeftWall;
    public bool touchRightWall;
    public bool onWall;


    private void Awake()
    {
        //获取组件使用权限
        coll = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (!manual)
        {
            rightOffset = new Vector2((coll.bounds.size.x + coll.offset.x) / 2, coll.bounds.size.y / 2);
            leftOffset = new Vector2(-rightOffset.x, rightOffset.y);
        }

        if(isPlayer)
        {
            playerControler = GetComponent<PlayerControler>();
        }
    }

    private void Update()
    {
        Check();
    }

    //public void Check()
    //{
    //    //检测地面
    //    isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRaduis, groundLayer);


    //    //墙体判断
    //    touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRaduis, groundLayer);
    //    touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRaduis, groundLayer);
    //}

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRaduis);
    //    Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRaduis);
    //    Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRaduis);
    //}

    public void Check()
    {
        //检测地面
        if(onWall)
        {
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRaduis, groundLayer);
        }
        else
        {
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, 0), checkRaduis, groundLayer);
        }
        

        //墙体判断
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(leftOffset.x, leftOffset.y), checkRaduis, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(rightOffset.x, rightOffset.y), checkRaduis, groundLayer);
        if(isPlayer)
        {
            onWall = (touchLeftWall && playerControler.inputDirection.x < 0f || touchRightWall && playerControler.inputDirection.x > 0f) && rb.velocity.y < 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(leftOffset.x, leftOffset.y), checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(rightOffset.x, rightOffset.y), checkRaduis);
    }

}
