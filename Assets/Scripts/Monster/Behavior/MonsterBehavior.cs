using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ������ �⺻ �ൿ�� �����ϴ� �߻�Ŭ����
/// </summary>
public abstract class MonsterBehavior : MonoBehaviour, IAttackListener
{
    /// <summary>
    /// ���� �����͸� ĸ��ȭ�ϴ� Ŭ����
    /// </summary>
    [Serializable]
    public struct MonsterData // Struct�� �����ϰ�, �Һ����� ������, ������ �������� ������ �ʿ䰡 ���� �����͸� ������ �� ���
    {
        public string MonsterName;
        public MonsterDefine.RankType RankType;
        public MonsterDefine.MoveType MoveType;
        public int MaxHp;
        public float MoveSpeed;
        public float Acceleration;
        public Vector2 JumpForce;

        public void PrintAllData()
        {
            // debug all data
            Debug.Log($"MonsterName: {MonsterName}");
            Debug.Log($"RankType: {RankType}");
            Debug.Log($"MoveType: {MoveType}");
            Debug.Log($"MaxHp: {MaxHp}");
            Debug.Log($"MoveSpeed: {MoveSpeed}");
            Debug.Log($"Acceleration: {Acceleration}");
            Debug.Log($"JumpForce: {JumpForce}");
        }
    }

    #region Attribute

    // Basic Component
    public Rigidbody2D RigidBody2D { get; private set; }
    public Animator Animator { get; private set; }
    public Collider2D MainBodyCollider2D { get; private set; }
    public MaterialManager MaterialManager { get; private set; }
    public SoundList SoundList { get; private set; }

    // State
    public Monster_StateBase CurrentState { get; private set; }
    private Monster_StateBase _initialState;

    // Module
    public FloatingPatrolModule FloatingPatrolModule { get; private set; }
    public GroundMovementModule GroundMovementModule { get; private set; }
    public FloatingMovementModule FloatingMovementModule { get; private set; }

    // Evaluator
    public GroundPatrolEvaluator GroundPatrolEvaluator { get; private set; }
    public GroundChaseEvaluator GroundChaseEvaluator { get; private set; }
    public FloatingChaseEvaluator FloatingChaseEvaluator { get; private set; }
    public AttackEvaluator AttackEvaluator { get; private set; }

    [field: Header("Condition")]
    [field: Space]

    [field: SerializeField]
    public int DefaultDir
    {
        get;
        private set;
    }
    [field: SerializeField]
    public int RecentDir // mean look direction
    {
        get;
        set;
    }
    [field: SerializeField]
    public bool IsGround
    {
        get;
        set;
    }
    [field: SerializeField]
    public bool IsInAir
    {
        get;
        set;
    }
    [field: SerializeField]
    public bool IsAttacking
    {
        get;
        set;
    }
    [field: SerializeField]
    public bool IsHiding
    {
        get;
        set;
    }
    [field: SerializeField]
    public bool IsGodMode
    {
        get;
        set;
    }
    [field: SerializeField]
    public bool IsHitting
    {
        get;
        set;
    }
    [field: SerializeField]
    public bool IsHurt
    {
        get;
        set;
    }
    [field: SerializeField]
    public bool IsDead
    {
        get;
        set;
    }

    [Header("Monster Data")]
    [Space]

    [SerializeField]
    private MonsterDataObject _monsterDataObject;

    [Space]

    public MonsterData monsterData;
    [SerializeField] private int _curHp;
    public int CurHp
    {
        get => _curHp;
        set
        {
            _curHp = value;

            // Debug.Log("CurHp is changed");

            // ü���� 0 ���ϰ� �Ǹ� ���
            if (_curHp <= 0)
            {
                // Debug.Log("Die");

                _curHp = 0;
                Die();
            }
        }
    }

    [field: Space]

    [field: Header("Ground Check")]
    [field: Space]

    [field: SerializeField]
    public LayerMask GroundCheckLayer
    {
        get;
        private set;
    }
    [field: SerializeField]
    public Collider2D GroundCheckCollider
    {
        get;
        private set;
    }
    public RaycastHit2D groundRayHit;

    [field: Header("ETC")]
    [field: Space]

    [field: SerializeField]
    public Transform CenterOfMass
    {
        get;
        private set;
    }
    public Bounds RespawnBounds
    {
        get;
        private set;
    }
    public Vector3 FirstPosition
    {
        get;
        private set;
    }

    /// <summary>
    /// animation transition event
    /// </summary>
    /// <param name="targetTransitionParam">���̸� ���� Ʈ���� �Ķ����</param>
    /// <param name="state">������ ����</param>
    /// <returns>�οﰪ�� �����Ͽ�, �ڷ�ƾ���� �ش� �̺�Ʈ�� ���Ḧ ��ٸ�</returns>
    public delegate bool AnimTransitionDelegate(string targetTransitionParam, Monster_StateBase state);
    public event AnimTransitionDelegate AnimTransitionEvent;

