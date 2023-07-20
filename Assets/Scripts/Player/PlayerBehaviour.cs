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

    [SerializeField] float _wallCheckDistance = 0.8f;
    [SerializeField] bool _isWallJump;

    [Header("Dive Settings")]

    [SerializeField] float _groundCheckDistance = 0.3f;
    [SerializeField] float _diveCheckDistance = 50f;
    [SerializeField] float _groundDistance;
    [SerializeField] float _diveThreshhold = 2.0f;

    [Header("Player Direction")]

    [SerializeField] int _recentDir = 1;



    PlayerJumpController _jumpController;
    PlayerAttackController _attackController;
    InteractionController _interactionController;
    PlayerInputPreprocessor _inputPreprocessor;
    DashState _dashState;


#region Properties

    public bool IsGrounded { get; set; }
    public bool IsTouchedWall { get; set; }

    public bool CanBasicAttack { get { return StateIs<IdleState>() || StateIs<RunState>() || StateIs<InAirState>(); } }
    public bool CanHealing { get { return StateIs<IdleState>(); } }
    public bool CanShootingAttack { get { return StateIs<IdleState>(); } }

    public RaycastHit2D GroundHit { get; private set; }
    public RaycastHit2D DiveHit { get; private set; }
    public RaycastHit2D WallHit { get; private set; }

    public InputState RawInputs { get { return InputManager.Instance.GetState(); } }
    public int RecentDir { get { return _recentDir; } set { _recentDir = value; } }
    public Vector2 PlayerLookDir { get { return new Vector2(RecentDir, 0); } }

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

#region Check Ground & Wall

        // Check Ground
        GroundHit = Physics2D.Raycast(_groundCheckTrans.position, Vector2.down, _groundCheckDistance, _groundLayer);

        // Check Dive Hit
        DiveHit = Physics2D.Raycast(_groundCheckTrans.position, Vector2.down, _diveCheckDistance, _groundLayer);

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

        // Ground Distance
        _groundDistance = _groundCheckTrans.position.y - DiveHit.point.y;

        #endregion

        // Animation Param
        Animator.SetBool("IsGround", IsGrounded);
        Animator.SetFloat("AirSpeedY", Rigidbody.velocity.y);

        // Player Flip
        if (StateIs<RunState>() || StateIs<InAirState>())
        {
            // 좌 & 우 방향키가 입력되므로 Flip
            if (Mathf.RoundToInt(RawInputs.Movement.x) != 0)
                UpdateImageFlip();
        }

        // TODO : 여기 if문 조건 줄여보기
        // In Air State
        if (!IsGrounded)
        {
            if (!StateIs<InAirState>() && !StateIs<DashState>() && !StateIs<WallState>() && !StateIs<DiveState>() && !StateIs<ShootingState>())
                ChangeState<InAirState>();
        }

        // Dash State
        if (Input.GetKeyDown(KeyCode.V) && _dashState.EnableDash && Mathf.RoundToInt(RawInputs.Movement.x) != 0)
        {
            if (!StateIs<WallState>() && !StateIs<DiveState>() && !StateIs<ShootingState>())
            {
                if (!StateIs<DashState>())
                    ChangeState<DashState>();
            }
        }

        // TODO : 쿨타임 관리 시스템 만들기
        // Dash CoolTime
        if (!_dashState.IsDashing && !_dashState.EnableDash)
        {
            if (Time.time >= _dashState.TimeEndedDash + _dashState.CoolTime)
            {
                if (IsGrounded || StateIs<WallState>())
                    _dashState.EnableDash = true;
            }
        }

        // Desolate Dive State
        if (Input.GetKeyDown(KeyCode.Alpha5) && RawInputs.Movement.y < 0)
        {
            if(StateIs<InAirState>() && _groundDistance > _diveThreshhold)
                ChangeState<DiveState>();
        }

        // TODO : 쿨타임 관리 시스템 만들기
        // Desolate Dive CoolTime
        if (!_dashState.IsDashing && !_dashState.EnableDash)
        {
            if (Time.time >= _dashState.TimeEndedDash + _dashState.CoolTime)
            {
                if (IsGrounded || StateIs<WallState>())
                    _dashState.EnableDash = true;
            }
        }
    }

    private void UpdateImageFlip()
    {
        _recentDir = (int)RawInputs.Movement.x;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * _recentDir, transform.localScale.y, transform.localScale.z);
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