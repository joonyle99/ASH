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
    [SerializeField] Transform _groundAboveCheckTrans;
    [SerializeField] float _groundCheckRadius;
    [SerializeField] float _groundAboveCheckLength;

    [Header("Climb Check")]
    [Space]

    [SerializeField] LayerMask _climbLayer;
    [SerializeField] Transform _climbCheckTrans;
    [SerializeField] float _climbCheckLength;

    [Header("Player")]
    [Space]

    [SerializeField] int _maxHp;
    [SerializeField] int _curHp;

    [Space]

    [SerializeField] bool _isHurt;
    [SerializeField] bool _isDead;
    [SerializeField] bool _isGodMode;

    [Space]

    [SerializeField] bool _isCanBasicAttack = true;
    [SerializeField] bool _isCanJump = true;
    [SerializeField] bool _isCanDash = true;

    [Header("Viewr")]
    [Space]

    [SerializeField] CapsuleCollider2D _bodyCollider;
    [SerializeField] Rigidbody2D _handRigidbody;

    [Header("White Flash")]
    [Space]

    [SerializeField] Material _whiteFlashMaterial;
    [SerializeField] float _godModeTime = 1.5f;
    [SerializeField] float _flashInterval = 0.06f;

    [SerializeField] SpriteRenderer[] _spriteRenderers;
    Material[] _originalMaterials;
    Coroutine _whiteFlashRoutine;

    [Header("Effects")]
    [Space]

    [SerializeField] ParticleHelper _walkDustEmitter;
    [SerializeField] ParticleHelper _walkDirtEmitter;
    [SerializeField] ParticleHelper _landDustEmitter;
    [SerializeField] ParticleHelper _landDirtEmitter;
    [SerializeField] ParticleHelper _dashEffect;
    [SerializeField] ParticleHelper _dashTrailEffect;

    // Controller
    PlayerAttackController _playerAttackController;
    InteractionController _interactionController;
    PlayerMovementController _playerMovementController;
    LightController _lightController;

    // Sound List
    SoundList _soundList;

    #endregion

    #region Properties

    // Can Property
    public bool CanBasicAttack { get { return _isCanBasicAttack && (CurrentStateIs<IdleState>() || CurrentStateIs<RunState>() || CurrentStateIs<InAirState>()) && (_lightController.IsLightButtonPressable && !_lightController.IsLightWorking); } set { _isCanBasicAttack = value; } }
    public bool CanDash { get { return _isCanDash && PersistentDataManager.Get<bool>("Dash"); } set { _isCanDash = value; } }
    public bool CanInteract { get { return CurrentStateIs<IdleState>() || CurrentStateIs<RunState>(); } }

    // Condition Property
    public bool IsGrounded { get { return GroundHit; } }
    public bool IsTouchedWall { get { return ClimbHit; } }
    public bool IsClimbable { get; set; }
    public bool IsClimbJump { get; set; }
    public bool IsHurt { get { return _isHurt; } set { _isHurt = value; } }
    public bool IsDead { get { return _isDead; } set { _isDead = value; } }
    public bool IsGodMode { get { return _isGodMode; } set { _isGodMode = value; } }
    public int CurHp { get { return _curHp; } }

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
    public bool IsDirSync { get { return Mathf.Sign(PlayerLookDir2D.x * RawInputs.Movement.x) > 0.01f; } }
    public bool IsOppositeDirSync { get { return Mathf.Sign(PlayerLookDir2D.x * RawInputs.Movement.x) < -0.01f; } }
    public Vector2 PlayerLookDir2D { get { return new Vector2(RecentDir, 0f); } }
    public Vector3 PlayerLookDir3D { get { return new Vector3(RecentDir, 0f, 0f); } }

    // RaycastHit
    public RaycastHit2D GroundHit { get; private set; }
    public RaycastHit2D UpwardGroundHit { get; set; }
    public RaycastHit2D ClimbHit { get; set; }

    // Component
    public PlayerAttackController PlayerAttackController { get { return _playerAttackController; } }
    public InteractionController InteractionController { get { return _interactionController; } }
    public PlayerMovementController PlayerMovementController { get { return _playerMovementController; } }
    public Rigidbody2D HandRigidBody { get { return _handRigidbody; } }
    public CapsuleCollider2D BodyCollider { get { return _bodyCollider; } }
    public SoundList SoundList { get { return _soundList; } }

    // SpriteRenderer / Material
    public SpriteRenderer[] SpriteRenderers { get { return _spriteRenderers; } }
    public Material[] OriginalMaterials => _originalMaterials;

    #endregion

    #region Function

    protected override void Awake()
    {
        // Controller
        _playerAttackController = GetComponent<PlayerAttackController>();
        _interactionController = GetComponent<InteractionController>();
        _playerMovementController = GetComponent<PlayerMovementController>();
        _lightController = GetComponent<LightController>();

        // collider
        _bodyCollider = GetComponent<CapsuleCollider2D>();

        // SoundList
        _soundList = GetComponent<SoundList>();

        // Material for White Flash
        LoadFlashMaterial();
        SaveOriginalMaterial();
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

        if (InputManager.Instance.State.BasicAttackKey.KeyDown)
            OnBasicAttackPressed();

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

        #region Check Ground & Climb

        // Check Ground
        GroundHit = Physics2D.CircleCast(_groundCheckTrans.position, _groundCheckRadius, Vector2.down, 0f, _groundLayer);

        // Check Upward
        UpwardGroundHit = Physics2D.Raycast(transform.position, Vector2.up, _groundAboveCheckLength, _groundLayer);

        // Check Climb
        ClimbHit = Physics2D.Raycast(_climbCheckTrans.position, PlayerLookDir2D, _climbCheckLength, _climbLayer);
        if (ClimbHit)
        {
            // TODO : 벽의 방향을 localScale로 하면 위험하다
            int wallLookDir = Math.Sign(ClimbHit.transform.localScale.x);
            bool isDirSync = (wallLookDir * RecentDir) > 0;
            IsClimbable = !isDirSync;
        }

        #endregion

        #region Animaotr Parameter

        // TODO : 필요없는거 삭제하기
        // + 애니메이터 수정하기
        Animator.SetBool("IsGround", IsGrounded);
        Animator.SetFloat("AirSpeedY", Rigidbody.velocity.y);
        Animator.SetFloat("InputHorizontal", RawInputs.Movement.x);
        Animator.SetFloat("PlayerLookDirX", PlayerLookDir2D.x);
        Animator.SetBool("IsDirSync", IsDirSync);

        #endregion
    }

    // basic
    private void InitPlayer()
    {
        _curHp = _maxHp;
        RecentDir = Math.Sign(transform.localScale.x);
    }
    private void UpdateImageFlip()
    {
        if (CurrentStateIs<RunState>() || CurrentStateIs<InAirState>() || PlayerMovementController.isActiveAndEnabled)
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
            if (CurrentStateIs<IdleState>() || CurrentStateIs<RunState>() || CurrentStateIs<JumpState>())
                ChangeState<InAirState>();
        }
    }


    // key pressed event
    void OnBasicAttackPressed()
    {
        if (CanBasicAttack)
            _playerAttackController.CastBasicAttack();
    }

    // about hit
    void TakeDamage(float damage)
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
    IEnumerator SlowMotionCoroutine(float duration)
    {
        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
    // flash
    private void LoadFlashMaterial()
    {
        _whiteFlashMaterial =
            Resources.Load<Material>("Materials/WhiteFlashMaterial");
    }
    private void SaveOriginalMaterial()
    {
        _originalMaterials = new Material[_spriteRenderers.Length];

        for (int i = 0; i < _originalMaterials.Length; i++)
            _originalMaterials[i] = _spriteRenderers[i].material;
    }
    public void InitMaterial()
    {
        for (int i = 0; i < _spriteRenderers.Length; i++)
            _spriteRenderers[i].material = _originalMaterials[i];
    }
    public void InitSpriteRendererAlpha()
    {
        foreach (var renderer in SpriteRenderers)
        {
            Color color = renderer.color;
            color.a = 1;
            renderer.color = color;
        }
    }
    private void ChangeMaterial()
    {
        for (int i = 0; i < _originalMaterials.Length; i++)
            _spriteRenderers[i].material = _whiteFlashMaterial;
    }
    private IEnumerator WhiteFlash()
    {
        // turn to white material
        ChangeMaterial();

        while (IsGodMode)
        {
            foreach (var spriteRenderer in _spriteRenderers)
                spriteRenderer.material.SetFloat("_FlashAmount", 0.4f);

            yield return new WaitForSeconds(_flashInterval);

            foreach (var spriteRenderer in _spriteRenderers)
                spriteRenderer.material.SetFloat("_FlashAmount", 0f);

            yield return new WaitForSeconds(_flashInterval);
        }

        // turn to original material
        if (!CurrentStateIs<InstantRespawnState>())
            InitMaterial();
    }
    public void StartWhiteFlash()
    {
        if (this._whiteFlashRoutine != null)
            StopCoroutine(this._whiteFlashRoutine);

        this._whiteFlashRoutine = StartCoroutine(WhiteFlash());
    }

    // god mode
    private IEnumerator GodModeTimer()
    {
        IsGodMode = true;
        yield return new WaitForSeconds(_godModeTime);
        IsGodMode = false;
    }
    public void StartGodModeTimer()
    {
        StartCoroutine(GodModeTimer());
    }

    // respawn
    public void OnRevive()
    {
        _curHp = _maxHp;
    }
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
        // set color
        Gizmos.color = Color.red;

        // Draw Ground Check
        Gizmos.DrawWireSphere(_groundCheckTrans.position, _groundCheckRadius);

        // Draw Ground Above Check
        Gizmos.DrawLine(_groundAboveCheckTrans.position,
            _groundAboveCheckTrans.position + Vector3.up * _groundAboveCheckLength);

        // Draw Wall Check
        Gizmos.DrawLine(_climbCheckTrans.position, _climbCheckTrans.position + PlayerLookDir3D * _climbCheckLength);
    }
}