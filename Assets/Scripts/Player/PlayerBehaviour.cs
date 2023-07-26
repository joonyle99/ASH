using System.Collections;
using UnityEngine;

public class PlayerBehaviour : StateMachineBase
{
    [Header("Check Params")]

    [Space]

    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Transform _groundCheckTrans;
    [SerializeField] LayerMask _wallLayer;
    [SerializeField] Transform _wallCheckTrans;

    [Header("Check Distance")]

    [Space]

    [Range(0f, 5f)] [SerializeField] float _groundCheckDistance = 0.1f;
    [Range(0f, 30f)] [SerializeField] float _diveCheckDistance = 15f;
    [Range(0f, 5f)] [SerializeField] float _wallCheckDistance = 0.2f;

    [Header("Dive Settings")]

    [Space]

    [Range(0f, 10f)] [SerializeField] float _diveThreshhold = 4f;


    [Header("Player Settings")]

    [Space]

    [Range(0f, 200f)] [SerializeField] float _hp = 100f;

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

    public bool CanDash { get; set; }

    public RaycastHit2D GroundHit { get; set; }
    public RaycastHit2D DiveHit { get; set; }
    public RaycastHit2D WallHit { get; set; }

    public InputState RawInputs { get { return InputManager.Instance.GetState(); } }
    public InputState SmoothedInputs { get { return _inputPreprocessor.SmoothedInputs; } }
    public InteractionController InteractionController { get { return _interactionController; } }   // InputManager.Instance.GetState() 와 동일

    public int RecentDir { get; set; }
    public Vector2 PlayerLookDir { get { return new Vector2(RecentDir, 0); } }

    public bool IsWallJump { get; set; }
    public float GroundDistance { get; set; }
    public float DiveThreshhold
    {
        get { return _diveThreshhold; }
        private set { _diveThreshhold = value; }
    }

    public float HP
    {
        get { return _hp;}
        set { _hp = value; }
    }

    #endregion

    private void Awake()
    {
        // Controller
        _jumpController = GetComponent<PlayerJumpController>();
        _attackController = GetComponent<PlayerAttackController>();
        _interactionController = GetComponent<InteractionController>();

        // InputPreProcessor
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
        // InputManager.Instance.JumpPressedEvent -= _jumpController.OnJumpPressed; //TODO : unsubscribe
        // InputManager.Instance.BasicAttackPressedEvent -= OnBasicAttackPressed; //TODO : unsubscribe
        // InputManager.Instance.HealingPressedEvent -= OnHealingPressed; //TODO : unsubscribe
        // InputManager.Instance.ShootingAttackPressedEvent -= OnShootingAttackPressed; //TODO : unsubscribe
    }

    protected override void Update()
    {
        base.Update();

        #region Animaotr Parameter

        Animator.SetBool("IsGround", IsGrounded);
        Animator.SetFloat("AirSpeedY", Rigidbody.velocity.y);
        Animator.SetFloat("GroundDistance", GroundDistance);

        #endregion

        #region Basic Behavior

        // Player Flip
        if (StateIs<RunState>() || StateIs<InAirState>())
        {
            if (Mathf.RoundToInt(RawInputs.Movement.x) != 0 && RecentDir != Mathf.RoundToInt(RawInputs.Movement.x))
                UpdateImageFlip();
        }

        // In Air State
        if (!IsGrounded && !StateIs<InAirState>())
        {
            if (!StateIs<WallState>() && !StateIs<DashState>() && !StateIs<DiveState>() && !StateIs<ShootingState>())
                ChangeState<InAirState>();
        }

        #endregion

        #region Check Ground & Wall

        // Check Ground
        GroundHit = Physics2D.Raycast(_groundCheckTrans.position, Vector2.down, _groundCheckDistance, _groundLayer);

        if (GroundHit)
            IsGrounded = true;
        else
            IsGrounded = false;

        // Check Wall
        WallHit = Physics2D.Raycast(_wallCheckTrans.position, Vector2.right * RecentDir, _wallCheckDistance, _wallLayer);

        if (WallHit)
            IsTouchedWall = true;
        else
            IsTouchedWall = false;

        // Check Dive Hit
        DiveHit = Physics2D.Raycast(_groundCheckTrans.position, Vector2.down, _diveCheckDistance, _groundLayer);

        // Ground Distance
        GroundDistance = _groundCheckTrans.position.y - DiveHit.point.y;

        #endregion

        #region Skill CoolTime

        // Dash CoolTime
        if (!_dashState.IsDashing)
        {
            if (Time.time >= _dashState.TimeEndedDash + _dashState.CoolTime)
            {
                if (IsGrounded || StateIs<WallState>())
                    CanDash = true;
            }
        }

        // Dive CoolTime

        // Shooting CoolTime

        #endregion
    }

