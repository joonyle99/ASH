using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerBehaviour : StateMachineBase
{
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Collider2D _groundCheckCollider;

    [SerializeField] LayerMask _wallLayer;
    [SerializeField] Transform _wallCheckTrans;

    /// <summary>
    /// Smooth 효과로 전처리 된 InputState
    /// </summary>
    public InputState SmoothedInputs { get { return _inputPreprocessor.SmoothedInputs; } }
    /// <summary>
    /// InputManager.Instance.GetState() 와 동일
    /// </summary>
    public InputState RawInputs { get { return InputManager.Instance.GetState(); } }
    public bool IsGrounded { get; private set; }
    public bool IsTouchedWall { get { return _isTouchedWall; } private set { _isTouchedWall = value; } }
    public int MaxJumpCount { get { return _jumpController.MaxJumpCount; } }
    public Rigidbody2D Rigidbody { get { return _rigidbody; } }
    public int RecentDir { get { return _recentDir; } }

    PlayerJumpController _jumpController;
    DashState _dashState;
    PlayerInputPreprocessor _inputPreprocessor;
    Rigidbody2D _rigidbody;

    [SerializeField] private bool _isTouchedWall;
    [SerializeField] float _wallCheckDistance = 0.5f;

    bool _isJumpQueued;
    float _timeAfterJumpQueued;
    int _recentDir = 1;

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

        // Player Flip
        if (!StateIs<DashState>() && !StateIs<WallState>())
            UpdateImageFlip();

        // Animation
        Animator.SetBool("Grounded", IsGrounded);
        Animator.SetFloat("AirSpeedY", Rigidbody.velocity.y);

        // Check Ground / Wall
        IsGrounded = _groundCheckCollider.IsTouchingLayers(_groundLayer);
        IsTouchedWall = Physics2D.Raycast(_wallCheckTrans.position, Vector2.right * _recentDir, _wallCheckDistance, _wallLayer);

        if (!IsGrounded) // TODO : 필요하다면 코요테 타임 동안은 InAir상태가 안되게 할지 결정
        {
            if (!StateIs<InAirState>() && !StateIs<DashState>() && !StateIs<WallState>())
                ChangeState<InAirState>();
        }

        // Dash Start
        if (Input.GetKeyDown(KeyCode.V) && _dashState.EnableDash && RawInputs.Movement.x != 0 && !StateIs<WallState>())
        {
            if (!StateIs<DashState>())
                ChangeState<DashState>();
        }

        // Dash CoolTime
        if (!_dashState.Dashing && !_dashState.EnableDash)
        {
            if (Time.time >= _dashState.TimeEndedDash + _dashState.CoolTime)
            {
                if (IsGrounded)
                {
                    _dashState.EnableDash = true;
                }
            }
        }

        // Wall Jump
    }

    private void UpdateImageFlip()
    {
        if (RawInputs.Movement.x != 0)
            _recentDir = (int)RawInputs.Movement.x;
        transform.localScale = new Vector3(_recentDir, transform.localScale.y, transform.localScale.z);
        //_anim.transform.rotation = left ? Quaternion.Euler(0, -90, 0) : Quaternion.Euler(0, 90, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_wallCheckTrans.position, _wallCheckTrans.position + Vector3.right * _wallCheckDistance * _recentDir);
    }
}