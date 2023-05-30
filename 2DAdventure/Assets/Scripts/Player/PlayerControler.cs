using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using UnityEngine.Rendering;

public class PlayerControler : MonoBehaviour
{
    [Header("监听事件")]
    public SceneLoadEventSO sceneLoadEvent;
    public VoidEventSO afterSceneLoaded;
    public VoidEventSO loadDataEvent;
    public VoidEventSO backToMenuEvent;

    //输入控制
    public PlayerInputControl inputControl;
    //刚体效果
    public Rigidbody2D rb;
    public PhysicsCheck physicsCheck;
    public CapsuleCollider2D coll;
    //设备输入值(x,y)
    public Vector2 inputDirection;

    private PlayerAnimation playerAnimation;

    private Character character;
    [Header("基本参数")]
    //速度
    public float speed;
    //跑步速度
    private float runSpeed;
    //行走速度
    private float walkSpeed => speed / 2.5f;


    //跳跃力
    public float jumpForce;
    //蹬墙跳的力
    public float wallJumpForce;
    //受伤回弹的力
    public float hurtForce;
    //滑铲距离
    public float slideDistance;
    //滑铲速度
    public float slideSpeed;
    //单次滑铲能量消耗
    public int slidePowerCost;

    private Vector2 originalOffset;
    private Vector2 originalSize;

    [Header("物理材质")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    [Header("状态")]
    //下蹲状态
    public bool isCrouch;
    public bool isHurt;
    public bool isDead;
    public bool isAttack;
    public bool wallJump;
    public bool isSlide;
    
    private void Awake()
    {
        //获得组件效果
        rb = GetComponent<Rigidbody2D>();
        physicsCheck= GetComponent<PhysicsCheck>();
        coll = GetComponent<CapsuleCollider2D>();
        playerAnimation= GetComponent<PlayerAnimation>();
        character = GetComponent<Character>();

        originalOffset = coll.offset;
        originalSize = coll.size;

        inputControl = new PlayerInputControl();

        #region 跳跃
        //把Jump的方法添加到按键按下的那一刻执行
        inputControl.GamePlay.Jump.started += Jump;
        #endregion

        #region 强制走路
        runSpeed = speed;
        //performed代表按键按住时
        inputControl.GamePlay.Walk.performed += ctx => 
        {
            if (physicsCheck.isGround)
                speed = walkSpeed;
        };
        //canceled 按键松开时
        inputControl.GamePlay.Walk.canceled += ctx =>
        {
            if (physicsCheck.isGround)
                speed = runSpeed;
        };
        #endregion

        #region 攻击
        inputControl.GamePlay.Attack.started += PlayerAttack;
        #endregion

        #region 滑铲
        inputControl.GamePlay.Slide.started += Slide;
        #endregion

        inputControl.Enable();
    }


    private void OnEnable()
    {
        sceneLoadEvent.LoadRequestEvent += OnLoadEvent;
        afterSceneLoaded.OnEventRaised += OnAfterSceneLoadedEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;
    }

    private void OnDisable()
    {
        inputControl.Disable();
        sceneLoadEvent.LoadRequestEvent -= OnLoadEvent;
        afterSceneLoaded.OnEventRaised -= OnAfterSceneLoadedEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;
    }


    private void Update()
    {
        inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();

        CheckState();
    }

    private void FixedUpdate()
    {
        if(!isHurt && !isAttack)
        Move();
    }


    //测试
    //private void OnTriggerStay2D(Collider2D other)
    //{
    //    Debug.Log(other.name);
    //}

    /// <summary>
    /// 场景加载停止控制
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    private void OnLoadEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        inputControl.GamePlay.Disable();
    }
    /// <summary>
    /// 读取游戏进度
    /// </summary>
    private void OnLoadDataEvent()
    {
        isDead = false;
    }

    /// <summary>
    /// 加载完成启动控制
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnAfterSceneLoadedEvent()
    {
        inputControl.GamePlay.Enable();
    }

    public void Move()
    {
        //人物移动
        //下蹲时不能移动
        if(!isCrouch && !wallJump && !isSlide)
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);
        //人物面朝方向
        int faceDir = (int)transform.localScale.x;
        //判断输入方向为左还是右
        if (inputDirection.x > 0)
            faceDir = 1;
        if (inputDirection.x < 0)
            faceDir = -1;

        //人物翻转
        transform.localScale = new Vector3(faceDir, 1, 1);

        //下蹲
        isCrouch = inputDirection.y < -0.5f  && physicsCheck.isGround;
        if (isCrouch)
        {
            //修改碰撞体大小和位移
            coll.offset = new Vector2(-0.05f, 0.85f);
            coll.size = new Vector2(0.70f, 1.70f);
        }
        else
        {
            //还原之前的碰撞体参数
            coll.offset = originalOffset;
            coll.size = originalSize;
        }
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if (physicsCheck.isGround)
        {
            //Debug.Log("JUMP");
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            //播放跳跃音效
            GetComponent<AudioDefination>()?.PlayAudioClip();

            isSlide = false;
            StopAllCoroutines();
        }
        else if (physicsCheck.onWall)
        {
            rb.AddForce(new Vector2(-inputDirection.x, 2.5f) * wallJumpForce, ForceMode2D.Impulse);

            wallJump = true;

            GetComponent<AudioDefination>()?.PlayAudioClip();
        }
    }

    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        playerAnimation.PlayAttack();
        isAttack = true;

    }

    private void Slide(InputAction.CallbackContext obj)
    {
        if(!isSlide && physicsCheck.isGround && character.currentPower >= slidePowerCost)
        {
            isSlide = true;

            var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x, transform.position.y);

            gameObject.layer = LayerMask.NameToLayer("Enemy");
            //打开协程
            StartCoroutine(TriggerSlide(targetPos));

            character.OnSlide(slidePowerCost);
        }
        
    }
    //协程方法
    private IEnumerator TriggerSlide(Vector3 target)
    {
        do
        {
            yield return null;
            //当前面是悬崖时停止协程
            if (!physicsCheck.isGround)
                break;

            //滑动的过程中撞墙停下
            if(physicsCheck.touchLeftWall &&transform.localScale.x < 0f || physicsCheck.touchRightWall && transform.localScale.x > 0f)
            {
                isSlide = false;
                break;
            }

            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed, transform.position.y));
        } while (MathF.Abs(target.x - transform.position.x) > 0.1f);

        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }



    #region UnityEvent
    public void GetHurt(Transform attacker)
    {
        //受伤后isHurt表示受到伤害，同时速度归零
        isHurt = true;
        rb.velocity = Vector2.zero;
        //人物受伤后回弹
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;
        //刚体受到一个瞬时的力
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }


    public void PlayerDead()
    {
        isDead = true;
        //游戏操作结束
        inputControl.GamePlay.Disable();
    }
    #endregion

    private void CheckState()
    {
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;

        if(physicsCheck.onWall)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }

        if(wallJump && rb.velocity.y < 0f)
        {
            wallJump = false;
        }

    }
}

