using System;
using System.Collections;
using UnityEngine;

public class PlayerBehaviour : StateMachineBase, IAttackListener
{
    #region Attribute

    [Header("Player")]
    [Space]

    [SerializeField] private int _limitHp = 20;
    [SerializeField] private int _maxHp = 10;
    [SerializeField] private int _startHp = 5;
    [SerializeField] private int _curHp;

    [Space]

    [SerializeField] private bool _isHurt;
    [SerializeField] private bool _isGodMode;
    [SerializeField] private bool _isDead;

    [Space]

    [SerializeField] private bool _isCanAttack = true;
    [SerializeField] private bool _isCanDash = true;

    [Header("Effects")]
    [Space]

    [SerializeField] private ParticleHelper _walkDustEmitter;
    [SerializeField] private ParticleHelper _walkDirtEmitter;
    [SerializeField] private ParticleHelper _landDustEmitter;
    [SerializeField] private ParticleHelper _landDirtEmitter;
    [SerializeField] private ParticleHelper _dashEffect;
    [SerializeField] private ParticleHelper _dashTrailEffect;

    [Header("ETC")]
    [Space]

    [SerializeField] private CapsuleCollider2D _bodyCollider;
    [SerializeField] private MaterialController materialController;
    [SerializeField] private SoundList _soundList;
    [SerializeField] private Rigidbody2D _handRigidbody;

    // Controller
    private PlayerMovementController _playerMovementController;
    private PlayerAttackController _playerAttackController;
    private PlayerInteractionController playerInteractionController;
    private PlayerLightSkillController playerLightSkillController;
    private PlayerHeadAimController playerHeadAimController;

    #endregion

    #region Properties

    // Can Property
    public bool CanAttack
    {
        get => _isCanAttack && (CurrentState is IAttackableState) && (playerLightSkillController.IsLightButtonPressable && !playerLightSkillController.IsLightWorking);
        set => _isCanAttack = value;
    }
    public bool CanDash
    {
        get => _isCanDash && PersistentDataManager.Get<bool>("Dash");
        set => _isCanDash = value;
    }
    public bool CanInteract => CurrentState is IInteractableState;

    // Condition Property
    public bool IsGrounded => GroundHit;
    public bool IsTouchedWall => ClimbHit;
    public bool IsClimbable { get; set; }
    public bool IsClimbJump { get; set; }
    public bool IsHurt
    {
        get => _isHurt;
        set => _isHurt = value;
    }
    public bool IsGodMode
    {
        get => _isGodMode;
        set => _isGodMode = value;
    }
    public bool IsDead
    {
        get => _isDead;
        set => _isDead = value;
    }
    public int CurHp
    {
        get => _curHp;
        set
        {
            _curHp = value;

            if (_curHp > _maxHp) _curHp = _maxHp;   // 최대 체력을 넘어갈 수는 없다
            else if (_curHp < 0) _curHp = 0;        // 체력이 0 미만이 될 수는 없다
        }
    }
    public int MaxHp
    {
        get => _maxHp;
        set
        {
            _maxHp = value;

            if (_maxHp > _limitHp) _maxHp = _limitHp; // 최대 체력은 제한된다
            else if (_maxHp < 0) _maxHp = 0;          // 최대 체력은 0 미만이 될 수는 없다
        }
    }

    // Input Property
    public InputState RawInputs => InputManager.Instance.State;
    public bool IsMoveXKey => Math.Abs(RawInputs.Movement.x) > 0.01f;
    public bool IsMoveRightKey => RawInputs.Movement.x > 0.01f;
    public bool IsMoveLeftKey => RawInputs.Movement.x < -0.01f;
    public bool IsMoveYKey => Math.Abs(RawInputs.Movement.y) > 0.01f;
    public bool IsMoveUpKey => RawInputs.Movement.y > 0.01f;
    public bool IsMoveDownKey => RawInputs.Movement.y < -0.01f;

    // Direction Property
    public int RecentDir { get; set; }
    public bool IsDirSync => Mathf.Sign(PlayerLookDir2D.x * RawInputs.Movement.x) > 0.01f;
    public bool IsOppositeDirSync => Mathf.Sign(PlayerLookDir2D.x * RawInputs.Movement.x) < -0.01f;
    public Vector2 PlayerLookDir2D => new(RecentDir, 0f);
    public Vector3 PlayerLookDir3D => new(RecentDir, 0f, 0f);

    // RayCastHit
    public RaycastHit2D GroundHit { get; set; }
    public RaycastHit2D ClimbHit { get; set; }
    public RaycastHit2D UpwardGroundHit { get; set; }

    // Component
    public PlayerAttackController PlayerAttackController => _playerAttackController;
    public PlayerInteractionController PlayerInteractionController => playerInteractionController;
    public PlayerMovementController PlayerMovementController => _playerMovementController;

