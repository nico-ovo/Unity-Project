using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using UnityEngine.Rendering;

public class PlayerControler : MonoBehaviour
{
    [Header("�����¼�")]
    public SceneLoadEventSO sceneLoadEvent;
    public VoidEventSO afterSceneLoaded;
    public VoidEventSO loadDataEvent;
    public VoidEventSO backToMenuEvent;

    //�������
    public PlayerInputControl inputControl;
    //����Ч��
    public Rigidbody2D rb;
    public PhysicsCheck physicsCheck;
    public CapsuleCollider2D coll;
    //�豸����ֵ(x,y)
    public Vector2 inputDirection;

    private PlayerAnimation playerAnimation;

    private Character character;
    [Header("��������")]
    //�ٶ�
    public float speed;
    //�ܲ��ٶ�
    private float runSpeed;
    //�����ٶ�
    private float walkSpeed => speed / 2.5f;


    //��Ծ��
    public float jumpForce;
    //��ǽ������
    public float wallJumpForce;
    //���˻ص�����
    public float hurtForce;
    //��������
    public float slideDistance;
    //�����ٶ�
    public float slideSpeed;
    //���λ�����������
    public int slidePowerCost;

    private Vector2 originalOffset;
    private Vector2 originalSize;

    [Header("�������")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    [Header("״̬")]
    //�¶�״̬
    public bool isCrouch;
    public bool isHurt;
    public bool isDead;
    public bool isAttack;
    public bool wallJump;
    public bool isSlide;
    
    private void Awake()
    {
        //������Ч��
        rb = GetComponent<Rigidbody2D>();
        physicsCheck= GetComponent<PhysicsCheck>();
        coll = GetComponent<CapsuleCollider2D>();
        playerAnimation= GetComponent<PlayerAnimation>();
        character = GetComponent<Character>();

        originalOffset = coll.offset;
        originalSize = coll.size;

        inputControl = new PlayerInputControl();

        #region ��Ծ
        //��Jump�ķ�����ӵ��������µ���һ��ִ��
        inputControl.GamePlay.Jump.started += Jump;
        #endregion

        #region ǿ����·
        runSpeed = speed;
        //performed��������סʱ
        inputControl.GamePlay.Walk.performed += ctx => 
        {
            if (physicsCheck.isGround)
                speed = walkSpeed;
        };
        //canceled �����ɿ�ʱ
        inputControl.GamePlay.Walk.canceled += ctx =>
        {
            if (physicsCheck.isGround)
                speed = runSpeed;
        };
        #endregion

        #region ����
        inputControl.GamePlay.Attack.started += PlayerAttack;
        #endregion

        #region ����
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


    //����
    //private void OnTriggerStay2D(Collider2D other)
    //{
    //    Debug.Log(other.name);
    //}

    /// <summary>
    /// ��������ֹͣ����
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    private void OnLoadEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        inputControl.GamePlay.Disable();
    }
    /// <summary>
    /// ��ȡ��Ϸ����
    /// </summary>
    private void OnLoadDataEvent()
    {
        isDead = false;
    }

    /// <summary>
    /// ���������������
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnAfterSceneLoadedEvent()
    {
        inputControl.GamePlay.Enable();
    }

    public void Move()
    {
        //�����ƶ�
        //�¶�ʱ�����ƶ�
        if(!isCrouch && !wallJump && !isSlide)
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);
        //�����泯����
        int faceDir = (int)transform.localScale.x;
        //�ж����뷽��Ϊ������
        if (inputDirection.x > 0)
            faceDir = 1;
        if (inputDirection.x < 0)
            faceDir = -1;

        //���﷭ת
        transform.localScale = new Vector3(faceDir, 1, 1);

        //�¶�
        isCrouch = inputDirection.y < -0.5f  && physicsCheck.isGround;
        if (isCrouch)
        {
            //�޸���ײ���С��λ��
            coll.offset = new Vector2(-0.05f, 0.85f);
            coll.size = new Vector2(0.70f, 1.70f);
        }
        else
        {
            //��ԭ֮ǰ����ײ�����
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
            //������Ծ��Ч
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
            //��Э��
            StartCoroutine(TriggerSlide(targetPos));

            character.OnSlide(slidePowerCost);
        }
        
    }
    //Э�̷���
    private IEnumerator TriggerSlide(Vector3 target)
    {
        do
        {
            yield return null;
            //��ǰ��������ʱֹͣЭ��
            if (!physicsCheck.isGround)
                break;

            //�����Ĺ�����ײǽͣ��
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
        //���˺�isHurt��ʾ�ܵ��˺���ͬʱ�ٶȹ���
        isHurt = true;
        rb.velocity = Vector2.zero;
        //�������˺�ص�
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;
        //�����ܵ�һ��˲ʱ����
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }


    public void PlayerDead()
    {
        isDead = true;
        //��Ϸ��������
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

