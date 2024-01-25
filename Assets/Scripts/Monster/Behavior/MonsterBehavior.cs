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
    [SerializeField] private bool _isHide;
    public bool IsHide
    {
        get => _isHide;
        set => _isHide = value;
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
        set => _monsterName = value;
    }
    [SerializeField] private int _maxHp;
    public int MaxHp
    {
        get => _maxHp;
        set => _maxHp = value;
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
        set => _moveSpeed = value;
    }
    [SerializeField] private float _acceleration;
    public float Acceleration
    {
        get => _acceleration;
        set => _acceleration = value;
    }
    [SerializeField] private Vector2 _jumpForce;
    public Vector2 JumpForce
    {
        get => _jumpForce;
        set => _jumpForce = value;
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

    [Header("BoxCast Attack")]
    [Space]

    [SerializeField] protected LayerMask _attackTargetLayer;
    [SerializeField] protected GameObject _attackHitEffect;

    [Header("Ground Check")]
    [Space]

    [SerializeField] private LayerMask _groundCheckLayer;
    [SerializeField] private BoxCollider2D _groundCheckCollider;
    [SerializeField] private Collider2D _groundRayHitCollider;
    public RaycastHit2D GroundRayHit;


    [Space]

    [SerializeField] private Transform _centerOfMass;

    // Blink
    private Material _whiteFlashMaterial;
    private Material _superArmorMaterial;
    private readonly float _flashInterval = 0.08f;
    private readonly float _flashDuration = 0.9f;
    private bool _isFlashing;
    private SpriteRenderer[] _spriteRenderers;
    private Material[] _originalMaterials;
    private Coroutine _flashTimerRoutine;
    private Coroutine _whiteFlashRoutine;
    private Coroutine _superArmorRoutine;

    // Fade Out
    private readonly float _targetFadeOutTime = 2f;
    private float _elapsedFadeOutTime;

    // animation transition event
    public delegate bool CustomAnimTransitionEvent(string targetTransitionParam, Monster_StateBase state);
    public CustomAnimTransitionEvent customAnimTransitionEvent;

    // box cast attack event
    public delegate void CustomBoxCastAttackEvent();
    public CustomBoxCastAttackEvent customBoxCastAttackEvent;

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

        // Sprite Renderer / Original Material
        LoadFlashMaterial();
        SaveSpriteRenderers();
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

        // 무게중심 설정
        if (!_centerOfMass) _centerOfMass = this.transform;
        Rigidbody.centerOfMass = _centerOfMass.localPosition;
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

                // set groundRayHitCollider
                if (GroundRayHit)
                    _groundRayHitCollider = GroundRayHit.collider;

                // set condition
                IsGround = hasGroundContact;
                IsInAir = !hasGroundContact;

                break;

            case MonsterDefine.MoveType.Fly:

                IsGround = false;
                IsInAir = true;

                break;
        }

        // range based attack by attack evaluator
        if (AttackEvaluator)
        {
            if (CurrentState is IAttackableState)
            {
                if (AttackEvaluator.IsTargetWithinRange())
                {
                    AttackEvaluator.StartCoolTimeCoroutine();
                    StartChangeStateCoroutine("Attack", CurrentState);
                }
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
        IsDead = true;

        // Check Die Trigger
        foreach (AnimatorControllerParameter param in Animator.parameters)
        {
            if (param.name == "Die" && param.type == AnimatorControllerParameterType.Trigger)
                Animator.SetTrigger("Die");
        }

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
        var effect = GetComponent<DisintegrateEffect>();
        yield return new WaitForSeconds(0.3f);

        Rigidbody.simulated = false;
        Animator.speed = 0;

        // Stop movement
        var navMeshMoveModule = GetComponent<NavMeshMoveModule>();
        if (navMeshMoveModule)
            navMeshMoveModule.SetStopAgent(true, true);

        effect.Play();
        yield return new WaitUntil(() => effect.IsEffectDone);

        /*
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
        */
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
                {
                    Instantiate(_attackHitEffect, rayCastHit.point + Random.insideUnitCircle * 0.3f, Quaternion.identity);
                    customBoxCastAttackEvent?.Invoke();
                }
            }
        }
    }

    // hitBox
    public void TurnOffHitBox()
    {
        var hitBox = GetComponentInChildren<MonsterBodyHitModule>();

        if (hitBox)
            hitBox.gameObject.SetActive(false);
    }
    public void TurnToCollisionHitBox()
    {
        var hitBox = GetComponentInChildren<MonsterBodyHitModule>();

        if (hitBox)
        {
            Collider2D hitBoxCollider = hitBox.GetComponent<Collider2D>();
            hitBoxCollider.isTrigger = false;
            hitBox.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }
    public void SetIsAttackableHitBox(bool isBool)
    {
        var str = isBool ? "Attackable" : "UnAttackable";
        Debug.Log(str);

        var monsterBodyHitModule = GetComponentInChildren<MonsterBodyHitModule>();

        if (monsterBodyHitModule)
            monsterBodyHitModule.IsAttackable = isBool;
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

    // flash
    private void LoadFlashMaterial()
    {
        _whiteFlashMaterial =
            Resources.Load<Material>("Materials/WhiteFlashMaterial");
        _superArmorMaterial =
            Resources.Load<Material>("Materials/SuperArmorFlashMaterial");
    }
    private void SaveSpriteRenderers()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }
    private void SaveOriginalMaterial()
    {
        _originalMaterials = new Material[_spriteRenderers.Length];

        for (int i = 0; i < _originalMaterials.Length; i++)
            _originalMaterials[i] = _spriteRenderers[i].material;
    }
    private void InitMaterial()
    {
        // Debug.Log($"{this.gameObject.name}의 InitMaterial");

        for (int i = 0; i < _spriteRenderers.Length; i++)
            _spriteRenderers[i].material = _originalMaterials[i];
    }
    private void ChangeMaterial(Material material)
    {
        for (int i = 0; i < _originalMaterials.Length; i++)
            _spriteRenderers[i].material = material;
    }
    private IEnumerator WhiteFlash()
    {
        // turn to white material
        ChangeMaterial(_whiteFlashMaterial);

        while (_isFlashing)
        {
            foreach (var spriteRenderer in _spriteRenderers)
                spriteRenderer.material.SetFloat("_FlashAmount", 0.3f);

            yield return new WaitForSeconds(_flashInterval);

            foreach (var spriteRenderer in _spriteRenderers)
                spriteRenderer.material.SetFloat("_FlashAmount", 0f);

            yield return new WaitForSeconds(_flashInterval);
        }

        // TODO : Dead 상태에서 WhiteFlash가 호출되는 일은 없겠지만, 혹시 모르니까
        if (!IsDead)
            InitMaterial();
    }
    public void StartWhiteFlash()
    {
        if (this._superArmorRoutine != null)
            StopCoroutine(this._superArmorRoutine);

        if (this._whiteFlashRoutine != null)
            StopCoroutine(this._whiteFlashRoutine);

        this._whiteFlashRoutine = StartCoroutine(WhiteFlash());
    }
    private IEnumerator SuperArmorFlash()
    {
        // turn to white material
        ChangeMaterial(_superArmorMaterial);

        while (_isFlashing)
        {
            foreach (var spriteRenderer in _spriteRenderers)
                spriteRenderer.material.SetFloat("_FlashAmount", 0.2f);

            yield return new WaitForSeconds(_flashInterval);

            foreach (var spriteRenderer in _spriteRenderers)
                spriteRenderer.material.SetFloat("_FlashAmount", 0f);

            yield return new WaitForSeconds(_flashInterval);

            yield return null;
        }

        if (!IsDead)
            InitMaterial();
    }
    public void StartSuperArmorFlash()
    {
        if (this._whiteFlashRoutine != null)
            StopCoroutine(this._whiteFlashRoutine);

        if (this._superArmorRoutine != null)
            StopCoroutine(this._superArmorRoutine);

        this._superArmorRoutine = StartCoroutine(SuperArmorFlash());
    }

    // hit & die
    public void HitProcess(AttackInfo attackInfo, bool onDamage = true, bool onKnockBack = true, bool onWhiteFlash = true)
    {
        StartHitTimer();
        GetComponent<SoundList>().PlaySFX("SE_Hurt");

        if (onWhiteFlash)
        {
            StartFlashTimer();
            StartWhiteFlash();
        }

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
            Die();

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
    private IEnumerator FlashTimer(float duration)
    {
        _isFlashing = true;
        yield return new WaitForSeconds(duration);
        _isFlashing = false;
    }
    public void StartFlashTimer(float duration = 0f)
    {
        if (_flashTimerRoutine != null)
            StopCoroutine(_flashTimerRoutine);

        if (duration < 0.01f)
            duration = _flashDuration;

        _flashTimerRoutine = StartCoroutine(FlashTimer(duration));
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
    private IEnumerator ChangeStateCoroutine(string targetTransitionParam, Monster_StateBase currentState)
    {
        // 애니메이션 전이 조건이 있는 경우 조건을 만족할 때까지 대기
        if (customAnimTransitionEvent != null)
            yield return new WaitUntil(() => customAnimTransitionEvent(targetTransitionParam, currentState));

        Animator.SetTrigger(targetTransitionParam);
    }
    public void StartChangeStateCoroutine(string targetTransitionParam, Monster_StateBase currentState)
    {
        StartCoroutine(ChangeStateCoroutine(targetTransitionParam, currentState));
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

        Debug.DrawRay(GroundRayHit.point, groundNormal, Color.cyan);

        Vector2 targetVelocity = moveDirection * MoveSpeed;
        Vector2 velocityNeeded = targetVelocity - Vector2.Dot(Rigidbody.velocity, moveDirection) * moveDirection;   // 경사면을 따라 움직이기 위한 벡터
        Vector2 moveForce = velocityNeeded * Acceleration;

        Debug.DrawRay(transform.position, moveDirection, Color.cyan);

        Rigidbody.AddForce(moveForce);
    }

    #endregion
}
