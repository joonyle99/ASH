using System;
using System.Collections;
using UnityEngine;

public class PlayerBehaviour : StateMachineBase, IAttackListener
{
    #region Attribute

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

    [Header("White Flash for Hit")]
    [Space]

    [SerializeField] Material _whiteFlashMaterial;
    [SerializeField] SpriteRenderer[] _spriteRenderers;

    [SerializeField] float _godModeTime = 1.5f;
    [SerializeField] float _flashInterval = 0.06f;

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

    [Header("ETC")]
    [Space]

    [SerializeField] CapsuleCollider2D _bodyCollider;
    [SerializeField] Rigidbody2D _handRigidbody;

    // Controller
    PlayerAttackController _playerAttackController;
    InteractionController _interactionController;
    PlayerMovementController _playerMovementController;
    LightController _lightController;
    HeadAimController _headAimContoller;

    // Sound List
    SoundList _soundList;

    #endregion

    #region Properties

    // Can Property
    public bool CanBasicAttack
    {
        get => _isCanBasicAttack && (CurrentStateIs<IdleState>() || CurrentStateIs<RunState>() || CurrentStateIs<InAirState>()) && (_lightController.IsLightButtonPressable && !_lightController.IsLightWorking);
        set => _isCanBasicAttack = value;
    }
    public bool CanDash { get => _isCanDash && PersistentDataManager.Get<bool>("Dash");
        set => _isCanDash = value;
    }
    public bool CanInteract => CurrentStateIs<IdleState>() || CurrentStateIs<RunState>();

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
    public bool IsDead
    {
        get => _isDead;
        set => _isDead = value;
    }
    public bool IsGodMode
    {
        get => _isGodMode;
        set => _isGodMode = value;
    }
    public int CurHp
    {
        get => _curHp;
        set
        {
            _curHp = value;

            if (_curHp > _maxHp) _curHp = _maxHp;
            else if (_curHp < 0) _curHp = 0;
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
    public RaycastHit2D UpwardGroundHit { get; set; }
    public RaycastHit2D ClimbHit { get; set; }

    // Component
    public PlayerAttackController PlayerAttackController => _playerAttackController;
    public InteractionController InteractionController => _interactionController;
    public PlayerMovementController PlayerMovementController => _playerMovementController;
    public Rigidbody2D HandRigidBody => _handRigidbody;
    public CapsuleCollider2D BodyCollider => _bodyCollider;
    public SoundList SoundList => _soundList;

    // SpriteRenderer for White Flash
    public SpriteRenderer[] SpriteRenderers => _spriteRenderers;

    #endregion

    #region Function

    protected override void Awake()
    {
        // Controller
        _playerAttackController = GetComponent<PlayerAttackController>();
        _interactionController = GetComponent<InteractionController>();
        _playerMovementController = GetComponent<PlayerMovementController>();
        _lightController = GetComponent<LightController>();
        _headAimContoller = GetComponent<HeadAimController>();

        // collider
        _bodyCollider = GetComponent<CapsuleCollider2D>();

        // SoundList
        _soundList = GetComponent<SoundList>();

        // Material for White Flash
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

        #region Animaotr Parameter

        Animator.SetBool("IsGround", IsGrounded);
        Animator.SetFloat("AirSpeedY", Rigidbody.velocity.y);
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
        if (CurrentStateIs<RunState>() || CurrentStateIs<InAirState>())
        {
            if (IsOppositeDirSync && IsMoveXKey)
            {
                RecentDir = (int)RawInputs.Movement.x;
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * RecentDir, transform.localScale.y, transform.localScale.z);

                _headAimContoller.HeadAimControlOnFlip();
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

    #endregion
}