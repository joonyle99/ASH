using UnityEngine;

public class PlayerBehaviour : StateMachineBase
{
    [Header("Check Params")]

    // ground
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Transform _groundCheckTrans;

    // wall
    [SerializeField] LayerMask _wallLayer;
    [SerializeField] Transform _wallCheckTrans;


    [Header("Wall Settings")]

    [SerializeField] float _wallCheckDistance = 0.7f;
    [SerializeField] bool _isWallJump;
    [SerializeField] bool _isTouchedWall;

    [Header("Dive Settings")]

    [SerializeField] float _groundCheckDistance = 0.6f;
    [SerializeField] float _groundDistance;
    [SerializeField] float _diveThreshhold = 2.0f;

    // controllers
    PlayerJumpController _jumpController;
    PlayerAttackController _attackController;
    InteractionController _interactionController;

    PlayerInputPreprocessor _inputPreprocessor;

    DashState _dashState;

    int _recentDir = 1;

    #region Properties

    public bool IsGrounded { get; private set; }
    public bool IsTouchedWall { get { return _isTouchedWall; } private set { _isTouchedWall = value; } }

    public bool CanBasicAttack { get { return StateIs<IdleState>() || StateIs<WalkState>() || StateIs<InAirState>(); } }
    public bool CanHealing { get { return StateIs<IdleState>(); } }
    public bool CanShootingAttack { get { return StateIs<IdleState>(); } }

    public RaycastHit2D GroundHit { get; private set; }
    public RaycastHit2D WallHit { get; private set; }

    public InputState RawInputs { get { return InputManager.Instance.GetState(); } }
    public int RecentDir { get { return _recentDir; } set { _recentDir = value; } }
    public Vector2 PlayerLookDir { get { return new Vector2(RecentDir, 0); } }

    public int MaxJumpCount { get { return _jumpController.MaxJumpCount; } }
    public bool IsWallJump { get { return _isWallJump; } set { _isWallJump = value; } }

    public InputState SmoothedInputs { get { return _inputPreprocessor.SmoothedInputs; } }          // Smooth 효과로 전처리 된 InputState
    public InteractionController InteractionController { get { return _interactionController; } }   // InputManager.Instance.GetState() 와 동일

    #endregion

    private void Awake()
    {
        _jumpController = GetComponent<PlayerJumpController>();
        _attackController = GetComponent<PlayerAttackController>();
        _interactionController = GetComponent<InteractionController>();

        _inputPreprocessor = GetComponent<PlayerInputPreprocessor>();

        _dashState = GetComponent<DashState>();

    }
    protected override void Start()
    {
        base.Start();
        InputManager.Instance.JumpPressedEvent += _jumpController.OnJumpPressed; //TODO : subscribe
        InputManager.Instance.BasicAttackPressedEvent += OnBasicAttackPressed; //TODO : subscribe
        InputManager.Instance.HealingPressedEvent += OnHealingPressed; //TODO : subscribe
        InputManager.Instance.ShootingAttackPressedEvent += OnShootingAttackPressed; //TODO : subscribe
    }
    private void OnDestroy()
    {
        InputManager.Instance.JumpPressedEvent -= _jumpController.OnJumpPressed; //TODO : unsubscribe
        InputManager.Instance.BasicAttackPressedEvent -= OnBasicAttackPressed; //TODO : unsubscribe
        InputManager.Instance.HealingPressedEvent -= OnHealingPressed; //TODO : unsubscribe
        InputManager.Instance.ShootingAttackPressedEvent -= OnShootingAttackPressed; //TODO : unsubscribe
    }

    protected override void Update()
    {
        base.Update();

        // Player Flip
        if (!StateIs<DashState>() && !StateIs<WallState>())
            UpdateImageFlip();

        // Animation Param
        Animator.SetBool("Grounded", IsGrounded);
        Animator.SetFloat("AirSpeedY", Rigidbody.velocity.y);

        // Check Ground
        GroundHit = Physics2D.Raycast(_groundCheckTrans.position, Vector2.down, _groundCheckDistance, _groundLayer);
        if (GroundHit)
            IsGrounded = true;
        else
            IsGrounded = false;

        // Check Wall
        WallHit = Physics2D.Raycast(_wallCheckTrans.position, Vector2.right * _recentDir, _wallCheckDistance, _wallLayer);
        if (WallHit)
        {
            // Debug.Log("Player.WallHit.collider.name : " + WallHit.transform.gameObject.name);
            IsTouchedWall = true;
        }
        else
            IsTouchedWall = false;

        // Ground Distance
        _groundDistance = _groundCheckTrans.position.y - GroundHit.point.y;

        // In Air State
        if (!IsGrounded)
        {
            if (!StateIs<InAirState>() && !StateIs<DashState>() && !StateIs<WallState>() && !StateIs<DesolateDiveState>())
                ChangeState<InAirState>();
        }

        // Dash State
        if (Input.GetKeyDown(KeyCode.V) && _dashState.EnableDash && RawInputs.Movement.x != 0)
        {
            if(!StateIs<WallState>() && !StateIs<DesolateDiveState>())
            {
                if (!StateIs<DashState>())
                    ChangeState<DashState>();
            }
        }

        // Dash CoolTime
        if (!_dashState.Dashing && !_dashState.EnableDash)
        {
            if (Time.time >= _dashState.TimeEndedDash + _dashState.CoolTime)
            {
                if (IsGrounded || StateIs<WallState>())
                    _dashState.EnableDash = true;
            }
        }

        // Desolate Dive State
        // 1. Jump Height > 2.0f
        // 2. When not in DashState
        // 3. InAirState -> DiveStatee
        if (Input.GetKeyDown(KeyCode.Alpha5) && RawInputs.Movement.y < 0)
        {
            if(StateIs<InAirState>() && !StateIs<DashState>() && _groundDistance > _diveThreshhold)
                ChangeState<DesolateDiveState>();
        }
    }

    private void UpdateImageFlip()
    {
        // Input이 없을 때는 방향을 유지
        if (RawInputs.Movement.x != 0)
            _recentDir = (int)RawInputs.Movement.x;

        transform.localScale = new Vector3(_recentDir, transform.localScale.y, transform.localScale.z);
    }



    void OnBasicAttackPressed()
    {
        if (CanBasicAttack)
            CastBasicAttack();
    }
    void OnHealingPressed()
    {
        if (CanBasicAttack)
            CastHealing();
    }
    void OnShootingAttackPressed()
    {
        if (CanShootingAttack)
            CastShootingAttack();
    }



    void CastBasicAttack()
    {
        _attackController.CastBasicAttack();
    }
    void CastHealing()
    {

    }
    void CastShootingAttack()
    {
        _attackController.CastShootingAttack();
    }



    private void OnDrawGizmos()
    {
        // Draw Ground Check
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_wallCheckTrans.position, _wallCheckTrans.position + Vector3.right * _wallCheckDistance * _recentDir);

        // Draw Wall Check
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_groundCheckTrans.position, _groundCheckTrans.position + Vector3.down * _groundCheckDistance);
    }
}