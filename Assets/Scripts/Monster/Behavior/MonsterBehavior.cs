using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 몬스터의 기본 행동을 정의하는 추상클래스
/// </summary>
public abstract class MonsterBehavior : MonoBehaviour, IAttackListener
{
    /// <summary>
    /// 몬스터 데이터를 캡슐화하는 클래스
    /// </summary>
    [Serializable]
    public struct MonsterData // Struct는 간단하고, 불변성을 가지며, 원본에 동적으로 수정할 필요가 없는 데이터를 저장할 때 사용
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

            // 체력이 0 이하가 되면 사망
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
    /// <param name="targetTransitionParam">전이를 위한 트리거 파라미터</param>
    /// <param name="state">현재의 상태</param>
    /// <returns>부울값을 리턴하여, 코루틴에서 해당 이벤트의 종료를 기다림</returns>
    public delegate bool AnimTransitionDelegate(string targetTransitionParam, Monster_StateBase state);
    public event AnimTransitionDelegate AnimTransitionEvent;

    /// <summary>
    /// 메서드 참조를 위한 델리게이트 (함수 포인터 선언)
    /// 이 델리게이트는 No Parameter, No Return 메서드를 대상으로 한다
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

                // Debug.Log($"{groundRayHits.Length}개의 RayCastHit");

                // ground rayCast hit
                bool hasGroundContact = groundRayHits.Length > 0;

                // groundRayHits는 몬스터와 지면이 충돌한 지점에 대한 정보이다.
                // 이 중, 가장 가까운 지점을 GroundRayHit에 저장한다.
                foreach (var hit in groundRayHits)
                {
                    // 여기서 왜 hit.normal이 아래쪽을 향하는지 확인해야 한다.
                    // Debug.DrawRay(hit.point, hit.normal, Color.cyan);
                    // Debug.Log($"{hit.collider.gameObject.name}");

                    // 충돌 지점과 이 오브젝트와의 거리가 가장 가까운 놈을 저장
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
        // 몬스터의 이름
        monsterData.MonsterName = _monsterDataObject.Name.ToString();

        // 몬스터의 랭크
        monsterData.RankType = _monsterDataObject.RankType;

        // 몬스터의 행동 타입
        monsterData.MoveType = _monsterDataObject.MoveType;

        // 몬스터의 최대 체력
        monsterData.MaxHp = _monsterDataObject.MaxHp;

        // 몬스터의 이동속도
        monsterData.MoveSpeed = _monsterDataObject.MoveSpeed;

        // 몬스터의 가속도
        monsterData.Acceleration = _monsterDataObject.Acceleration;

        // 몬스터의 점프파워
        monsterData.JumpForce = _monsterDataObject.JumpForce;
    }
    public virtual void KnockBack(Vector2 forceVector)
    {
        if (FloatingMovementModule)
        {
            // NavMeshAgent 전용 KnockBack
            FloatingMovementModule.SetVelocity(forceVector / 2.0f);
        }
        else
        {
            // 속도 초기화
            RigidBody2D.velocity = Vector2.zero;

            // Monster의 Mass에 따른 보정
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
        IsDead = true;  // 거북이는 IsDead == true 여도 필드상에 존재할 수 있다.

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
        // 활동 영역을 설정하는 스크립트로부터 bounds를 전달받는다.
        RespawnBounds = bounds;
    }
    public void SetRecentDir(int targetDir)
    {
        // flip을 시킬지에 대한 값
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

        // 바라보는 방향 변경
        RecentDir = targetDir;

        // 바라보는 방향으로 Flip
        transform.localScale = new Vector3(transform.localScale.x * flipValue, transform.localScale.y, transform.localScale.z);
    }
    public void StartSetRecentDirAfterGrounded(int targetDir)
    {
        StartCoroutine(SetRecentDirAfterGrounded(targetDir));
    }
    private IEnumerator SetRecentDirAfterGrounded(int targetDir)
    {
        // 코루틴을 사용해 방향 전환을 땅을 밟은 후로 설정한다
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
        // includeInactive : true -> 비활성화된 자식 오브젝트도 검색
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
        // 애니메이션 전이 조건이 있는 경우 조건을 만족할 때까지 대기
        if (AnimTransitionEvent != null)
            yield return new WaitUntil(() => AnimTransitionEvent(targetTransitionParam, currentState));

        Animator.SetTrigger(targetTransitionParam);

        // 추가로 실행해야 하는 함수
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
