using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 몬스터의 기본 행동을 정의
/// </summary>
public abstract class MonsterBehavior : MonoBehaviour, IAttackListener
{
    #region Attribute

    public class MonsterAttackInfo
    {
        public float Damage = 1f;
        public Vector2 Force = Vector2.zero;

        public MonsterAttackInfo(float damage, Vector2 force)
        {
            Damage = damage;
            Force = force;
        }
    }

    // Basic Component
    public Rigidbody2D Rigidbody { get; private set; }
    public Animator Animator { get; private set; }

    [Header("MonsterBehavior")]
    [Space]

    [Header("State")]
    [Space]

    [SerializeField] private Monster_StateBase _currentState;
    public Monster_StateBase CurrentState
    {
        get => _currentState;
        private set => _currentState = value;
    }
    private Monster_StateBase _initialState;
    private Monster_StateBase _previousState;

    [Header("Module")]
    [Space]

    [SerializeField] private FloatingPatrolModule _floatingPatrolModule;
    public FloatingPatrolModule FloatingPatrolModule
    {
        get => _floatingPatrolModule;
        private set => _floatingPatrolModule = value;
    }
    [SerializeField] private NavMeshMoveModule _navMeshMoveModule;
    public NavMeshMoveModule NavMeshMoveModule
    {
        get => _navMeshMoveModule;
        private set => _navMeshMoveModule = value;
    }

    [Header("Evaluator")]
    [Space]

    [SerializeField] private GroundPatrolEvaluator _groundPatrolEvaluator;
    public GroundPatrolEvaluator GroundPatrolEvaluator
    {
        get => _groundPatrolEvaluator;
        private set => _groundPatrolEvaluator = value;
    }
    [SerializeField] private GroundChaseEvaluator _groundChaseEvaluator;
    public GroundChaseEvaluator GroundChaseEvaluator
    {
        get => _groundChaseEvaluator;
        private set => _groundChaseEvaluator = value;
    }
    [SerializeField] private FloatingChaseEvaluator _floatingChaseEvaluator;
    public FloatingChaseEvaluator FloatingChaseEvaluator
    {
        get => _floatingChaseEvaluator;
        private set => _floatingChaseEvaluator = value;
    }
    [SerializeField] private AttackEvaluator _attackEvaluator;
    public AttackEvaluator AttackEvaluator
    {
        get => _attackEvaluator;
        private set => _attackEvaluator = value;
    }
    [SerializeField] private CautionEvaluator _cautionEvaluator;
    public CautionEvaluator CautionEvaluator
    {
        get => _cautionEvaluator;
        private set => _cautionEvaluator = value;
    }

    [Header("Condition")]
    [Space]

    [SerializeField] private int _defaultDir = 1;
    public int DefaultDir
    {
        get => _defaultDir;
        private set => _defaultDir = value;
    }
    [SerializeField] private int _recentDir;
    public int RecentDir
    {
        get => _recentDir;
        set => _recentDir = value;
    }
    [SerializeField] private bool _isGround;
    public bool IsGround
    {
        get => _isGround;
        set => _isGround = value;
    }
    [SerializeField] private bool _isInAir;
    public bool IsInAir
    {
        get => _isInAir;
        set => _isInAir = value;
    }
    [SerializeField] private bool _isSuperArmor;
    public bool IsSuperArmor
    {
        get => _isSuperArmor;
        set => _isSuperArmor = value;
    }
    [SerializeField] private bool _isGodMode;
    public bool IsGodMode
    {
        get => _isGodMode;
        set => _isGodMode = value;
    }
    [SerializeField] private bool _isHit;
    public bool IsHit
    {
        get => _isHit;
        set => _isHit = value;
    }
    [SerializeField] private bool _isHurt;
    public bool IsHurt
    {
        get => _isHurt;
        set => _isHurt = value;
    }
    [SerializeField] private bool _isDead;
    public bool IsDead
    {
        get => _isDead;
        set => _isDead = value;
    }

    [Header("Monster Data")]
    [Space]

    [SerializeField] private MonsterData _monsterData;

    [Space]

