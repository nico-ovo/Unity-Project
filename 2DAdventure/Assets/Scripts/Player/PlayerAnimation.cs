using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    private PlayerControler playerControler;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck= GetComponent<PhysicsCheck>();
        playerControler = GetComponent<PlayerControler>();
    }

    private void Update()
    {
        SetAnimation();
        
    }

    public void SetAnimation()
    {
        anim.SetFloat("velocityX", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("velocityY", rb.velocity.y);
        anim.SetBool("isGround", physicsCheck.isGround);
        anim.SetBool("isCrouch", playerControler.isCrouch);
        anim.SetBool("isDead", playerControler.isDead);
        anim.SetBool("isAttack", playerControler.isAttack);
        anim.SetBool("onWall", physicsCheck.onWall);
        anim.SetBool("isSlide", playerControler.isSlide);
    }

    public void PlayerHurt()
    {
        anim.SetTrigger("hurt");
    }

    public void PlayAttack()
    {
        anim.SetTrigger("attack");
    }
}
