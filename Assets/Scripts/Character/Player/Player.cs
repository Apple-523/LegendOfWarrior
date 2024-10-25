using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("移动速度")]
    public float normalSpeed;
    public float runSpeed;
    [Header("跳起速度")]
    public float jumpSpeed;

    [Header("攻击状态")]
    [SerializeField]
    private bool isAttack;

    /// <summary>
    /// 输入系统
    /// </summary>
    private PlayerInputSystem playerInputSystem;
    private Rigidbody2D rigidbody2d;


    /// <summary>
    /// 输入的方向
    /// </summary>
    private Vector2 inputDirection;
    private PhysicsCheckEventHandler pcEventHandler;
    private PlayerEventHandler playerEventHandler;


    private bool isOnGround;



    #region 生命周期
    private void Awake()
    {
        isAttack = false;
        playerInputSystem = new PlayerInputSystem();
        rigidbody2d = GetComponent<Rigidbody2D>();
        pcEventHandler = GetComponentInChildren<PhysicsCheckEventHandler>();
        playerEventHandler = GetComponentInChildren<PlayerEventHandler>();
        
    }
    private void OnEnable()
    {
        playerInputSystem.Enable();
        playerInputSystem.Player.Jump.started += onClickJump;
        pcEventHandler.OnCharacterGroundChange += OnPlayerGroundChange;
        pcEventHandler.OnCharacterByWall += OnPlayerByWall;
        playerEventHandler.OnCharacterEndAttack += OnPlayerEndAttack;
        playerInputSystem.Player.Fire.started += OnPlayerFire;
    }

    private void OnDisable()
    {
        playerInputSystem.Disable();
        playerInputSystem.Player.Jump.started -= onClickJump;
        pcEventHandler.OnCharacterGroundChange -= OnPlayerGroundChange;
        pcEventHandler.OnCharacterByWall -= OnPlayerByWall;
        playerEventHandler.OnCharacterEndAttack -= OnPlayerEndAttack;
        playerInputSystem.Player.Fire.started -= OnPlayerFire;
    }


    private void Update()
    {

    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    #endregion


    private void onClickJump(InputAction.CallbackContext context)
    {
        if (isOnGround)
        {
            rigidbody2d.AddForce(transform.up * jumpSpeed, ForceMode2D.Impulse);
            playerEventHandler.CharacterBeginJump(false);
        }
    }



    /// <summary>
    /// 控制人物移动
    /// </summary>
    private void PlayerMove()
    {
        inputDirection = playerInputSystem.Player.Move.ReadValue<Vector2>();
        // if (inputDirection.x < float.Epsilon && inputDirection.y < float.Epsilon) {
        //     // 没有输入时不动
        //     return;
        // }
        //TODO: wmy 走和跑的转换 在键盘按下shift时，进行强制走路
        Vector2 speedVector = new Vector2(inputDirection.x * runSpeed, rigidbody2d.velocity.y);
        rigidbody2d.velocity = speedVector;
        Vector3 localScale = transform.localScale;
        if (inputDirection.x > 0)
        {
            localScale.x = 1;
        }
        if (inputDirection.x < 0)
        {
            localScale.x = -1;
        }
        transform.localScale = localScale;
    }

    #region 事件接收
    private void OnPlayerGroundChange(object sender, bool e)
    {
        isOnGround = e;
    }

    private void OnPlayerByWall(object sender, EventBOOLArgs args)
    {
        if (args.AtLeastOneTrue())
        {
            Vector2 speedVector = new Vector2(0, rigidbody2d.velocity.y);
            rigidbody2d.velocity = speedVector;
        }
    }

    private void OnPlayerFire(InputAction.CallbackContext context)
    {
        isAttack = true;
        playerEventHandler.CharacterPressAttack(isAttack);

    }

    private void OnPlayerEndAttack(object sender, bool isAttack)
    {
        this.isAttack = isAttack;
    }
    #endregion
}
