using UnityEngine;

public class PlayerBehaviour : StateMachineBase
{
    [Header("Check Params")]

    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Transform _groundCheckTrans;
    [SerializeField] LayerMask _wallLayer;
    [SerializeField] Transform _wallCheckTrans;

    [Header("Check Distance")]

    [SerializeField] float _groundCheckDistance = 0.1f;
    [SerializeField] float _diveCheckDistance = 15f;
    [SerializeField] float _wallCheckDistance = 0.2f;

    [Header("Wall Settings")]

    [SerializeField] bool _isWallJump;

    [Header("Dive Settings")]
    [SerializeField] float _groundDistance;
    [SerializeField] float _diveThreshhold = 4f;

    [Header("Player Direction")]

    [SerializeField] int _recentDir = 1;

    // Controller
    PlayerJumpController _jumpController;
    PlayerAttackController _attackController;
    InteractionController _interactionController;

    PlayerInputPreprocessor _inputPreprocessor;

    // State
    DashState _dashState;
    DiveState _diveState;
    ShootingState _shootingState;

    #region Properties

    public bool IsGrounded { get; set; }
    public bool IsTouchedWall { get; set; }

    public bool CanBasicAttack { get { return StateIs<IdleState>() || StateIs<RunState>() || StateIs<InAirState>(); } }
    public bool CanHealing { get { return StateIs<IdleState>(); } }
    public bool CanShootingAttack { get { return StateIs<IdleState>(); } }

    public RaycastHit2D GroundHit { get; set; }
    public RaycastHit2D DiveHit { get; set; }
    public RaycastHit2D WallHit { get; set; }

    public InputState RawInputs { get { return InputManager.Instance.GetState(); } }
    public InputState SmoothedInputs { get { return _inputPreprocessor.SmoothedInputs; } }          // Smooth 효과로 전처리 된 InputState
    public InteractionController InteractionController { get { return _interactionController; } }   // InputManager.Instance.GetState() 와 동일

    public int RecentDir { get { return _recentDir; } set { _recentDir = value; } }
    public Vector2 PlayerLookDir { get { return new Vector2(RecentDir, 0); } }

    public bool IsWallJump { get { return _isWallJump; } set { _isWallJump = value; } }

    #endregion

    private void Awake()
    {
        // Controller
        _jumpController = GetComponent<PlayerJumpController>();
        _attackController = GetComponent<PlayerAttackController>();
        _interactionController = GetComponent<InteractionController>();

        _inputPreprocessor = GetComponent<PlayerInputPreprocessor>();

        // State
        _dashState = GetComponent<DashState>();
        _diveState = GetComponent<DiveState>();
        _shootingState = GetComponent<ShootingState>();
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

        // Animaotr Parameter
        Animator.SetBool("IsGround", IsGrounded);
        Animator.SetFloat("AirSpeedY", Rigidbody.velocity.y);
        Animator.SetFloat("GroundDistane", _groundDistance);

        // Player Flip
        if (StateIs<RunState>() || StateIs<InAirState>())
        {
            if (Mathf.RoundToInt(RawInputs.Movement.x) != 0 && _recentDir != Mathf.RoundToInt(RawInputs.Movement.x))
                UpdateImageFlip();
        }

        #region Check Ground & Wall

        // Check Ground
        GroundHit = Physics2D.Raycast(_groundCheckTrans.position, Vector2.down, _groundCheckDistance, _groundLayer);

        if (GroundHit)
            IsGrounded = true;
        else
            IsGrounded = false;

        // Check Wall
        WallHit = Physics2D.Raycast(_wallCheckTrans.position, Vector2.right * _recentDir, _wallCheckDistance, _wallLayer);

        if (WallHit)
            IsTouchedWall = true;
        else
            IsTouchedWall = false;

        // Check Dive Hit
        DiveHit = Physics2D.Raycast(_groundCheckTrans.position, Vector2.down, _diveCheckDistance, _groundLayer);

        // Ground Distance
        _groundDistance = _groundCheckTrans.position.y - DiveHit.point.y;

        #endregion

        #region Skill CoolTime

        // Dash CoolTime
        if (!_dashState.IsDashing && !_dashState.EnableDash)
        {
            if (Time.time >= _dashState.TimeEndedDash + _dashState.CoolTime)
            {
                if (IsGrounded || StateIs<WallState>())
                    _dashState.EnableDash = true;
            }
        }

        // Dive CoolTime

        // Shooting CoolTime

        #endregion

        #region Change State

        // In Air State
        if (!IsGrounded && !StateIs<InAirState>())
        {
            if (!StateIs<DashState>() && !StateIs<WallState>() && !StateIs<DiveState>() && !StateIs<ShootingState>())
                ChangeState<InAirState>();
        }

        // Dash State
        if (Input.GetKeyDown(KeyCode.V) && !StateIs<DashState>())
        {
            if (_dashState.EnableDash && Mathf.RoundToInt(RawInputs.Movement.x) != 0)
            {
                if (StateIs<RunState>() || StateIs<InAirState>())
                    ChangeState<DashState>();
            }
        }

        // Dive State
        if (Input.GetKeyDown(KeyCode.Alpha5) && RawInputs.Movement.y < 0)
        {
            if (StateIs<InAirState>() && _groundDistance > _diveThreshhold)
                ChangeState<DiveState>();
        }

        #endregion

    }

    private void UpdateImageFlip()
    {
        _recentDir = (int)RawInputs.Movement.x;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * _recentDir, transform.localScale.y, transform.localScale.z);
    }

    void OnBasicAttackPressed()
    {
        if (CanBasicAttack)
            _attackController.CastBasicAttack();
    }
    void OnHealingPressed()
    {

    }
    void OnShootingAttackPressed()
    {
        if (CanShootingAttack)
            _attackController.CastShootingAttack();
    }

    public void OnHitbyWater(float damage, Vector3 spawnPoint)
    {
        Debug.Log("물 웅덩이에 닿음 ");
        //애니메이션, 체력 닳기 등 하면 됨.
        //애니메이션 종료 후 spawnpoint에서 생성

        //TEMP
        transform.position = spawnPoint;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw Wall Check
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_wallCheckTrans.position, _wallCheckTrans.position + Vector3.right * _wallCheckDistance * _recentDir);

        // Draw Ground Check
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_groundCheckTrans.position, _groundCheckTrans.position + Vector3.down * _groundCheckDistance);

        // Draw Dive Check
        Gizmos.color = Color.white;
        Gizmos.DrawLine(_groundCheckTrans.position + new Vector3(0.1f, 0),
            _groundCheckTrans.position + new Vector3(0.1f, -_diveCheckDistance));
    }
}