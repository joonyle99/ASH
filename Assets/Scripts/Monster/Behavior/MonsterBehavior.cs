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

    [Serializable]
    public struct RespawnData
    {
        public Vector3 FirstPosition;
        public Bounds RespawnBounds;
    }

    #region Attribute

    // Basic Component
    public Rigidbody2D RigidBody2D { get; private set; }
    public Animator Animator { get; private set; }
    public Collider2D MainBodyCollider2D { get; private set; }      // circle collider 2d or box collider 2d
    public MaterialController MaterialController { get; private set; }
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

    [Header("Condition")]
    [Space]

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
    public MonsterData monsterData;
    public RespawnData respawnData;

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
        MaterialController = GetComponent<MaterialController>();
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
    }
    protected virtual void Start()
    {
        InitData();

        InitCondition();

        InitState();
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
        if (isDeathProcess) StartCoroutine(DeathProcessCoroutine());
    }
    public virtual void Respawn()
    {
        StartCoroutine(RespawnProcessCoroutine());
    }

    // init
    private void InitData()
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
    private void InitCondition()
    {
        RecentDir = DefaultDir;

        if (!CenterOfMass) CenterOfMass = this.transform;
        RigidBody2D.centerOfMass = CenterOfMass.localPosition;

        respawnData.FirstPosition = transform.position;

        CurHp = monsterData.MaxHp;
    }
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

    // basic
    public void SetRespawnBounds(Bounds bounds)
    {
        respawnData.RespawnBounds = bounds;
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
            if (MaterialController)
            {
                if (MaterialController.BlinkEffect)
                    MaterialController.BlinkEffect.Play();
                else
                    Debug.LogWarning("Blink Effect isn't attached");
            }
        }

        if (onKnockBack)
            KnockBack(attackInfo.Force);

        if (onDamage)
            CurHp -= (int)attackInfo.Damage;
    }
    public void KnockBack(Vector2 forceVector)
    {
        if (FloatingMovementModule)
        {
            // NavMeshAgent ���� KnockBack
            // Debug.Log("KnockBack");
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
    private IEnumerator DeathProcessCoroutine()
    {
        yield return new WaitForSeconds(0.2f);

        // Stop movement
        Animator.speed = 0;
        RigidBody2D.simulated = false;
        if (FloatingMovementModule)
        {
            FloatingMovementModule.SetStopAgent(true);
            FloatingMovementModule.SetVelocity(Vector3.zero);
        }

        // Wait until death effect is done
        yield return StartCoroutine(DeathEffectCoroutine());

        // Stop all coroutines on this behavior
        StopAllCoroutines();

        // ������� Monster Respawn Manager���� �ѱ��
        MonsterRespawnManager.Instance.NotifyDeath(this);
    }
    private IEnumerator DeathEffectCoroutine()
    {
        // effect process
        MaterialController.DisintegrateEffect.Play();
        yield return new WaitUntil(() => MaterialController.DisintegrateEffect.IsEffectDone);
    }
    private IEnumerator RespawnProcessCoroutine()
    {
        // stop before respawn effect
        Animator.speed = 0;
        if (FloatingMovementModule) FloatingMovementModule.SetStopAgent(true);
        else RigidBody2D.simulated = false;
        SetHitBoxDisable(true);

        // Wait until respawn effect is done
        yield return StartCoroutine(ReSpawnEffectCoroutine());

        // resume after respawn effect
        Animator.speed = 1;
        if (FloatingMovementModule) FloatingMovementModule.SetStopAgent(false);
        else RigidBody2D.simulated = true;
        SetHitBoxDisable(false);
    }
    private IEnumerator ReSpawnEffectCoroutine()
    {
        // effect process
        MaterialController.RespawnEffect.Play();
        yield return new WaitUntil(() => MaterialController.RespawnEffect.IsEffectDone);
    }

    // state
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

    // Wrapper
    public void SetAnimatorTrigger(string key)
    {
        Animator.SetTrigger(key);
    }
    public void PlaySound(string key)
    {
        SoundList.PlaySFX(key);
    }
    public void DestroyMonster()
    {
        Destroy(transform.root ? transform.root.gameObject : gameObject);
    }

    #endregion
}
