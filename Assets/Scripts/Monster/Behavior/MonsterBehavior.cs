using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

/// <summary>
/// ������ �⺻ �ൿ�� �����ϴ� �߻�Ŭ����
/// </summary>
public abstract class MonsterBehavior : MonoBehaviour, IAttackListener
{
    #region Attribute

    public Rigidbody2D Rigidbody { get; private set; }
    public Animator Animator { get; private set; }

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
    [SerializeField] private MonsterMovementModule _monsterMovementModule;
    public MonsterMovementModule MonsterMovementModule
    {
        get => _monsterMovementModule;
        private set => _monsterMovementModule = value;
    }
    [SerializeField] private NavMeshMovementModule navMeshMovementModule;
    public NavMeshMovementModule NavMeshMovementModule
    {
        get => navMeshMovementModule;
        private set => navMeshMovementModule = value;
    }

    [Header("Evaluator")]
    [Space]

    [SerializeField] private GroundPatrolEvaluator groundPatrolEvaluator;
    public GroundPatrolEvaluator GroundPatrolEvaluator
    {
        get => groundPatrolEvaluator;
        private set => groundPatrolEvaluator = value;
    }
    [SerializeField] private GroundChaseEvaluator groundChaseEvaluator;
    public GroundChaseEvaluator GroundChaseEvaluator
    {
        get => groundChaseEvaluator;
        private set => groundChaseEvaluator = value;
    }
    [SerializeField] private FloatingChaseEvaluator floatingChaseEvaluator;
    public FloatingChaseEvaluator FloatingChaseEvaluator
    {
        get => floatingChaseEvaluator;
        private set => floatingChaseEvaluator = value;
    }
    [SerializeField] private AttackEvaluator attackEvaluator;
    public AttackEvaluator AttackEvaluator
    {
        get => attackEvaluator;
        private set => attackEvaluator = value;
    }
    [SerializeField] private CautionEvaluator cautionEvaluator;
    public CautionEvaluator CautionEvaluator
    {
        get => cautionEvaluator;
        private set => cautionEvaluator = value;
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
    [SerializeField] private bool _isAttacking;
    public bool IsAttacking
    {
        get => _isAttacking;
        set => _isAttacking = value;
    }
    [SerializeField] private bool _isHiding;
    public bool IsHiding
    {
        get => _isHiding;
        set => _isHiding = value;
    }
    [SerializeField] private bool _isGodMode;
    public bool IsGodMode
    {
        get => _isGodMode;
        set => _isGodMode = value;
    }
    [SerializeField] private bool _isHitting;
    public bool IsHitting
    {
        get => _isHitting;
        set => _isHitting = value;
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

    [SerializeField] private Transform _centerOfMass;
    [SerializeField] private MonsterData _monsterData;

    [Space]

    [SerializeField] private string _monsterName;
    public string MonsterName
    {
        get => _monsterName;
        set => _monsterName = value;
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
        set
        {
            _curHp = value;

            if (_curHp <= 0)
            {
                _curHp = 0;
                Die();
            }
        }
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

    [Header("Ground Check")]
    [Space]

    [SerializeField] private LayerMask _groundCheckLayer;
    [SerializeField] private BoxCollider2D _groundCheckCollider;
    public RaycastHit2D GroundRayHit;

    [Header("Basic BoxCast Attack")]
    [Space]

    [SerializeField] protected LayerMask _attackTargetLayer;
    [SerializeField] protected GameObject _attackHitEffect;

    // blink Effect
    private BlinkEffect _blinkEffect;

    // animation transition event
    public delegate bool CustomAnimTransitionEvent(string targetTransitionParam, Monster_StateBase state);
    public CustomAnimTransitionEvent customAnimTransitionEvent;

    // box cast attack event
    public delegate void CustomBasicBoxCastAttackEvent();
    public CustomBasicBoxCastAttackEvent customBasicBoxCastAttackEvent;

    #endregion

    #region Function

    protected virtual void Awake()
    {
        // Basic Component
        Rigidbody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();

        // Module
        FloatingPatrolModule = GetComponent<FloatingPatrolModule>();
        MonsterMovementModule = GetComponent<MonsterMovementModule>();
        NavMeshMovementModule = GetComponent<NavMeshMovementModule>();

        // Evaluator
        GroundPatrolEvaluator = GetComponent<GroundPatrolEvaluator>();
        GroundChaseEvaluator = GetComponent<GroundChaseEvaluator>();
        FloatingChaseEvaluator = GetComponent<FloatingChaseEvaluator>();
        AttackEvaluator = GetComponent<AttackEvaluator>();
        CautionEvaluator = GetComponent<CautionEvaluator>();

        _blinkEffect = GetComponent<BlinkEffect>();

        // Init State
        InitState();
    }
    protected virtual void Start()
    {
        // ���� �Ӽ� ����
        SetUp();

        // �ٶ󺸴� ���� ����
        RecentDir = DefaultDir;

        // �����߽� ����
        if (!_centerOfMass) _centerOfMass = this.transform;
        Rigidbody.centerOfMass = _centerOfMass.localPosition;
    }
    protected virtual void Update()
    {
        if (IsDead)
            return;

        // ground check
        switch (MoveType)
        {
            case MonsterDefine.MoveType.GroundTurret:
            case MonsterDefine.MoveType.GroundWalking:
            case MonsterDefine.MoveType.GroundJumpping:

                // ground rayCast
                RaycastHit2D[] groundRayHits = Physics2D.BoxCastAll(_groundCheckCollider.transform.position,
                    _groundCheckCollider.bounds.size, 0f, Vector2.zero, 0f,
                    _groundCheckLayer);

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

            if (CurrentState is IAttackableState)
            {
                if (AttackEvaluator.IsTargetWithinRange())
                {
                    AttackEvaluator.StartEvaluatorCoolTime();
                    StartChangeStateCoroutine("Attack", CurrentState);
                }
            }
        }
    }
    protected virtual void FixedUpdate()
    {

    }
    public virtual void SetUp()
    {
        // ������ �̸�
        MonsterName = _monsterData.MonsterName.ToString();

        // ������ ��ũ
        RankType = _monsterData.RankType;

        // ������ �ൿ Ÿ��
        MoveType = _monsterData.MoveType;

        // ������ �ִ� ü��
        MaxHp = _monsterData.MaxHp;
        CurHp = MaxHp;

        // ������ �̵��ӵ�
        MoveSpeed = _monsterData.MoveSpeed;

        // ������ ���ӵ�
        Acceleration = _monsterData.Acceleration;

        // ������ �����Ŀ�
        JumpForce = _monsterData.JumpForce;
    }
    public virtual void KnockBack(Vector2 forceVector)
    {
        var navMesh = GetComponent<NavMeshAgent>();
        if (navMesh) navMesh.velocity = forceVector / 2.0f;
        else
        {
            // �ӵ� �ʱ�ȭ
            Rigidbody.velocity = Vector2.zero;

            // Monster�� Mass�� ���� ����
            forceVector *= Rigidbody.mass / 1.0f;
            Rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
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
    public virtual void Die()
    {
        IsDead = true;

        // Check Die Trigger
        foreach (AnimatorControllerParameter param in Animator.parameters)
        {
            if (param.name == "Die" && param.type == AnimatorControllerParameterType.Trigger)
                Animator.SetTrigger("Die");
        }

        DisableHitBox();

        DeathEffect();
    }

    // Effect
    public void DeathEffect()
    {
        // death effect
        StartCoroutine(DeathCoroutine());
    }
    private IEnumerator DeathCoroutine()
    {
        // DeathEffectCoroutine�� ���� ������ ���
        yield return StartCoroutine(DeathEffectCoroutine());

        if (transform.root) Destroy(transform.root.gameObject);
        else Destroy(gameObject);
    }
    protected virtual IEnumerator DeathEffectCoroutine()
    {
        var effect = GetComponent<DisintegrateEffect>();
        yield return new WaitForSeconds(0.3f);  // �ڿ������� ȿ���� ���� ���

        // NavMesh Agent Stop movement
        var navMeshMoveModule = GetComponent<NavMeshMovementModule>();
        if (navMeshMoveModule) navMeshMoveModule.SetStopAgent(true, true);

        // Generic Stop movement
        Rigidbody.simulated = false;
        Animator.speed = 0;

        // wait until effect done
        effect.Play();
        yield return new WaitUntil(() => effect.IsEffectDone);
    }

    // basic
    public void SetRecentDir(int targetDir)
    {
        // flip�� ��ų���� ���� ��
        int flipValue = RecentDir * targetDir;

        // �ٶ󺸴� ���� ����
        RecentDir = targetDir;

        // �ٶ󺸴� �������� Flip
        transform.localScale = new Vector3(transform.localScale.x * flipValue, transform.localScale.y, transform.localScale.z);
    }
    private IEnumerator SetRecentDirAfterGrounded(int targetDir)
    {
        // �ڷ�ƾ�� ����� ���� ��ȯ�� ���� ���� �ķ� �����Ѵ�
        yield return new WaitUntil(() => IsGround);

        SetRecentDir(targetDir);
    }
    public void StartSetRecentDirAfterGrounded(int targetDir)
    {
        StartCoroutine(SetRecentDirAfterGrounded(targetDir));
    }
    public void BasicBoxCastAttack(Vector2 targetPosition, Vector2 attackBoxSize, MonsterAttackInfo attackinfo, LayerMask targetLayer)
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
                    customBasicBoxCastAttackEvent?.Invoke();
                }
            }
        }
    }
    public void ColliderCastAttack(Collider2D collider, MonsterAttackInfo attackinfo, LayerMask targetLayer)
    {
        // layer wrapping to contactFilter
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(targetLayer);

        // collider cast
        List<RaycastHit2D> rayCastHits = new List<RaycastHit2D>();
        collider.Cast(Vector2.right, contactFilter, rayCastHits, 0);

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
                    customBasicBoxCastAttackEvent?.Invoke();
                }
            }
        }
    }

    // hitBox
    public void DisableHitBox()
    {
        var hitBox = GetComponentInChildren<MonsterBodyHit>();

        if (hitBox)
            hitBox.gameObject.SetActive(false);
    }
    public void GroundizeHitBox()
    {
        var hitBox = GetComponentInChildren<MonsterBodyHit>();

        if (hitBox)
        {
            Collider2D hitBoxCollider = hitBox.GetComponent<Collider2D>();
            hitBoxCollider.isTrigger = false;
            hitBox.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }
    public void SetAttackableHitBox(bool isBool)
    {
        // var str = isBool ? "Attackable" : "UnAttackable";
        // Debug.Log(str);

        var monsterBodyHitModule = GetComponentInChildren<MonsterBodyHit>();

        if (monsterBodyHitModule)
            monsterBodyHitModule.IsAttackable = isBool;
    }

    // hit & die
    public void HitProcess(AttackInfo attackInfo, bool onDamage = true, bool onKnockBack = true, bool useBlinkEffect = true)
    {
        StartCoroutine(HitTimer());
        GetComponent<SoundList>().PlaySFX("SE_Hurt");

        if (useBlinkEffect)
        {
            if (_blinkEffect)
                _blinkEffect.StartBlink();
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

        Animator.SetTrigger("Hurt");
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
        // �ִϸ��̼� ���� ������ �ִ� ��� ������ ������ ������ ���
        if (customAnimTransitionEvent != null)
            yield return new WaitUntil(() => customAnimTransitionEvent(targetTransitionParam, currentState));

        Animator.SetTrigger(targetTransitionParam);
    }
    public void StartChangeStateCoroutine(string targetTransitionParam, Monster_StateBase currentState)
    {
        StartCoroutine(ChangeStateCoroutine(targetTransitionParam, currentState));
    }

    #endregion
}
