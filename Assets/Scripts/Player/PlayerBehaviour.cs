using System;
using System.Collections;
using UnityEngine;

public class PlayerBehaviour : StateMachineBase, IAttackListener
{
    #region Attribute

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

    [Space]

    [SerializeField] bool _isHurt;
    [SerializeField] bool _isDead;
    [SerializeField] bool _isGodMode;
    [SerializeField] bool _isCanDash = true;

    [Tooltip("이 각도를 초과한 경사에선 서있지 못함")]
    [SerializeField] float _slopeThreshold = 45f;

    [Header("Viewr")]
    [Space]

    [SerializeField] SkinnedMeshRenderer _capeRenderer;
    [SerializeField] Rigidbody2D _handRigidbody;
    [SerializeField] CapsuleCollider2D _bodyCollider;

    [Header("Blink / God Mode")]
    [Space]

    [SerializeField] Material _whiteMaterial;
    [SerializeField] float _godModeTime = 1.5f;
    [SerializeField] float _blinkDuration = 0.1f;

    SpriteRenderer[] _spriteRenderers;
    Material[] _originalMaterials;
    Coroutine _blinkRoutine;

    [Header("Effects")]
    [SerializeField] ParticleHelper _walkDustEmitter;
    [SerializeField] ParticleHelper _walkDirtEmitter;
    [SerializeField] ParticleHelper _landDustEmitter;
    [SerializeField] ParticleHelper _landDirtEmitter;
    [SerializeField] ParticleHelper _dashEffect;
    [SerializeField] ParticleHelper _dashTrailEffect;

    // Controller
    PlayerAttackController _attackController;
    InteractionController _interactionController;
    PlayerMovementController _movementController;

    // Sound List
    SoundList _soundList;

    #endregion

    #region Properties

    // Can Property
    public bool CanBasicAttack { get { return CurrentStateIs<IdleState>() || CurrentStateIs<RunState>() || CurrentStateIs<InAirState>(); } }
    public bool CanShootingAttack { get { return CurrentStateIs<IdleState>(); } }
    public bool CanDash { get { return _isCanDash; } set { _isCanDash = value; } }

    // Condition Property
    public bool IsGrounded { get { return GroundHit; } }
    public bool IsTouchedWall { get { return WallHit; } }
    public bool IsWallJump { get; set; }
    public bool IsInteractable { get { return CurrentStateIs<IdleState>() || CurrentStateIs<RunState>(); } }
    public bool IsHurt { get { return _isHurt; } set { _isHurt = value; } }
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
    public RaycastHit2D GroundHit { get; private set; }
    public RaycastHit2D UpwardGroundHit { get; set; }
    public RaycastHit2D WallHit { get; set; }
    public RaycastHit2D DiveHit { get; set; }
    public InteractionController InteractionController { get { return _interactionController; } }
    public PlayerMovementController MovementController { get { return _movementController; } }
    public Rigidbody2D HandRigidBody { get { return _handRigidbody; } }
    public CapsuleCollider2D BodyCollider { get { return _bodyCollider; } }
    public SoundList SoundList { get { return _soundList; } }
    public Material[] OriginalMaterials => _originalMaterials;

    #endregion

    #region Function

    protected override void Awake()
    {
        // Controller
        _attackController = GetComponent<PlayerAttackController>();
        _interactionController = GetComponent<InteractionController>();
        _movementController = GetComponent<PlayerMovementController>();

        // Collider
        _bodyCollider = GetComponent<CapsuleCollider2D>();

        SaveOriginalMaterial();

        // SoundList
        _soundList = GetComponent<SoundList>();
    }
    protected override void Start()
    {
        base.Start();

        // init player
        InitPlayer();

        // play bgm
        SoundManager.Instance.PlayCommonBGM("Exploration1", 0.3f);
    }
    protected override void Update()
    {
        base.Update();

        if (IsDead)
            return;

        #region Input

        if (InputManager.Instance.State.BasicAttackKey.KeyDown)
            OnBasicAttackPressed();
        if (InputManager.Instance.State.ShootingAttackKey.KeyDown)
            OnShootingAttackPressed();

        #endregion

        #region Check Ground & Wall

        // Check Ground
        GroundHit = Physics2D.CircleCast(_groundCheckTrans.position, _groundCheckRadius, Vector2.down, 0f, _groundLayer);

        // Check Upward
        UpwardGroundHit = Physics2D.Raycast(transform.position, Vector2.up, _upwardRayLength, _groundLayer);

        // Check Wall
        WallHit = Physics2D.Raycast(_wallCheckRayTrans.position, PlayerLookDir2D, _wallCheckRayLength, _wallLayer);

        // Check Dive Hit
        DiveHit = Physics2D.Raycast(_groundCheckTrans.position, Vector2.down, _diveCheckLength, _groundLayer);
        GroundDistance = _groundCheckTrans.position.y - DiveHit.point.y;

        #endregion

        #region Basic Behavior

        // Player Flip
        UpdateImageFlip();

        // Change In Air State
        ChangeInAirState();

        // Check Dead State
        CheckDie();

        #endregion

        #region Animaotr Parameter

        Animator.SetBool("IsGround", IsGrounded);
        Animator.SetFloat("AirSpeedY", Rigidbody.velocity.y);
        Animator.SetFloat("GroundDistance", GroundDistance);
        Animator.SetFloat("InputHorizontal", RawInputs.Movement.x);
        Animator.SetFloat("PlayerLookDirX", PlayerLookDir2D.x);
        Animator.SetBool("IsDirSync", IsDirSync);

        #endregion
    }