    // ETC
    public CapsuleCollider2D BodyCollider => _bodyCollider;
    public MaterialController MaterialController => materialController;
    public SoundList SoundList => _soundList;
    public Rigidbody2D HandRigidBody => _handRigidbody;

    #endregion

    #region Function

    protected override void Awake()
    {
        base.Awake();

        // Controller
        _playerAttackController = GetComponent<PlayerAttackController>();
        playerInteractionController = GetComponent<PlayerInteractionController>();
        _playerMovementController = GetComponent<PlayerMovementController>();
        playerLightSkillController = GetComponent<PlayerLightSkillController>();
        playerHeadAimController = GetComponent<PlayerHeadAimController>();

        // ETC
        _bodyCollider = GetComponent<CapsuleCollider2D>();
        materialController = GetComponent<MaterialController>();
        _soundList = GetComponent<SoundList>();
    }
    protected override void Start()
    {
        base.Start();

        // init player
        InitPlayer();
    }
    protected override void Update()
    {
        base.Update();

        if (IsDead)
            return;

        #region Input

        if (InputManager.Instance.State.AttackKey.KeyDown && CanAttack)
            _playerAttackController.CastAttack();

        #endregion

        #region Basic Behavior

        // Player Flip
        UpdateImageFlip();

        // Change In Air State
        ChangeInAirState();

        // Check Dead State
        if (_curHp <= 0)
        {
            ChangeState<DieState>();
            return;
        }

        #endregion

        #region Animaotr Parameter

        Animator.SetBool("IsGround", IsGrounded);
        Animator.SetFloat("AirSpeedY", Rigidbody.velocity.y);
        Animator.SetBool("IsDirSync", IsDirSync);

        #endregion
    }

    // basic
    private void InitPlayer()
    {
        // 체력 초기화
        _curHp = _startHp;

        // 바라보는 방향 설정
        RecentDir = Math.Sign(transform.localScale.x);
    }
    private void UpdateImageFlip()
    {
        if (CurrentStateIs<RunState>() || CurrentStateIs<InAirState>())
        {
            if (IsOppositeDirSync && IsMoveXKey)
            {
                RecentDir = (int)RawInputs.Movement.x;
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * RecentDir, transform.localScale.y, transform.localScale.z);

                playerHeadAimController.HeadAimControlOnFlip();
            }
        }
    }
    private void ChangeInAirState()
    {
        if (!IsGrounded)
        {
            if (CurrentStateIs<IdleState>() || CurrentStateIs<RunState>() || CurrentStateIs<JumpState>())
                ChangeState<InAirState>();
        }
    }

    // about hit
    public IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        // fail return condition
        if (IsHurt || IsGodMode || IsDead)
            return IAttackListener.AttackResult.Fail;

        PlaySound_SE_Hurt_02();
        StartCoroutine(SlowMotionCoroutine(0.3f));

        TakeDamage((int)attackInfo.Damage);
        // Change Die State
        if (_curHp <= 0)
            return IAttackListener.AttackResult.Success;

        KnockBack(attackInfo.Force);

        // Change Hurt State
        ChangeState<HurtState>();

        return IAttackListener.AttackResult.Success;
    }
    private void TakeDamage(float damage)
    {
        _curHp -= (int)damage;
        if (_curHp <= 0)
        {
            _curHp = 0;
            ChangeState<DieState>();
        }
    }
    public void KnockBack(Vector2 forceVector)
    {
        Rigidbody.velocity = Vector2.zero;
        Rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
    }
    private IEnumerator SlowMotionCoroutine(float duration)
    {
        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    // about health
    public void IncreaseMaxHp(int amount)
    {
        MaxHp += amount;
    }
    public void RecoverCurHp(int amount)
    {
        CurHp += amount;
    }


    // respawn
    public void TriggerInstantRespawn(float damage)
    {
        TakeDamage(damage);
        if (CurHp > 0)
            InstantRespawn();
    }
    public void InstantRespawn()
    {
        ChangeState<InstantRespawnState>(true);
        SceneContext.Current.InstantRespawn();
    }

    // anim
    public void FinishState_AnimEvent()
    {
        ChangeState<IdleState>();
    }

    #endregion

    #region Sound

    public void PlaySound_SE_Run()
    {
        _soundList.PlaySFX("SE_Run");

        _walkDustEmitter.Emit(1);
        _walkDirtEmitter.Emit(UnityEngine.Random.Range(0, 3));
    }
    public void PlaySound_SE_Jump_01()
    {
        _soundList.PlaySFX("SE_Jump_01");
    }
    public void PlaySound_SE_Jump_02()
    {
        _soundList.PlaySFX("SE_Jump_02");

        _landDustEmitter.Emit(2);
        _landDirtEmitter.Emit(7);
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

        _dashEffect.Emit(1);
        _dashTrailEffect.Emit(1);
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

    public void PlaySound(string key)
    {
        _soundList.PlaySFX(key);
    }

    #endregion
}