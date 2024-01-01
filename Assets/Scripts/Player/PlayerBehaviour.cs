using System;
using System.Collections;
using UnityEngine;

public class PlayerBehaviour : StateMachineBase
{
    [Header("Ground Check")]
    [Space]

    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Transform _groundCheckTrans;
    [SerializeField] float _groundCheckRadius;
    [SerializeField] float _groundCheckLength;

    [Header("Wall Check")]
    [Space]

    [SerializeField] LayerMask _wallLayer;
    [SerializeField] Transform _wallCheckRayTrans;
    [SerializeField] float _wallCheckRayLength;
    [SerializeField] float _upwardRayLength;

    [Header("Dive Check")]
    [Space]

    [SerializeField] float _diveCheckLength;
    [SerializeField] float _diveThreshholdHeight;

    [Header("Player")]
    [Space]

    [SerializeField] int _maxHp;
    [SerializeField] int _curHp;
    [SerializeField] bool _isHurtable = true;
    [SerializeField] bool _isDead;
    [SerializeField] bool _isGodMode;
    [SerializeField] bool _isCanDash = true;

    [Tooltip("이 각도를 초과한 경사에선 서있지 못함")]
    [SerializeField] float _slopeThreshold = 45f;

    [Header("Viewr")]
    [Space]

    [SerializeField] Collider2D _groundHitCollider;
    [SerializeField] Collider2D _wallHitCollider;
    [SerializeField] Collider2D _DiveHitCollider;
    [SerializeField] Collider2D _mainCollider;
    [SerializeField] SkinnedMeshRenderer _capeRenderer;
    [SerializeField] Rigidbody2D _hand;

    // Controller
    PlayerJumpController _jumpController;
    PlayerAttackController _attackController;
    InteractionController _interactionController;
    PlayerMovementController _movementController;

    //Joint for interactable
    Joint2D _joint;

    // Sound List
    SoundList _soundList;

    // Padding Vector
    readonly Vector3 _paddingVec = new Vector3(0.1f, 0f, 0f);

    #region Properties

    // Can Property
    public bool CanBasicAttack { get { return StateIs<IdleState>() || StateIs<RunState>() || StateIs<InAirState>(); } }
    public bool CanShootingAttack { get { return StateIs<IdleState>(); } }
    public bool CanDash { get { return _isCanDash; } set { _isCanDash = value; } }

    // Condition Property
    public bool IsGrounded { get { return GroundHit; } }
    public bool IsTouchedWall { get { return WallHit; } }
    public bool IsWallJump { get; set; }
    public bool IsInteractable { get { return StateIs<IdleState>() || StateIs<RunState>(); } }
    public bool IsHurtable { get { return _isHurtable; } set { _isHurtable = value; } }
    public bool IsDead { get { return _isDead; } set { _isDead = value; } }
    public bool IsGodMode { get { return _isGodMode; } set { _isGodMode = value; } }
    public int CurHp { get { return _curHp; } set { _curHp = value; } }

    // Check Property
    public float GroundDistance { get; set; }
    public float DiveThreshholdHeight { get { return _diveThreshholdHeight; } private set { _diveThreshholdHeight = value; } }
    public float SlopeThreshold { get { return _slopeThreshold; } }

    // Input Property
    public InputState RawInputs { get { return InputManager.Instance.State; } }
    public bool IsMoveXKey { get { return Math.Abs(RawInputs.Movement.x) > 0.01f; } }
    public bool IsMoveRightKey { get { return RawInputs.Movement.x > 0.01f; } }
    public bool IsMoveLeftKey { get { return RawInputs.Movement.x < -0.01f; } }
    public bool IsMoveYKey { get { return Math.Abs(RawInputs.Movement.y) > 0.01f; } }
    public bool IsMoveUpKey { get { return RawInputs.Movement.y > 0.01f; } }
    public bool IsMoveDownKey { get { return RawInputs.Movement.y < -0.01f; } }

    // Direction Property
    public int RecentDir { get; set; }
    public bool IsDirSync { get { return Mathf.Abs(PlayerLookDir2D.x - RawInputs.Movement.x) < 0.01f; } }
    public bool IsOppositeDirSync { get { return Mathf.Abs(PlayerLookDir2D.x + RawInputs.Movement.x) < 0.01f; } }
    public Vector2 PlayerLookDir2D { get { return new Vector2(RecentDir, 0f); } }
    public Vector3 PlayerLookDir3D { get { return new Vector3(RecentDir, 0f, 0f); } }

