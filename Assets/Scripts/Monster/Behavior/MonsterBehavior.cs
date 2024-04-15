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
    /// cf) struct는 간단하고, 불변성을 가지며, 원본에 동적으로 수정할 필요가 없는 데이터를 저장할 때 사용
    /// </summary>
    [System.Serializable]
    public class MonsterData
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
    public SoundList SoundList { get; private set; }

    // State
    public Monster_StateBase CurrentState { get; private set; }
    private Monster_StateBase _initialState;
    private Monster_StateBase _previousState;

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
    public int RecentDir
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

    [field: Header("Monster Data")]
    [field: Space]

    [field: SerializeField]
    public Transform CenterOfMass
    {
        get;
        private set;
    }

    [field: Space]

    [field: SerializeField]
    public MonsterDataObject MonsterDataObject
    {
        get;
        private set;
    }
    public MonsterData monsterData;
    [SerializeField] private int _curHp;
    public int CurHp
    {
        get => _curHp;
        set
        {
            _curHp = value;

            if (_curHp <= 0)
            {
                _curHp = 0;
                Die(true, true);        // 기본적으로 몬스터 사망 시, HitBox 비활성화, DeathEffect 실행
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
    public BlinkEffect BlinkEffect
    {
        get;
        private set;
    }

    // animation transition event
    public delegate bool AnimTransitionEvent(string targetTransitionParam, Monster_StateBase state);
    public event AnimTransitionEvent animTransitionEvent;

    #endregion

    #region Function

    protected virtual void Awake()
    {
        // Basic Component
        RigidBody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        MainBodyCollider2D = GetComponent<Collider2D>();
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

        // ETC
        BlinkEffect = GetComponent<BlinkEffect>();

        // Init State
        InitState();
    }
    protected virtual void Start()
    {
        // 몬스터 속성 설정
        SetUp();

        // 바라보는 방향 설정
        RecentDir = DefaultDir;

        // 현재 체력 설정
        CurHp = monsterData.MaxHp;

        // 무게중심 설정
        if (!CenterOfMass) CenterOfMass = this.transform;
        RigidBody2D.centerOfMass = CenterOfMass.localPosition;
    }
    protected virtual void Update()
    {
        if (IsDead)
            return;

        // ground check
        switch (monsterData.MoveType)
        {
            case MonsterDefine.MoveType.GroundTurret:
            case MonsterDefine.MoveType.GroundWalking:
            case MonsterDefine.MoveType.GroundJumpping:

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
                    // Debug.Log("공격 범위에 대상이 존재");

                    StartChangeStateCoroutine("Attack", CurrentState);

                    // TODO: 쿨타임 부분을 어떻게 해당 State가 끝난 후 시작하도록 할지 고민을 해보자
                    AttackEvaluator.StartEvaluatorCoolTime();
                }
            }
        }
    }

    public virtual void SetUp()
    {
        // 몬스터의 이름
        monsterData.MonsterName = MonsterDataObject.Name.ToString();

        // 몬스터의 랭크
        monsterData.RankType = MonsterDataObject.RankType;

        // 몬스터의 행동 타입
        monsterData.MoveType = MonsterDataObject.MoveType;

        // 몬스터의 최대 체력
        monsterData.MaxHp = MonsterDataObject.MaxHp;

        // 몬스터의 이동속도
        monsterData.MoveSpeed = MonsterDataObject.MoveSpeed;

        // 몬스터의 가속도
        monsterData.Acceleration = MonsterDataObject.Acceleration;

        // 몬스터의 점프파워
        monsterData.JumpForce = MonsterDataObject.JumpForce;
    }
    public virtual void KnockBack(Vector2 forceVector)
    {
        var navMesh = GetComponent<NavMeshAgent>();
        if (navMesh) navMesh.velocity = forceVector / 2.0f;
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
    public virtual void Die(bool isHitBoxDisable = true, bool isDeathEffect = true)
    {
        IsDead = true;

        // Check that Animator has Die Trigger Param
        foreach (AnimatorControllerParameter param in Animator.parameters)
        {
            if (param.name == "Die" && param.type == AnimatorControllerParameterType.Trigger)
                SetAnimatorTrigger("Die");
        }

        // Collider 비활성화
        DisableCollider(isHitBoxDisable);

        if (isDeathEffect)
            DeathEffect();
    }

    // Effect
    private void DeathEffect()
    {
        // death effect
        StartCoroutine(DeathEffectCoroutine());
    }
    protected virtual IEnumerator DeathEffectCoroutine()
    {
        var effect = GetComponent<DisintegrateEffect_New>();
        yield return new WaitForSeconds(0.2f);  // 자연스러운 효과를 위한 대기

        // NavMesh Agent Stop movement
        var navMeshMoveModule = GetComponent<FloatingMovementModule>();
        if (navMeshMoveModule) navMeshMoveModule.SetStopAgent(true, true);

        // Generic Stop movement
        RigidBody2D.simulated = false;
        Animator.speed = 0;

        // wait until effect done
        effect.Play();
        yield return new WaitUntil(() => effect.IsEffectDone);
        Destroy(transform.root ? transform.root.gameObject : gameObject);
    }

    // basic
    public void SetRecentDir(int targetDir)
    {
        // flip을 시킬지에 대한 값
        int flipValue = RecentDir * targetDir;

        // 바라보는 방향 변경
        RecentDir = targetDir;

        // 바라보는 방향으로 Flip
        transform.localScale = new Vector3(transform.localScale.x * flipValue, transform.localScale.y, transform.localScale.z);
    }
    private IEnumerator SetRecentDirAfterGrounded(int targetDir)
    {
        // 코루틴을 사용해 방향 전환을 땅을 밟은 후로 설정한다
        yield return new WaitUntil(() => IsGround);

        SetRecentDir(targetDir);
    }
    public void StartSetRecentDirAfterGrounded(int targetDir)
    {
        StartCoroutine(SetRecentDirAfterGrounded(targetDir));
    }

    // control hitBox & collider
    public void DisableCollider(bool isHitBoxDisable)
    {
        var colliders = GetComponentsInChildren<Collider2D>(true);
        foreach (var collider in colliders)
        {
            // Main Body Collider2D를 생략하는 옵션
            if (collider == MainBodyCollider2D) continue;

            if (!isHitBoxDisable)
            {
                // HitBox Collider를 생략하는 옵션
                var hitBox = collider.GetComponent<MonsterBodyHit>();
                if (hitBox != null) continue;
            }

            collider.enabled = false;
        }
    }
    public void SetHitBoxActive(bool isBool)
    {
        // includeInactive : true -> 비활성화된 자식 오브젝트도 검색
        var hitBox = GetComponentInChildren<MonsterBodyHit>(true);

        if (hitBox)
            hitBox.gameObject.SetActive(isBool);
    }
    public void SetHitBoxStepable(bool isBool)
    {
        var hitBox = GetComponentInChildren<MonsterBodyHit>(true);

        if (hitBox)
        {
            if (isBool)
            {
                Collider2D hitBoxCollider = hitBox.GetComponent<Collider2D>();
                hitBoxCollider.isTrigger = false;
                hitBox.gameObject.layer = LayerMask.NameToLayer("Default");
            }
            else
            {
                Collider2D hitBoxCollider = hitBox.GetComponent<Collider2D>();
                hitBoxCollider.isTrigger = true;
                hitBox.gameObject.layer = LayerMask.NameToLayer("MonsterHitBox");
            }
        }
    }
    public void SetHitBoxAttackable(bool isBool)
    {
        var hitBox = GetComponentInChildren<MonsterBodyHit>(true);

        if (hitBox)
            hitBox.IsAttackable = isBool;
    }

    // hit
    public void HitProcess(AttackInfo attackInfo, bool onDamage = true, bool onKnockBack = true, bool useBlinkEffect = true)
    {
        StartCoroutine(HitTimer());
        PlaySound("Hurt");

        if (useBlinkEffect)
        {
            if (BlinkEffect)
                BlinkEffect.Play();
            else
                Debug.LogWarning("Blink Effect isn't attached");
        }

        if (onKnockBack)
            KnockBack(attackInfo.Force);

        if (onDamage)
            CurHp -= (int)attackInfo.Damage;
    }
    public void HurtProcess()
    {
        if (IsDead) return;

        // superArmor is started when monster attack
        // superArmor : hurt animation x
        if (IsAttacking) return;

        SetAnimatorTrigger("Hurt");
    }
    private IEnumerator HitTimer()
    {
        IsHitting = true;
        yield return new WaitForSeconds(0.05f);
        IsHitting = false;
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
        _previousState = _initialState;
    }
    public void UpdateState(Monster_StateBase state)
    {
        _previousState = CurrentState;
        CurrentState = state;
    }
    public bool CurrentStateIs<TState>() where TState : Monster_StateBase
    {
        return CurrentState is TState;
    }
    public bool PreviousStateIs<TState>() where TState : Monster_StateBase
    {
        return _previousState is TState;
    }
    private IEnumerator ChangeStateCoroutine(string targetTransitionParam, Monster_StateBase currentState)
    {
        // 애니메이션 전이 조건이 있는 경우 조건을 만족할 때까지 대기
        if (animTransitionEvent != null)
            yield return new WaitUntil(() => animTransitionEvent(targetTransitionParam, currentState));

        Animator.SetTrigger(targetTransitionParam);
    }
    public void StartChangeStateCoroutine(string targetTransitionParam, Monster_StateBase currentState)
    {
        StartCoroutine(ChangeStateCoroutine(targetTransitionParam, currentState));
    }

    // Animator
    public void SetAnimatorTrigger(string key)
    {
        Animator.SetTrigger(key);
    }
    // Sound
    public void PlaySound(string key)
    {
        SoundList.PlaySFX(key);
    }

    #endregion
}
