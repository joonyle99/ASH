using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : StateMachineBase
{
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Collider2D _groundCheckCollider;

    /// <summary>
    /// Smooth 효과로 전처리 된 InputState
    /// </summary>
    public InputState SmoothedInputs { get { return _inputPreprocessor.SmoothedInputs; } }
    /// <summary>
    /// InputManager.Instance.GetState() 와 동일
    /// </summary>
    public InputState RawInputs { get { return InputManager.Instance.GetState(); } }
    public bool IsGrounded { get; private set; }
    public int MaxJumpCount { get { return _jumpController.MaxJumpCount; } }

    public Rigidbody2D Rigidbody { get { return _rigidbody; } }


    PlayerJumpController _jumpController;
    DashState _dashState;
    PlayerInputPreprocessor _inputPreprocessor;
    Rigidbody2D _rigidbody;

    bool _isJumpQueued;
    float _timeAfterJumpQueued;
    public int recentDir = 1;


    private void Awake()
    {
        _inputPreprocessor = GetComponent<PlayerInputPreprocessor>();
        _jumpController = GetComponent<PlayerJumpController>();
        _dashState = GetComponent<DashState>();
        _rigidbody = GetComponent<Rigidbody2D>();
        
    }
    protected override void Start()
    {
        base.Start();
        InputManager.Instance.JumpPressedEvent += _jumpController.OnJumpPressed; //TODO : unsubscribe

    }

    protected override void Update()
    {
        base.Update();

        UpdateImageFlip();

        Animator.SetBool("Grounded", IsGrounded);
        Animator.SetFloat("AirSpeedY", Rigidbody.velocity.y);

        IsGrounded = _groundCheckCollider.IsTouchingLayers(_groundLayer);

        if(!IsGrounded) // TODO : 필요하다면 코요테 타임 동안은 InAir상태가 안되게 할지 결정
        {
            if (!StateIs<InAirState>() && !StateIs<DashState>())
                ChangeState<InAirState>();
        }

        // Dash Start
        if (Input.GetKeyDown(KeyCode.LeftShift) && _dashState.EnableDash)
        {
            if (!StateIs<DashState>())
                ChangeState<DashState>();
        }

        // 플레이어가 땅을 밟으면 EnableDash 활성화
        if(IsGrounded)
        {
            _dashState.EnableDash = true;
        }
    }

    private void UpdateImageFlip()
    {
        if (RawInputs.Movement.x != 0)
            recentDir = (int)RawInputs.Movement.x;
        transform.localScale = new Vector3(recentDir, transform.localScale.y, transform.localScale.z);
        //_anim.transform.rotation = left ? Quaternion.Euler(0, -90, 0) : Quaternion.Euler(0, 90, 0);
    }
}