    [SerializeField] private string _monsterName;
    public string MonsterName
    {
        get => _monsterName;
        protected set => _monsterName = value;
    }
    [SerializeField] private int _maxHp;
    public int MaxHp
    {
        get => _maxHp;
        protected set => _maxHp = value;
    }
    [SerializeField] private int _curHp;
    public int CurHp
    {
        get => _curHp;
        set => _curHp = value;
    }
    [SerializeField] private float _moveSpeed;
    public float MoveSpeed
    {
        get => _moveSpeed;
        protected set => _moveSpeed = value;
    }
    [SerializeField] private float _acceleration;
    public float Acceleration
    {
        get => _acceleration;
        protected set => _acceleration = value;
    }
    [SerializeField] private Vector2 _jumpForce;
    public Vector2 JumpForce
    {
        get => _jumpForce;
        protected set => _jumpForce = value;
    }
    [SerializeField] private MonsterDefine.RankType _rankType;
    public MonsterDefine.RankType RankType
    {
        get => _rankType;
        protected set => _rankType = value;
    }
    [SerializeField] private MonsterDefine.MoveType _moveType;
    public MonsterDefine.MoveType MoveType
    {
        get => _moveType;
        protected set => _moveType = value;
    }

    [Header("Basic Attack")]
    [Space]

    [SerializeField] protected LayerMask _attackTargetLayer;
    [SerializeField] protected GameObject _attackHitEffect;

    [Header("Ground Check")]
    [Space]

    [SerializeField] private LayerMask _groundCheckLayer;
    [SerializeField] private BoxCollider2D _groundCheckCollider;
    public RaycastHit2D GroundRayHit;

    // Blink
    private Material _whiteFlashMaterial;
    private Material _superArmorMaterial;
    private readonly float _blinkDuration = 0.08f;
    private SpriteRenderer[] _spriteRenderers;
    private Material[] _originalMaterials;
    private Coroutine _whiteFlashRoutine;
    private Coroutine _superArmorRoutine;

    // Fade Out
    private readonly float _targetFadeOutTime = 2f;
    private float _elapsedFadeOutTime;

    // animation transition condition
    public delegate bool AnimationTransitionCondition(Monster_StateBase state);
    public AnimationTransitionCondition AnimTransitionCondition;

    #endregion

    #region Function