    private void UpdateImageFlip()
    {
        RecentDir = (int)RawInputs.Movement.x;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * RecentDir, transform.localScale.y, transform.localScale.z);
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

    public void OnHitbyPuddle(float damage, Vector3 spawnPoint, float reviveDelay)
    {
        Debug.Log("물 웅덩이에 닿음 ");
        //애니메이션, 체력 닳기 등 하면 됨.
        //애니메이션 종료 후 spawnpoint에서 생성

        //TEMP
        StartCoroutine(ReviveAfterPuddleHitCoroutine(reviveDelay));
        transform.position = spawnPoint;
    }
    IEnumerator ReviveAfterPuddleHitCoroutine(float reviveDelay)
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(reviveDelay);
        gameObject.SetActive(true);
    }
    public void OnDamage(int _damage)
    {

    }

    public void KnockBack(Vector2 vec)
    {
        // Rigidbody.AddForce(vec);
    }

    // TODO : 달리기 사운드 Loop 재생
    public void PlaySound_SE_Run()
    {
        GetComponent<SoundList>().PlaySFX("SE_Run");
    }

    // TODO : 점프 액션 사운드 Once 재생
    public void PlaySound_SE_Jump_01()
    {
        GetComponent<SoundList>().PlaySFX("SE_Jump_01");
    }

    // TODO : 점프 마무리 사운드 Once 재생
    public void PlaySound_SE_Jump_02()
    {
        GetComponent<SoundList>().PlaySFX("SE_Jump_02");
    }

    // TODO : 이단 점프 사운드 Once 재생
    public void PlaySound_SE_DoubleJump()
    {
        GetComponent<SoundList>().PlaySFX("SE_DoubleJump");
    }

    // TODO : 기본 공격 사운드 Once 재생
    public void PlaySound_SE_Attack()
    {
        GetComponent<SoundList>().PlaySFX("SE_Attack");
    }

    // TODO : 급강하 액션 사운드 Loop 재생?
    public void PlaySound_SE_DesolateDive_01()
    {
        GetComponent<SoundList>().PlaySFX("SE_DesolateDive_01");
    }

    // TODO : 급강하 마무리 사운드 Once 재생
    public void PlaySound_SE_DesolateDive_02()
    {
        GetComponent<SoundList>().PlaySFX("SE_DesolateDive_02");
    }

    // TODO : 발사 액션 사운드 Once 재생
    public void PlaySound_SE_Shooting_01()
    {
        GetComponent<SoundList>().PlaySFX("SE_Shooting_01");
    }

    // TODO : 발사 마무리 사운드 Once 재생
    public void PlaySound_SE_Shooting_02()
    {
        GetComponent<SoundList>().PlaySFX("SE_Shooting_02");
    }


    private void OnDrawGizmosSelected()
    {
        // Draw Wall Check
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_wallCheckTrans.position, _wallCheckTrans.position + Vector3.right * _wallCheckDistance * RecentDir);

        // Draw Ground Check
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_groundCheckTrans.position, _groundCheckTrans.position + Vector3.down * _groundCheckDistance);

        // Draw Dive Check
        Gizmos.color = Color.white;
        Gizmos.DrawLine(_groundCheckTrans.position + new Vector3(0.1f, 0),
            _groundCheckTrans.position + new Vector3(0.1f, -_diveCheckDistance));
    }
}