    /// <summary>
    /// �޼��� ������ ���� ��������Ʈ (�Լ� ������ ����)
    /// �� ��������Ʈ�� No Parameter, No Return �޼��带 ������� �Ѵ�
    /// </summary>
    public delegate void ActionDelegate();

    #endregion

    #region Function

    protected virtual void Awake()
    {
        // Basic Component
        RigidBody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        MainBodyCollider2D = GetComponent<Collider2D>();
        MaterialManager = GetComponent<MaterialManager>();
        SoundList = GetComponent<SoundList>();

        // Module
        FloatingPatrolModule = GetComponent<FloatingPatrolModule>();
        GroundMovementModule = GetComponent<GroundMovementModule>();
        FloatingMovementModule = GetComponent<FloatingMovementModule>();

        // Evaluator
        GroundPatrolEvaluator = GetComponent<GroundPatrolEvaluator>();
        GroundChaseEvaluator = GetComponent<GroundChaseEvaluator>();
        FloatingChaseEvaluator = GetComponent<FloatingChaseEvaluator>();
        AttackEvaluator = GetComponent<AttackEvaluator>();

        // Set recentDir
        RecentDir = DefaultDir;

        // Set center of mass
        if (!CenterOfMass) CenterOfMass = this.transform;
        if (RigidBody2D) RigidBody2D.centerOfMass = CenterOfMass.localPosition;

        // For respawn position
        RespawnBounds = MainBodyCollider2D.bounds;      // basic setting
        FirstPosition = transform.position;
    }
    protected virtual void Start()
    {
        Initialize();
    }
    protected virtual void Update()
    {
        if (IsDead)
            return;

        // ground check
        switch (monsterData.MoveType)
        {
            case MonsterDefine.MoveType.Ground:

                // ground rayCast
                RaycastHit2D[] groundRayHits = Physics2D.BoxCastAll(GroundCheckCollider.transform.position,
                    GroundCheckCollider.bounds.size, 0f, Vector2.zero, 0f,
                    GroundCheckLayer);

                // Debug.Log($"{groundRayHits.Length}���� RayCastHit");

                // ground rayCast hit
                bool hasGroundContact = groundRayHits.Length > 0;

                // groundRayHits�� ���Ϳ� ������ �浹�� ������ ���� �����̴�.
                // �� ��, ���� ����� ������ GroundRayHit�� �����Ѵ�.
                foreach (var hit in groundRayHits)
                {
                    // ���⼭ �� hit.normal�� �Ʒ����� ���ϴ��� Ȯ���ؾ� �Ѵ�.
                    // Debug.DrawRay(hit.point, hit.normal, Color.cyan);
                    // Debug.Log($"{hit.collider.gameObject.name}");

                    // �浹 ������ �� ������Ʈ���� �Ÿ��� ���� ����� ���� ����
                    if (groundRayHit)
                    {
                        float newDistSquared = Vector2.SqrMagnitude(hit.point - (Vector2)transform.position);
                        float oldDistSquared = Vector2.SqrMagnitude(groundRayHit.point - (Vector2)transform.position);

                        if (newDistSquared < oldDistSquared)
                            groundRayHit = hit;
                    }
                    else
                        groundRayHit = hit;
                }

                IsGround = hasGroundContact;
                IsInAir = !hasGroundContact;

                break;

            case MonsterDefine.MoveType.Fly:

                IsGround = false;
                IsInAir = true;

                break;
        }

        // range based attack by evaluator
        if (AttackEvaluator)
        {
            if (!AttackEvaluator.IsUsable) return;
            if (AttackEvaluator.IsDuringCoolTime) return;

            if (CurrentState is IAttackableState)
            {
                if (AttackEvaluator.IsTargetWithinRange())
                {
                    StartChangeStateCoroutine("Attack", CurrentState, AttackEvaluator.StartEvaluatorCoolTime());
                }
            }
        }
    }

