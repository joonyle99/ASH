using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : StateMachineBase
{
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Collider2D _groundCheckCollider;
    [SerializeField] LayerMask _wallLayer;
    [SerializeField] Collider2D _wallCheckCollider;

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
    public bool IsTouchedLWall { get { return _isTouchedLWall; } private set { _isTouchedLWall = value; } }
    public bool IsTouchedRWall { get { return _isTouchedRWall; } private set { _isTouchedRWall = value; } }
    public int MaxJumpCount { get { return _jumpController.MaxJumpCount; } }
    public Rigidbody2D Rigidbody { get { return _rigidbody; } }
    public Vector2 RecentDir { get { return new Vector2(_recentDir, 0); } }

    PlayerJumpController _jumpController;
    DashState _dashState;
    PlayerInputPreprocessor _inputPreprocessor;
    Rigidbody2D _rigidbody;

    [SerializeField] private bool _isTouchedWall;
    [SerializeField] private bool _isTouchedLWall;
    [SerializeField] private bool _isTouchedRWall;

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

        UpdateImageFlip();

        Animator.SetBool("Grounded", IsGrounded);
        Animator.SetFloat("AirSpeedY", Rigidbody.velocity.y);

        // Check Ground / Wall
        IsGrounded = _groundCheckCollider.IsTouchingLayers(_groundLayer);
        IsTouchedWall = _wallCheckCollider.IsTouchingLayers(_wallLayer);

        // Check Left or Right Wall
        if(IsTouchedWall)
        {
            if(_recentDir == 1) // 플레이어 방향이 오른쪽
            {
                IsTouchedRWall = true;
            }
            else if(_recentDir == -1) // 왼쪽
            {
                IsTouchedLWall = true;
            }
        }
        else
        {
            IsTouchedRWall = false;
            IsTouchedLWall = false;
        }

        if (!IsGrounded) // TODO : 필요하다면 코요테 타임 동안은 InAir상태가 안되게 할지 결정
        {
            if (!StateIs<InAirState>() && !StateIs<DashState>() && !StateIs<WallSlideState>())
                ChangeState<InAirState>();
        }

        // Dash Start
        if (Input.GetKeyDown(KeyCode.V) && _dashState.EnableDash && RawInputs.Movement.x != 0)
        {
            if (!StateIs<DashState>())
                ChangeState<DashState>();
        }

        // Dash CoolTime
        if(!_dashState.Dashing && !_dashState.EnableDash)
        {
            if(Time.time >= _dashState.TimeEndedDash + _dashState.CoolTime)
            {
                if (IsGrounded)
                {
                    _dashState.EnableDash = true;
                }
            }
        }
    }

    private void UpdateImageFlip()
    {
        if (RawInputs.Movement.x != 0)
            _recentDir = (int)RawInputs.Movement.x;
        transform.localScale = new Vector3(_recentDir, transform.localScale.y, transform.localScale.z);
        //_anim.transform.rotation = left ? Quaternion.Euler(0, -90, 0) : Quaternion.Euler(0, 90, 0);
    }
}