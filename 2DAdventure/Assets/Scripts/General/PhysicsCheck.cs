using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    //��ȡ���
    private CapsuleCollider2D coll;

    private PlayerControler playerControler;

    private Rigidbody2D rb;
    [Header("������")]

    public bool manual;
    public bool isPlayer;

    //�ŵ�λ�Ʋ�ֵ
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;

    public float checkRaduis;

    public LayerMask groundLayer;

    [Header("״̬")]
    public bool isGround;
    public bool touchLeftWall;
    public bool touchRightWall;
    public bool onWall;


    private void Awake()
    {
        //��ȡ���ʹ��Ȩ��
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
    //    //������
    //    isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRaduis, groundLayer);


    //    //ǽ���ж�
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
        //������
        if(onWall)
        {
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRaduis, groundLayer);
        }
        else
        {
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, 0), checkRaduis, groundLayer);
        }
        

        //ǽ���ж�
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