    public virtual void SetUp()
    {
        // ������ �̸�
        monsterData.MonsterName = _monsterDataObject.Name.ToString();

        // ������ ��ũ
        monsterData.RankType = _monsterDataObject.RankType;

        // ������ �ൿ Ÿ��
        monsterData.MoveType = _monsterDataObject.MoveType;

        // ������ �ִ� ü��
        monsterData.MaxHp = _monsterDataObject.MaxHp;

        // ������ �̵��ӵ�
        monsterData.MoveSpeed = _monsterDataObject.MoveSpeed;

        // ������ ���ӵ�
        monsterData.Acceleration = _monsterDataObject.Acceleration;

        // ������ �����Ŀ�
        monsterData.JumpForce = _monsterDataObject.JumpForce;
    }
    public virtual void KnockBack(Vector2 forceVector)
    {
        if (FloatingMovementModule)
        {
            // NavMeshAgent ���� KnockBack
            FloatingMovementModule.SetVelocity(forceVector / 2.0f);
        }
        else
        {
            // �ӵ� �ʱ�ȭ
            RigidBody2D.velocity = Vector2.zero;

            // Monster�� Mass�� ���� ����
            forceVector *= RigidBody2D.mass / 1.0f;
            RigidBody2D.AddForce(forceVector, ForceMode2D.Impulse);
        }
    }
    public virtual IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        if (IsGodMode || IsDead)
            return IAttackListener.AttackResult.Fail;

        HitProcess(attackInfo);
        HurtProcess();