    // Etc
    public LayerMask GroundLayerMask { get { return _groundLayer; } }
    public Collider2D MainCollider { get { return _mainCollider; } }
    public RaycastHit2D GroundHit { get; private set; }
    public RaycastHit2D UpwardGroundHit { get; set; }
    public RaycastHit2D WallHit { get; set; }
    public RaycastHit2D DiveHit { get; set; }
    public InteractionController InteractionController { get { return _interactionController; } }
    public PlayerMovementController MovementController { get { return _movementController; } }
    public Rigidbody2D HandRigidBody { get { return _hand; } }
    public SoundList SoundList { get { return _soundList; } }

    #endregion

    private void Awake()
    {
        // Collider
        _mainCollider = GetComponent<Collider2D>();

        // Controller
        _jumpController = GetComponent<PlayerJumpController>();
        _attackController = GetComponent<PlayerAttackController>();
        _interactionController = GetComponent<InteractionController>();
        _movementController = GetComponent<PlayerMovementController>();

        // SoundList
        _soundList = GetComponent<SoundList>();
    }

    private void OnEnable()
    {
        RecentDir = 1;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * RecentDir, transform.localScale.y, transform.localScale.z);
    }


    protected override void Start()
    {
        base.Start();

        // 배경 BGM 출력
        SoundManager.Instance.PlayCommonBGM("Exploration1", 0.3f);

        CurHp = _maxHp;
    }

    protected override void Update()
    {
        base.Update();


        if (InputManager.Instance.State.BasicAttackKey.KeyDown)
            OnBasicAttackPressed();
        if (InputManager.Instance.State.ShootingAttackKey.KeyDown)
            OnShootingAttackPressed();

        #region Animaotr Parameter

        Animator.SetBool("IsGround", IsGrounded);
        Animator.SetFloat("AirSpeedY", Rigidbody.velocity.y);
        Animator.SetFloat("GroundDistance", GroundDistance);
        Animator.SetFloat("InputHorizontal", RawInputs.Movement.x);
        Animator.SetFloat("PlayerLookDirX", PlayerLookDir2D.x);
        Animator.SetBool("IsDirSync", IsDirSync);

        #endregion

        #region Basic Behavior

        // Player Flip
        UpdateImageFlip();

        // Change In Air State
        ChangeInAirState();

        // Check Dead State
        CheckDieState();

        #endregion

        #region Check Ground & Wall

        // Check Ground
        GroundHit = Physics2D.CircleCast(_groundCheckTrans.position, _groundCheckRadius, Vector2.down, 0f, _groundLayer);
        _groundHitCollider = GroundHit.collider;

        // Check Upward
        UpwardGroundHit = Physics2D.Raycast(transform.position, Vector2.up, _upwardRayLength, _groundLayer);

        // Check Wall
        WallHit = Physics2D.Raycast(_wallCheckRayTrans.position, PlayerLookDir2D, _wallCheckRayLength, _wallLayer);
        _wallHitCollider = WallHit.collider;

        // Check Dive Hit
        DiveHit = Physics2D.Raycast(_groundCheckTrans.position, Vector2.down, _diveCheckLength, _groundLayer);
        GroundDistance = _groundCheckTrans.position.y - DiveHit.point.y;
        _DiveHitCollider = DiveHit.collider;

        #endregion
    }

    private void UpdateImageFlip()
    {
        if (StateIs<RunState>() || StateIs<InAirState>() || MovementController.isActiveAndEnabled)
        {
            if (!IsDirSync && Mathf.Abs(RawInputs.Movement.x) > 0.01f)
            {
                RecentDir = (int)RawInputs.Movement.x;
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * RecentDir, transform.localScale.y, transform.localScale.z);
            }
        }
    }
    private void ChangeInAirState()
    {
        if (!IsGrounded)
        {
            if(StateIs<IdleState>() || StateIs<RunState>() || StateIs<JumpState>())
                ChangeState<InAirState>();
        }
    }
    private void CheckDieState()
    {
        if (CurHp <= 0 && !IsDead)
        {
            CurHp = 0;
            ChangeState<DieState>();
        }
    }

    public void DisableHorizontalMovement()
    {
        Rigidbody.constraints |= RigidbodyConstraints2D.FreezePositionX;
    }
    public void EnableHorizontalMovement()
    {
        Rigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
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

    public void KnockBack(Vector2 forceVector)
    {
        Rigidbody.velocity = Vector2.zero;
        Rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
    }
    public void OnHit(int damage, Vector2 forceVector)
    {
        // 무적이거나 사망 상태라면 OnHit return
        if (IsGodMode || IsDead)
            return;

        // 피격 가능한 불가능한 상태이면 OnHit return
        if (!IsHurtable)
            return;

        // Debug.Log("In OnHit()");

        // Apply Damage
        CurHp -= damage;

        // Player Hurt Sound
        PlaySound_SE_Hurt_02();

        // Change Die State
        if (CurHp <= 0)
        {
            // Debug.Log("Apply Die");

            CurHp = 0;
            ChangeState<DieState>();

            return;
        }

        // Debug.Log("Apply Hurt");

        // Apply Knock Back
        KnockBack(forceVector);

        // Change Hurt State
        ChangeState<HurtState>();
    }
    public void OnHitbyPuddle(float damage)
    {
        //애니메이션, 체력 닳기 등 하면 됨.
        //애니메이션 종료 후 spawnpoint에서 생성

        if (CurHp == 1)
        {
            CurHp = _maxHp;
        }
        else
        {
            CurHp -= 1;
        }

        InstantRespawn();
    }
    public void OnHitByPhysicalObject(float damage, Rigidbody2D other)
    {
        // TODO

        Debug.Log(damage + " 데미지 입음");
    }

    public void TriggerInstantRespawn(float damage)
    {
        if (CurHp == 1)
            CurHp = _maxHp;
        else
            CurHp -= 1;

        InstantRespawn();
    }
    public void InstantRespawn()
    {
        //this.gameObject.SetActive(false);
        ChangeState<InstantRespawnState>(true);
        SceneContext.Current.InstantRespawn();
    }

    #region Sound

    public void PlaySound_SE_Run()
    {
        _soundList.PlaySFX("SE_Run");
    }

    public void PlaySound_SE_Jump_01()
    {
        _soundList.PlaySFX("SE_Jump_01");
    }

    public void PlaySound_SE_Jump_02()
    {
        _soundList.PlaySFX("SE_Jump_02");
    }

    public void PlaySound_SE_DoubleJump()
    {
        _soundList.PlaySFX("SE_DoubleJump");
    }

    public void PlaySound_SE_Attack()
    {
        _soundList.PlaySFX("SE_Attack");
    }

    public void PlaySound_SE_Dash()
    {
        _soundList.PlaySFX("SE_Dash");
    }

    public void PlaySound_SE_DesolateDive_01()
    {
        _soundList.PlaySFX("SE_DesolateDive_01");
    }

    public void PlaySound_SE_DesolateDive_02()
    {
        _soundList.PlaySFX("SE_DesolateDive_02");
    }

    public void PlaySound_SE_Shooting_01()
    {
        _soundList.PlaySFX("SE_Shooting_01");
    }

    public void PlaySound_SE_Shooting_02()
    {
        _soundList.PlaySFX("SE_Shooting_02");
    }

    public void PlaySound_SE_Hurt_01()
    {
        // _soundList.PlaySFX("SE_Hurt_01");
    }

    public void PlaySound_SE_Hurt_02()
    {
        _soundList.PlaySFX("SE_Hurt_02");
    }

    public void PlaySound_SE_Die_01()
    {
        _soundList.PlaySFX("SE_Die_01(long)");
    }

    public void PlaySound_SE_Die_02()
    {
        _soundList.PlaySFX("SE_Die_02");
    }

    public void PlaySound_SE_Healing_01()
    {
        _soundList.PlaySFX("SE_Healing_01");
    }

    public void PlaySound_SE_Healing_02()
    {
        _soundList.PlaySFX("SE_Healing_02");
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        // Draw Ground Check
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_groundCheckTrans.position, _groundCheckRadius);

        /*
        // Draw Ground Check With RayCast
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_groundCheckTrans.position,
            _groundCheckTrans.position + Vector3.down * _groundCheckLength);
        */

        // Draw Upward Ray
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - _paddingVec,
            transform.position - _paddingVec + Vector3.up * _upwardRayLength);

        // Draw Wall Check
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_wallCheckRayTrans.position, _wallCheckRayTrans.position + PlayerLookDir3D * _wallCheckRayLength);

        // Draw Dive Check
        Gizmos.color = Color.white;
        Gizmos.DrawLine(_groundCheckTrans.position + _paddingVec,
            _groundCheckTrans.position + _paddingVec + Vector3.down * _diveCheckLength);
    }
}