    protected virtual void Awake()
    {
        // Basic Component
        Rigidbody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();

        // Module
        FloatingPatrolModule = GetComponent<FloatingPatrolModule>();
        NavMeshMoveModule = GetComponent<NavMeshMoveModule>();

        // Evaluator
        GroundPatrolEvaluator = GetComponent<GroundPatrolEvaluator>();
        GroundChaseEvaluator = GetComponent<GroundChaseEvaluator>();
        FloatingChaseEvaluator = GetComponent<FloatingChaseEvaluator>();
        AttackEvaluator = GetComponent<AttackEvaluator>();
        CautionEvaluator = GetComponent<CautionEvaluator>();

        // Material
        LoadFlashMaterial();
        SaveOriginalMaterial();

        // Init State
        InitState();
    }
    protected virtual void Start()
    {
        // 몬스터 속성 설정
        SetUp();

        // 바라보는 방향 설정
        RecentDir = DefaultDir;
    }
    protected virtual void Update()
    {
        if (IsDead)
            return;

        // condition
        switch (MoveType)
        {
            case MonsterDefine.MoveType.GroundTurret:
            case MonsterDefine.MoveType.GroundWalking:
            case MonsterDefine.MoveType.GroundJumpping:

                // ground rayCast
                RaycastHit2D[] groundRayHits = Physics2D.BoxCastAll(_groundCheckCollider.transform.position,
                    _groundCheckCollider.bounds.size, 0f, Vector2.zero, 0f,
                    _groundCheckLayer);

                // groundRayHits는 몬스터와 지면이 충돌한 지점에 대한 정보이다.
                // 이 중, 가장 가까운 지점을 GroundRayHit에 저장한다.
                foreach (var hit in groundRayHits)
                {
                    // hit의 normal이 아래쪽을 향하면 안된다.
                    if (hit.normal.y < 0)
                        continue;

                    // 충돌 지점과 이 오브젝트와의 거리가 가장 가까운 놈을 저장
                    if (GroundRayHit)
                    {
                        float newDist = Vector2.Distance(transform.position, hit.point);
                        float oldDist = Vector2.Distance(transform.position, GroundRayHit.point);

                        if (newDist < oldDist)
                            GroundRayHit = hit;
                    }
                    else
                        GroundRayHit = hit;
                }

                bool hasGroundContact = groundRayHits.Length > 0;

                // set condition
                IsGround = hasGroundContact;
                IsInAir = !hasGroundContact;

                // reDirectable state
                if (CurrentStateIs<Monster_IdleState>() || CurrentStateIs<GroundPatrolState>())
                {
                    // set recentDir for patrol
                    if (GroundPatrolEvaluator)
                    {
                        // out patrol range
                        if (GroundPatrolEvaluator.IsOutOfPatrolRange())
                        {
                            if (GroundPatrolEvaluator.IsLeftOfLeftPoint())
                                StartSetRecentDirAfterGrounded(1);
                            else if (GroundPatrolEvaluator.IsRightOfRightPoint())
                                StartSetRecentDirAfterGrounded(-1);
                        }
                        // in patrol range
                        else
                        {
                            if (GroundPatrolEvaluator.IsTargetWithinRange())
                                StartSetRecentDirAfterGrounded(-RecentDir);
                        }
                    }

                    // set recentDir for chase
                    if (GroundChaseEvaluator)
                    {
                        if (GroundChaseEvaluator.IsTargetWithinRange())
                            SetRecentDir(GroundChaseEvaluator.ChaseDir);
                    }
                }

                break;

            case MonsterDefine.MoveType.Fly:

                IsGround = false;
                IsInAir = true;

                break;
        }

        // range based attack by attack evaluator
        if (AttackEvaluator)
        {
            if (AttackEvaluator.IsTargetWithinRange())
            {
                AttackEvaluator.StartCoolTimeCoroutine();
                StartChangeStateCoroutine("Attack", CurrentState);
            }
        }
    }
    protected virtual void SetUp()
    {
        // 몬스터의 이름
        MonsterName = _monsterData.MonsterName.ToString();

        // 몬스터의 최대 체력
        MaxHp = _monsterData.MaxHp;
        CurHp = MaxHp;

        // 몬스터의 이동속도
        MoveSpeed = _monsterData.MoveSpeed;

        // 몬스터의 가속도
        Acceleration = _monsterData.Acceleration;

        // 몬스터의 점프파워
        JumpForce = _monsterData.JumpForce;

        // 몬스터의 랭크
        RankType = _monsterData.RankType;

        // 몬스터의 행동 타입
        MoveType = _monsterData.MoveType;
    }
    public virtual void KnockBack(Vector2 forceVector)
    {
        var navMesh = GetComponent<NavMeshAgent>();

        if (navMesh)
            navMesh.velocity = forceVector / 1.5f;
        else
        {
            // 속도 초기화
            Rigidbody.velocity = Vector2.zero;

            // Monster의 Mass에 따른 보정
            forceVector *= Rigidbody.mass / 1.0f;
            Rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
        }
    }
    public virtual IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        if (IsGodMode || IsDead)
            return IAttackListener.AttackResult.Fail;

        // Hit Process
        HitProcess(attackInfo);

        // Check Hurt or Die Process
        CheckDieProcess();

