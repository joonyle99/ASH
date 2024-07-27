using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerBehaviour : StateMachineBase, IAttackListener, ISceneContextBuildListener
{
    private const int LIMIT_HP = 20;

    #region Attribute

    [Header("Player")]
    [Space]

    [SerializeField, Range(0, LIMIT_HP)] private int _maxHp = 10;
    [SerializeField, Range(0, LIMIT_HP)] private int _startHp = 5;
    [SerializeField] private int _curHp;

    [Space]

    [SerializeField] private bool _isHurt;
    [SerializeField] private bool _isGodMode;
    [SerializeField] private bool _isDead;

    [Space]

    [SerializeField] private int _godModeReferenceCount;

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

    [Header("Parts")]
    [Space]

    [SerializeField] private CapsuleCollider2D _bodyCollider;
    [SerializeField] private Rigidbody2D _handRigidbody;
    [SerializeField] private Collider2D _heartCollider;
    [SerializeField] private Cloth _capeCloth;
    [SerializeField] private Material _capeMaterial;

    [Header("ETC")]
    [Space]

    [SerializeField] private MaterialController materialController;
    [SerializeField] private SoundList _soundList;
    [SerializeField] private Transform _interactionMarkerPoint;

    // Controller
    private PlayerMovementController _playerMovementController;
    private PlayerAttackController _playerAttackController;
    private PlayerInteractionController _playerInteractionController;
    private PlayerLightSkillController _playerLightSkillController;
    private PlayerHeadAimController _playerHeadAimController;

    public delegate void HealthChangeEvent(int curHp, int maxHp);
    public event HealthChangeEvent OnHealthChanged;

    #endregion

    #region Properties

    /// <summary>
    /// �÷��̾ ������ �� �� �ִ� ���¸� ��Ÿ��
    /// </summary>
    public bool CanAttack
    {
        get => _isCanAttack && CurrentState is IAttackableState &&
               _playerLightSkillController.IsLightButtonPressable &&
               !_playerLightSkillController.IsLightWorking;

        set => _isCanAttack = value;
    }

    public bool CanDash
    {
        get => _isCanDash && PersistentDataManager.GetByGlobal<bool>("Dash");
        set => _isCanDash = value;
    }
    public bool CanInteract => CurrentState is IInteractableState;

    // Condition Property
    public bool IsGrounded => GroundHit;                                    // �÷��̾��� �Ʒ� �������� Circle Cast
    public bool IsUpWardGrounded => UpwardGroundHit;
    public bool IsUpWardGroundedForClimb => UpwardGroundHitForClimb;
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
        set
        {
            // ���۷��� ī���� ��� ����

            // Blink Effect���� True�� �����ǰ�, CutscenePlayer���� True�� �����Ǹ�
            // ���۷��� ī���Ͱ� 2�� �ȴ�.
            // ���� Blink Effect�� ����� ��, GodMode�� False�� �����Ϸ� �ϸ�
            // ���۷��� ī���Ͱ� 1�� �Ǹ鼭, GodMode�� True�� �����ȴ�.

            if (value)
            {
                _godModeReferenceCount++;
                _isGodMode = _godModeReferenceCount > 0;
            }
            else
            {
                _godModeReferenceCount = Math.Max(0, _godModeReferenceCount - 1);
                _isGodMode = _godModeReferenceCount > 0;
            }
        }
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

            if (_curHp > _maxHp) _curHp = _maxHp;   // �ִ� ü���� �Ѿ ���� ����
            else if (_curHp <= 0)
            {
                _curHp = 0;                         // ü���� 0 �̸��� �� ���� ����
                ChangeState<DieState>();
            }

            OnHealthChanged?.Invoke(_curHp, _maxHp);
        }
    }
    public int MaxHp
    {
        get => _maxHp;
        set
        {
            _maxHp = value;

            if (_maxHp > LIMIT_HP) _maxHp = LIMIT_HP; // �ִ� ü���� ���ѵȴ�
            else if (_maxHp < 0) _maxHp = 0;          // �ִ� ü���� 0 �̸��� �� ���� ����

            OnHealthChanged?.Invoke(_curHp, _maxHp);
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
    public RaycastHit2D UpwardGroundHitForClimb { get; set; }
    public RaycastHit2D ClimbHit { get; set; }

    // Component
    public PlayerAttackController PlayerAttackController => _playerAttackController;
    public PlayerInteractionController PlayerInteractionController => _playerInteractionController;
    public PlayerMovementController PlayerMovementController => _playerMovementController;

    // ETC
    public CapsuleCollider2D BodyCollider => _bodyCollider;
    public Collider2D HeartCollider => _heartCollider;
    public MaterialController MaterialController => materialController;
    public SoundList SoundList => _soundList;
    public Rigidbody2D HandRigidBody => _handRigidbody;
    public Transform InteractionMarker => _interactionMarkerPoint;

    #endregion

    #region Function

    protected override void Awake()
    {
        base.Awake();

        // Controller
        _playerAttackController = GetComponent<PlayerAttackController>();
        _playerInteractionController = GetComponent<PlayerInteractionController>();
        _playerMovementController = GetComponent<PlayerMovementController>();
        _playerLightSkillController = GetComponent<PlayerLightSkillController>();
        _playerHeadAimController = GetComponent<PlayerHeadAimController>();

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

        // Attack
        if (InputManager.Instance.State.AttackKey.KeyDown && CanAttack)
        {
            _playerAttackController.CastAttack();
        }

        // CHEAT: Recover Cheat HP
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            RecoverCurHp(2);
        }

        // CHEAT: Open Boss Door
        if (Input.GetKeyDown(KeyCode.F10))
        {
            // ���� ����� BossDoor�� ã�´� (LINQ ���)
            var closestBossDoor = FindObjectsByType<BossDoor>(FindObjectsSortMode.None)
                .OrderBy(door => Vector3.Distance(transform.position, door.transform.position))
                .FirstOrDefault();

            if (closestBossDoor == null)
                Debug.Log("No Boss Door found in the scene.");

            // ��ȣ�ۿ� �Ұ����� ���·� �����
            closestBossDoor.IsInteractable = false;

            // ���� ����� BossDoor�� ���������� �ݱ�, ���������� ����
            if (closestBossDoor.IsOpened)
                closestBossDoor.CloseDoor();
            else
                closestBossDoor.OpenDoor();
        }

        #endregion

        #region Basic Behavior

        // Player Flip
        UpdateImageFlip();

        // Change In Air State
        ChangeInAirState();

        // Control Cape
        if (IsMoveXKey)
        {
            CapeControlX();
        }
        else
        {
            CapeZeroX();
        }

        #endregion

        #region Animaotr Parameter

        Animator.SetBool("IsGround", IsGrounded);
        Animator.SetBool("IsUpwardGround", IsUpWardGrounded);

        Animator.SetFloat("AirSpeedY", Rigidbody.velocity.y);

        Animator.SetBool("IsDirSync", IsDirSync);
        Animator.SetBool("IsOppositeDirSync", IsOppositeDirSync);

        #endregion
    }

    // build listener
    public void OnSceneContextBuilt()
    {
        // TODO: ���� ü���� �۷ι��� ����� �÷��̾��� ü������ �����Ѵ�

    }

    // basic
    private void InitPlayer()
    {
        // ü�� �ʱ�ȭ
        if(JsonDataManager.Has("PlayerData"))
        {
            JsonDataManager.JsonLoad();
            JsonPlayerData playerData = JsonDataManager.GetObjectInGlobalSaveData<JsonPlayerData>("PlayerData");

            MaxHp = playerData._maxHp;
            CurHp = playerData._currentHp;
        }
        else
        {
            CurHp = _startHp;
        }

        // �ٶ󺸴� ���� ����
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
    public void SetCapeEmission(float intensity)
    {
        // material ��ü�� ��⸦ �����Ѵ�
        _capeMaterial.SetFloat("_Intensity", intensity);
    }

    // about hit
    public IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        // fail return condition
        if (IsHurt || IsGodMode || IsDead)
            return IAttackListener.AttackResult.Fail;

        // Debug.Log(System.Environment.StackTrace);

        PlaySound_SE_Hurt_02();
        StartCoroutine(SlowMotionCoroutine(0.3f));

        TakeDamage((int)attackInfo.Damage);

        // die state
        if (CurHp <= 0) return IAttackListener.AttackResult.Success;

        KnockBack(attackInfo.Force);

        // Change Hurt State
        ChangeState<HurtState>();

        return IAttackListener.AttackResult.Success;
    }
    private void TakeDamage(float damage)
    {
        CurHp -= (int)damage;
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
        CurHp = MaxHp;      // ü���� �ִ� ü�¸�ŭ ȸ���Ѵ�

        // Debug.Log("�ִ� ü�� ���� ����");
    }
    public void RecoverCurHp(int amount)
    {
        CurHp += amount;

        // Debug.Log("ü�� ȸ�� ����");
    }

    // respawn
    public void TriggerInstantRespawn(float damage)
    {
        if (IsDead) return;

        TakeDamage(damage);

        if (CurHp > 0)
        {
            ChangeState<InstantRespawnState>();
        }
    }

    // etc
    public void FinishState_AnimEvent()
    {
        // from hurt state

        ChangeState<IdleState>();
    }

    public void CapeControlX()
    {
        var vec = _capeCloth.externalAcceleration;
        vec.x = (-1) * RecentDir * 20f;
        _capeCloth.externalAcceleration = vec;
    }
    public void CapeZeroX()
    {
        var vec = _capeCloth.externalAcceleration;
        vec.x = 0f;
        _capeCloth.externalAcceleration = vec;
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
        _soundList.PlaySFX("SE_Die_02(Short)");
    }

    public void PlaySound(string key)
    {
        _soundList.PlaySFX(key);
    }

    #endregion
}