    // basic
    private void InitPlayer()
    {
        CurHp = _maxHp;
        RecentDir = Math.Sign(transform.localScale.x);
    }
    private void UpdateImageFlip()
    {
        if (CurrentStateIs<RunState>() || CurrentStateIs<InAirState>() || MovementController.isActiveAndEnabled)
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
            if(CurrentStateIs<IdleState>() || CurrentStateIs<RunState>() || CurrentStateIs<JumpState>())
                ChangeState<InAirState>();
        }
    }
    private void CheckDie()
    {
        if (CurHp <= 0)
        {
            CurHp = 0;
            ChangeState<DieState>();
        }
    }


    // key pressed event
    void OnBasicAttackPressed()
    {
        if (CanBasicAttack)
            _attackController.CastBasicAttack();
    }
    void OnShootingAttackPressed()
    {
        if (CanShootingAttack)
            _attackController.CastShootingAttack();
    }

    // about hit
    public void KnockBack(Vector2 forceVector)
    {
        Rigidbody.velocity = Vector2.zero;
        Rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
    }
    public IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        // fail return condition
        if (IsHurt || IsGodMode || IsDead)
            return IAttackListener.AttackResult.Fail;

        CurHp -= (int)attackInfo.Damage;
        PlaySound_SE_Hurt_02();

        // Change Die State
        if (CurHp <= 0)
        {
            CurHp = 0;
            ChangeState<DieState>();

            return IAttackListener.AttackResult.Success;
        }

        KnockBack(attackInfo.Force);

        // Change Hurt State
        ChangeState<HurtState>();

        return IAttackListener.AttackResult.Success;
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

    // blink
    private void SaveSpriteRenderers()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }
    private void SaveOriginalMaterial()
    {
        SaveSpriteRenderers();

        _originalMaterials = new Material[_spriteRenderers.Length];

        for (int i = 0; i < _originalMaterials.Length; i++)
            _originalMaterials[i] = _spriteRenderers[i].material;
    }
    private void InitMaterial()
    {
        for (int i = 0; i < _spriteRenderers.Length; i++)
            _spriteRenderers[i].material = _originalMaterials[i];
    }
    private IEnumerator Blink()
    {
        while (IsGodMode)
        {
            // turn to white material
            for (int i = 0; i < _originalMaterials.Length; i++)
                _spriteRenderers[i].material = _whiteMaterial;

            yield return new WaitForSeconds(_blinkDuration);

            // turn to original material
            for (int i = 0; i < _originalMaterials.Length; i++)
                _spriteRenderers[i].material = _originalMaterials[i];

            yield return new WaitForSeconds(_blinkDuration);
        }
    }
    public void StartBlink()
    {
        if (this._blinkRoutine != null)
        {
            InitMaterial();
            StopCoroutine(this._blinkRoutine);
        }
        this._blinkRoutine = StartCoroutine(Blink());
    }

    // god mode
    private IEnumerator GodModeTimer()
    {
        IsGodMode = true;
        yield return new WaitForSeconds(_godModeTime);
        IsGodMode = false;
    }
    public void StartGodMode()
    {
        StartCoroutine(GodModeTimer());
    }

    // respawn
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
        ChangeState<InstantRespawnState>(true);
        SceneContext.Current.InstantRespawn();
    }
    public void Alive()
    {
        _curHp = _maxHp;
    }

    // anim event
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

        // Draw Wall Check
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_wallCheckRayTrans.position, _wallCheckRayTrans.position + PlayerLookDir3D * _wallCheckRayLength);

        // Draw Upward Ray
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - UtilDefine.PaddingVector,
            transform.position - UtilDefine.PaddingVector + Vector3.up * _upwardRayLength);

        // Draw Dive Check
        Gizmos.color = Color.white;
        Gizmos.DrawLine(_groundCheckTrans.position + UtilDefine.PaddingVector,
            _groundCheckTrans.position + UtilDefine.PaddingVector + Vector3.down * _diveCheckLength);
    }
}