        return IAttackListener.AttackResult.Success;
    }
    public virtual void Die()
    {
        // Disable Hit Box
        TurnOffHitBox();

        // death effect
        StartCoroutine(DeathCoroutine());
    }

    // Effect
    private IEnumerator DeathCoroutine()
    {
        yield return StartCoroutine(DeathEffectCoroutine());

        if (transform.root) Destroy(transform.root.gameObject);
        else Destroy(gameObject);
    }
    protected virtual IEnumerator DeathEffectCoroutine()
    {
        // Fade Out Effect

        SpriteRenderer[] currentSpriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        float[] startAlphaArray = new float[currentSpriteRenderers.Length];
        for (int i = 0; i < currentSpriteRenderers.Length; i++)
            startAlphaArray[i] = currentSpriteRenderers[i].color.a;

        while (_elapsedFadeOutTime < _targetFadeOutTime)
        {
            _elapsedFadeOutTime += Time.deltaTime;
            float normalizedTime = _elapsedFadeOutTime / _targetFadeOutTime; // Normalize to 0 ~ 1

            for (int i = 0; i < currentSpriteRenderers.Length; i++)
            {
                Color targetColor = currentSpriteRenderers[i].color;
                targetColor.a = Mathf.Lerp(startAlphaArray[i], 0f, normalizedTime);
                currentSpriteRenderers[i].color = targetColor;
            }

            yield return null;
        }

        yield return null;
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
    public void BoxCastAttack(Vector2 targetPosition, Vector2 attackBoxSize, MonsterAttackInfo attackinfo, LayerMask targetLayer)
    {
        RaycastHit2D[] rayCastHits = Physics2D.BoxCastAll(targetPosition, attackBoxSize, 0f, Vector2.zero, 0.0f, targetLayer);
        foreach (var rayCastHit in rayCastHits)
        {
            var listeners = rayCastHit.rigidbody.GetComponents<IAttackListener>();
            foreach (var listener in listeners)
            {
                var forceVector = new Vector2(attackinfo.Force.x * Mathf.Sign(rayCastHit.transform.position.x - transform.position.x), attackinfo.Force.y);
                var attackResult = listener.OnHit(new AttackInfo(attackinfo.Damage, forceVector, AttackType.Monster_SkillAttack));

                if (attackResult == IAttackListener.AttackResult.Success)
                    Instantiate(_attackHitEffect, rayCastHit.point + Random.insideUnitCircle * 0.3f, Quaternion.identity);
            }
        }
    }

    // hitBox
    public void TurnOffHitBox()
    {
        GameObject hitBox = GetComponentInChildren<MonsterBodyHitModule>().gameObject;

        if (hitBox)
            hitBox.SetActive(false);
    }
    public void TurnToCollisionHitBox()
    {
        GameObject hitBox = GetComponentInChildren<MonsterBodyHitModule>().gameObject;

        if (hitBox)
        {
            Collider2D hitBoxCollider = hitBox.GetComponent<Collider2D>();
            hitBoxCollider.isTrigger = false;
            hitBox.layer = LayerMask.NameToLayer("Default");
        }
    }
    public void SetIsAttackableHitBox(bool isBool)
    {
        var monsterBodyHitModule = GetComponentInChildren<MonsterBodyHitModule>();

        if (monsterBodyHitModule)
            monsterBodyHitModule.IsAttackable = isBool;
    }
    public void SetIsHurtableHitBox(bool isBool)
    {
        var monsterBodyHitModule = GetComponentInChildren<MonsterBodyHitModule>();

        if (monsterBodyHitModule)
            monsterBodyHitModule.IsHurtable = isBool;
    }
    public IEnumerator AttackableHitBox(bool isBool)
    {
        yield return new WaitForSeconds(0.1f);

        SetIsAttackableHitBox(isBool);
    }
    public void StartAttackableHitBox()
    {
        StartCoroutine(AttackableHitBox(true));
    }
    public IEnumerator HurtableHitBox(bool isBool)
    {
        yield return new WaitForSeconds(0.1f);

        SetIsHurtableHitBox(isBool);
    }
    public void StartHurtableHitBox()
    {
        StartCoroutine(HurtableHitBox(true));
    }

    // flash
    private void LoadFlashMaterial()
    {
        _whiteFlashMaterial =
            Resources.Load<Material>("Materials/WhiteFlashMaterial");
        _superArmorMaterial =
            Resources.Load<Material>("Materials/SuperArmorFlashMaterial");
    }
    private void SaveOriginalMaterial()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        _originalMaterials = new Material[_spriteRenderers.Length];
        for (int i = 0; i < _originalMaterials.Length; i++)
            _originalMaterials[i] = _spriteRenderers[i].material;
    }
    private void ResetOriginalMaterial()
    {
        for (int i = 0; i < _spriteRenderers.Length; i++)
            _spriteRenderers[i].material = _originalMaterials[i];
    }
    private IEnumerator WhiteFlash()
    {
        while (IsHurt)
        {
            // turn to white material
            for (int i = 0; i < _originalMaterials.Length; i++)
                _spriteRenderers[i].material = _whiteFlashMaterial;

            yield return new WaitForSeconds(_blinkDuration);

            // turn to original material
            for (int i = 0; i < _originalMaterials.Length; i++)
                _spriteRenderers[i].material = _originalMaterials[i];

            yield return new WaitForSeconds(_blinkDuration);
        }
    }
    public void StartWhiteFlash()
    {
        if (this._whiteFlashRoutine != null)
        {
            ResetOriginalMaterial();
            StopCoroutine(this._whiteFlashRoutine);
        }
        this._whiteFlashRoutine = StartCoroutine(WhiteFlash());
    }
    private IEnumerator SuperArmorFlash()
    {
        while (IsSuperArmor)
        {
            // turn to white material
            for (int i = 0; i < _originalMaterials.Length; i++)
                _spriteRenderers[i].material = _superArmorMaterial;

            yield return new WaitForSeconds(_blinkDuration);

            // turn to original material
            for (int i = 0; i < _originalMaterials.Length; i++)
                _spriteRenderers[i].material = _originalMaterials[i];

            yield return new WaitForSeconds(_blinkDuration);
        }
    }
    public void StartSuperArmorFlash()
    {
        if (this._superArmorRoutine != null)
        {
            ResetOriginalMaterial();
            StopCoroutine(this._superArmorRoutine);
        }
        this._superArmorRoutine = StartCoroutine(SuperArmorFlash());
    }

    // hit & die
    public void HitProcess(AttackInfo attackInfo, bool onDamage = true, bool onKnockBack = true)
    {
        StartHitTimer();
        GetComponent<SoundList>().PlaySFX("SE_Hurt");

        if (onDamage)
            CurHp -= (int)attackInfo.Damage;

        if (onKnockBack)
            KnockBack(attackInfo.Force);
    }
    public void CheckDieProcess()
    {
        if (CurHp <= 0)
        {
            CurHp = 0;
            Animator.SetTrigger("Die");

            return;
        }

        // superArmor is started when monster attack
        // superArmor : hurt animation x
        if (IsSuperArmor)
            return;

        Animator.SetTrigger("Hurt");
    }
    private IEnumerator HitTimer()
    {
        IsHit = true;
        yield return new WaitForSeconds(0.05f);
        IsHit = false;
    }
    public void StartHitTimer()
    {
        StartCoroutine(HitTimer());
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
        _currentState = _initialState;
        _previousState = _initialState;
    }
    public void UpdateState(Monster_StateBase state)
    {
        _previousState = _currentState;
        _currentState = state;
    }
    public bool CurrentStateIs<TState>() where TState : Monster_StateBase
    {
        return _currentState is TState;
    }
    public bool PreviousStateIs<TState>() where TState : Monster_StateBase
    {
        return _previousState is TState;
    }
    private IEnumerator ChangeStateCoroutine(string targetTransitionParam, Monster_StateBase state)
    {
        // 애니메이션 전이 조건이 있는 경우 조건을 만족할 때까지 대기
        if(AnimTransitionCondition != null)
            yield return new WaitUntil(() => AnimTransitionCondition(state));

        Animator.SetTrigger(targetTransitionParam);
    }
    public void StartChangeStateCoroutine(string targetTransitionParam, Monster_StateBase state)
    {
        StartCoroutine(ChangeStateCoroutine(targetTransitionParam, state));
    }

    // behavior
    public void GroundWalking()
    {
        if (IsInAir)
            return;

        Vector2 groundNormal = GroundRayHit.normal;
        Vector2 moveDirection = RecentDir > 0
            ? (-1) * Vector2.Perpendicular(groundNormal)
            : Vector2.Perpendicular(groundNormal);

        Debug.DrawRay(GroundRayHit.point, groundNormal);

        Vector2 targetVelocity = moveDirection * MoveSpeed;
        Vector2 velocityNeeded = targetVelocity - Vector2.Dot(Rigidbody.velocity, moveDirection) * moveDirection;
        Vector2 moveForce = velocityNeeded * Acceleration;

        Debug.DrawRay(transform.position, moveForce);

        Rigidbody.AddForce(moveForce);
    }

    #endregion
}