        return IAttackListener.AttackResult.Success;
    }
    public virtual void Die(bool isHitBoxDisable = true, bool isDeathProcess = true)
    {
        // Set Dead
        IsDead = true;  // �ź��̴� IsDead == true ���� �ʵ�� ������ �� �ִ�.

        // Check that Animator has Die Trigger Param
        foreach (AnimatorControllerParameter param in Animator.parameters)
        {
            if (param.name == "Die" && param.type == AnimatorControllerParameterType.Trigger)
                SetAnimatorTrigger("Die");
        }

        // HitBox Disable
        SetHitBoxDisable(isHitBoxDisable);

        // Death Effect
        if (isDeathProcess) StartCoroutine(DeathProcessCoroutine(0.2f));
    }
    public virtual void Respawn(Vector3 respawnPosition)
    {
        // Enable gameObject
        gameObject.SetActive(true);

        // Set Position
        if (FloatingMovementModule) FloatingMovementModule.SetPosition(respawnPosition);
        else transform.position = respawnPosition;

        // Respawn Process
        StartCoroutine(RespawnProcessCoroutine());
    }

    // basic
    public void Initialize()
    {
        // SetUp MonsterData
        SetUp();

        // Set Look Direction
        SetRecentDir(DefaultDir);

        // Set Current HP
        CurHp = monsterData.MaxHp;

        // Init Condition
        if (IsAttacking) IsAttacking = false;
        if (IsHiding) IsHiding = false;
        if (IsGodMode) IsGodMode = false;
        if (IsHitting) IsHitting = false;
        if (IsHurt) IsHurt = false;
        if (IsDead) IsDead = false;

        // Init State
        InitState();
    }
    public void SetRespawnBounds(Bounds bounds)
    {
        // Ȱ�� ������ �����ϴ� ��ũ��Ʈ�κ��� bounds�� ���޹޴´�.
        RespawnBounds = bounds;
    }
    public void SetRecentDir(int targetDir)
    {
        // flip�� ��ų���� ���� ��
        var flipValue = RecentDir * targetDir;

        if (flipValue == 1)
        {
            // Debug.Log("recentDir == targetDir");
            return;
        }
        if (flipValue == 0)
        {
            // Debug.Log("recentDir or targetDir is '0'");
            return;
        }

        // �ٶ󺸴� ���� ����
        RecentDir = targetDir;

        // �ٶ󺸴� �������� Flip
        transform.localScale = new Vector3(transform.localScale.x * flipValue, transform.localScale.y, transform.localScale.z);
    }
    public void StartSetRecentDirAfterGrounded(int targetDir)
    {
        StartCoroutine(SetRecentDirAfterGrounded(targetDir));
    }
    private IEnumerator SetRecentDirAfterGrounded(int targetDir)
    {
        // �ڷ�ƾ�� ����� ���� ��ȯ�� ���� ���� �ķ� �����Ѵ�
        yield return new WaitUntil(() => IsGround);

        SetRecentDir(targetDir);
    }

    // hit
    public void HitProcess(AttackInfo attackInfo, bool onDamage = true, bool onKnockBack = true, bool useBlinkEffect = true)
    {
        PlaySound("Hurt");
        StartCoroutine(HitTimerCoroutine());

        if (useBlinkEffect)
        {
            if (MaterialManager)
            {
                if (MaterialManager.BlinkEffect)
                    MaterialManager.BlinkEffect.Play();
                else
                    Debug.LogWarning("Blink Effect isn't attached");
            }
        }

        if (onKnockBack)
            KnockBack(attackInfo.Force);

        if (onDamage)
            CurHp -= (int)attackInfo.Damage;
    }
    public void HurtProcess()
    {
        if (IsDead) return;
        if (IsAttacking) return;

        // Run Hurt Animation
        SetAnimatorTrigger("Hurt");
    }
    private IEnumerator HitTimerCoroutine()
    {
        IsHitting = true;
        yield return new WaitForSeconds(0.1f);
        IsHitting = false;
    }

    // hitBox
    public void SetHitBoxDisable(bool isDisable)
    {
        // includeInactive : true -> ��Ȱ��ȭ�� �ڽ� ������Ʈ�� �˻�
        var hitBox = GetComponentInChildren<MonsterBodyHit>(true);

        if (hitBox)
            hitBox.gameObject.SetActive(!isDisable);
    }
    public void SetHitBoxStepable(bool isStepable)
    {
        var hitBox = GetComponentInChildren<MonsterBodyHit>(true);

        if (hitBox)
        {
            Collider2D hitBoxCollider = hitBox.GetComponent<Collider2D>();
            if (hitBoxCollider)
            {
                hitBoxCollider.isTrigger = isStepable;
                hitBox.gameObject.layer = LayerMask.NameToLayer(isStepable ? "Default" : "MonsterHitBox");
            }
        }
    }
    public void SetHitBoxAttackable(bool isAttackable)
    {
        var hitBox = GetComponentInChildren<MonsterBodyHit>(true);

        if (hitBox)
            hitBox.IsAttackable = isAttackable;
    }

    // Death & Respawn
    private IEnumerator DeathProcessCoroutine(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        // Stop movement
        Animator.speed = 0;
        RigidBody2D.simulated = false;
        if (FloatingMovementModule)
        {
            FloatingMovementModule.SetStopAgent(true, true);
            // FloatingMovementModule.Agent.enabled = false;
            // FloatingMovementModule.SetPosition(Vector3.zero);
        }

        // Wait until death effect is done
        yield return StartCoroutine(DeathEffectCoroutine());

        // Notify Death to MonsterRespawnManager
        MonsterRespawnManager.Instance.NotifyDeath(this);

        // Stop all coroutines on this behavior
        StopAllCoroutines();

        // Disable gameObject
        gameObject.SetActive(false);
    }
    private IEnumerator DeathEffectCoroutine()
    {
        // effect process
        MaterialManager.DisintegrateEffect.Play();    // death effect needs delay for natural
        yield return new WaitUntil(() => MaterialManager.DisintegrateEffect.IsEffectDone);
        MaterialManager.DisintegrateEffect.Revert();
    }
    private IEnumerator RespawnProcessCoroutine()
    {
        // Wait until respawn effect is done
        yield return StartCoroutine(ReSpawnEffectCoroutine());

        // HitBox Enable
        SetHitBoxDisable(false);

        // Resume movement
        Animator.speed = 1;
        RigidBody2D.simulated = true;
        if (FloatingMovementModule)
        {
            FloatingMovementModule.SetStopAgent(false, false);
            // FloatingMovementModule.Agent.enabled = true;
        }

        // Reset Condition
        Initialize();
    }
    private IEnumerator ReSpawnEffectCoroutine()
    {
        // effect process
        MaterialManager.RespawnEffect.Play();
        yield return new WaitUntil(() => MaterialManager.RespawnEffect.IsEffectDone);
        MaterialManager.RespawnEffect.Revert();
    }

    // state
    private void InitState()
    {
        // Bring Entry State
        int initialPathHash = Animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        StateMachineBehaviour[] initialStates = Animator.GetBehaviours(initialPathHash, 0);
        foreach (var initialState in initialStates)
        {
            if (initialState as Monster_StateBase)
                _initialState = initialState as Monster_StateBase;
        }

        // Init Animation State
        CurrentState = _initialState;
    }
    public void UpdateState(Monster_StateBase state)
    {
        CurrentState = state;
    }
    public bool CurrentStateIs<TState>() where TState : Monster_StateBase
    {
        return CurrentState is TState;
    }
    public void StartChangeStateCoroutine(string targetTransitionParam, Monster_StateBase currentState, ActionDelegate myFunction = null)
    {
        StartCoroutine(ChangeStateCoroutine(targetTransitionParam, currentState, myFunction));
    }
    private IEnumerator ChangeStateCoroutine(string targetTransitionParam, Monster_StateBase currentState, ActionDelegate myFunction)
    {
        // �ִϸ��̼� ���� ������ �ִ� ��� ������ ������ ������ ���
        if (AnimTransitionEvent != null)
            yield return new WaitUntil(() => AnimTransitionEvent(targetTransitionParam, currentState));

        Animator.SetTrigger(targetTransitionParam);

        // �߰��� �����ؾ� �ϴ� �Լ�
        myFunction?.Invoke();
    }

    // Animator & Sound
    public void SetAnimatorTrigger(string key)
    {
        Animator.SetTrigger(key);
    }
    public void PlaySound(string key)
    {
        SoundList.PlaySFX(key);
    }

    #endregion
}
