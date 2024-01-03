using System.Collections;
using System.Threading;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// 몬스터의 기본 행동을 정의
/// </summary>
public abstract class MonsterBehavior : MonoBehaviour
{
    #region Attribute

    // ETC
    public Rigidbody2D RigidBody { get; private set; }
    public Animator Animator { get; private set; }

    [Header("State")]
    [Space]

    [SerializeField] private Monster_StateBase _initialState;
    [SerializeField] private Monster_StateBase _currentState;
    [SerializeField] private Monster_StateBase _previousState;

    [Header("Module")]
    [Space]

    [SerializeField] private WayPointPatrol _wayPointPatrol;
    public WayPointPatrol WayPointPatrol
    {
        get { return _wayPointPatrol; }
    }
    [SerializeField] private NavMeshMove _navMeshMove;
    public NavMeshMove NavMeshMove
    {
        get { return _navMeshMove; }
    }
    [SerializeField] private FloatingPatrolEvaluator _floatingPatrolEvaluator;
    public FloatingPatrolEvaluator FloatingPatrolEvaluator
    {
        get { return _floatingPatrolEvaluator; }
    }
    [SerializeField] private ChaseEvaluator _chaseEvaluator;
    public ChaseEvaluator ChaseEvaluator
    {
        get { return _chaseEvaluator; }
    }
    [SerializeField] private AttackEvaluator _attackEvaluator;
    public AttackEvaluator AttackEvaluator
    {
        get { return _attackEvaluator; }
    }

    [Header("Condition")]
    [Space]

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
    [SerializeField] private bool _isGodMode;
    public bool IsGodMode
    {
        get => _isGodMode;
        set => _isGodMode = value;
    }

    [Header("Monster Data")]
    [Space]

    [SerializeField] private MonsterData _monsterData;

    [Space]

    [SerializeField] private int _id;
    public int ID
    {
        get => _id;
        protected set => _id = value;
    }
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
    [SerializeField] private MonsterDefine.SIZE _monsterSize;
    public MonsterDefine.SIZE MonsterSize
    {
        get => _monsterSize;
        protected set => _monsterSize = value;
    }
    [SerializeField] private MonsterDefine.MONSTER_TYPE _monsterType;
    public MonsterDefine.MONSTER_TYPE MonsterType
    {
        get => _monsterType;
        protected set => _monsterType = value;
    }

    [Header("Blink")]
    [Space]

    [SerializeField] private Material _whiteMaterial;
    [SerializeField] private float _blinkDuration = 0.1f;
    private SpriteRenderer[] _spriteRenderers;
    private Material[] _originalMaterials;
    private Coroutine _blinkRoutine;

    [Header("FadeOut")]
    [Space]

    [SerializeField] private float _targetFadeOutTime = 3f;
    [SerializeField] private float _elapsedFadeOutTime = 0f;

    #endregion

    #region Function

    protected virtual void Awake()
    {
        // Basic Component
        RigidBody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();

        // Module
        _wayPointPatrol = GetComponent<WayPointPatrol>();
        _navMeshMove = GetComponent<NavMeshMove>();
        _floatingPatrolEvaluator = GetComponent<FloatingPatrolEvaluator>();
        _chaseEvaluator = GetComponent<ChaseEvaluator>();
        _attackEvaluator = GetComponent<AttackEvaluator>();

        SaveOriginalMaterial();

        // Init State
        InitState();
    }
    protected virtual void Start()
    {
        // 몬스터 속성 설정
        SetUp();
    }
    protected virtual void Update()
    {
        if (IsDead)
            return;

        CheckDie();
    }
    protected virtual void FixedUpdate()
    {

    }
    protected virtual void SetUp()
    {
        // 몬스터의 ID 설정
        ID = _monsterData.ID;

        // 몬스터의 이름 설정
        MonsterName = _monsterData.MonsterName;

        // 몬스터의 타입 설정
        MonsterType = _monsterData.MonsterType;

        // 몬스터의 최대 체력
        MaxHp = _monsterData.MaxHp;
        CurHp = MaxHp;

        // 몬스터의 이동속도 설정
        MoveSpeed = _monsterData.MoveSpeed;

        // 몬스터의 크기 타입
        MonsterSize = _monsterData.MonsterSize;
    }

    public virtual void KnockBack(Vector2 forceVector)
    {
        RigidBody.velocity = Vector2.zero;

        // Monster의 Mass에 따른 forceVector 보정
        float ratio = RigidBody.mass / 1.0f;
        forceVector *= ratio;

        RigidBody.AddForce(forceVector, ForceMode2D.Impulse);
    }
    public virtual void OnHit(int damage, Vector2 forceVector)
    {
        if (IsGodMode || IsDead)
            return;

        // Damage
        CurHp -= damage;

        // Hit
        StartHitTimer();
        KnockBack(forceVector);
        GetComponent<SoundList>().PlaySFX("SE_Hurt");

        // Change to Die State
        if (CurHp <= 0)
        {
            CurHp = 0;
            Animator.SetTrigger("Die");

            return;
        }

        // Change to Hurt State
        Animator.SetTrigger("Hurt");
    }
    public virtual void Die()
    {
        IsDead = true;

        // Disable Hit Box
        SetActiveHitBox(false);

        // 사라지기 시작
        StartDestroy();
    }

    // hitBox
    public void SetActiveHitBox(bool isBool)
    {
        // Disable Hit Box
        GameObject hitBox = GetComponentInChildren<MonsterBodyHit>(true).gameObject;

        if (hitBox != null)
            hitBox.SetActive(isBool);
    }
    public void SetDisableHitBox(bool isBool)
    {
        MonsterBodyHit hitBox = GetComponentInChildren<MonsterBodyHit>();

        if (hitBox != null)
            hitBox.IsDisableHitBox = isBool;
    }
    public void SetTriggerHitBox(bool isBool)
    {
        // Disable Hit Box
        GameObject hitBox = GetComponentInChildren<MonsterBodyHit>(true).gameObject;

        if (hitBox != null)
        {
            Collider2D hitBoxCollider = hitBox.GetComponent<Collider2D>();
            hitBoxCollider.isTrigger = isBool;
            hitBox.layer = isBool ? LayerMask.NameToLayer("Monster") : LayerMask.NameToLayer("Default");
        }
    }

    // basic
    private void CheckDie()
    {
        if (CurHp <= 0)
        {
            CurHp = 0;
            Animator.SetTrigger("Die");
        }
    }
    private IEnumerator HitTimer()
    {
        IsHit = true;
        yield return new WaitForSeconds(0.01f);
        IsHit = false;
    }
    public void StartHitTimer()
    {
        StartCoroutine(HitTimer());
    }

    // effect
    private void SaveSpriteRenderers()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }
    private void SaveOriginalMaterial()
    {
        SaveSpriteRenderers();

        _originalMaterials = new Material[_spriteRenderers.Length];
        for (int i = 0; i < _originalMaterials.Length; i++)
            _originalMaterials[i] = _spriteRenderers[i].material;
    }
    private void InitMaterial()
    {
        for (int i = 0; i < _spriteRenderers.Length; i++)
            _spriteRenderers[i].material = _originalMaterials[i];
    }
    private IEnumerator Blink()
    {
        while (IsHurt)
        {
            // turn to white material
            for (int i = 0; i < _originalMaterials.Length; i++)
                _spriteRenderers[i].material = _whiteMaterial;

            yield return new WaitForSeconds(_blinkDuration);

            // turn to original material
            for (int i = 0; i < _originalMaterials.Length; i++)
                _spriteRenderers[i].material = _originalMaterials[i];

            yield return new WaitForSeconds(_blinkDuration);
        }
    }
    public void StartBlink()
    {
        if (this._blinkRoutine != null)
        {
            InitMaterial();
            StopCoroutine(this._blinkRoutine);
        }
        this._blinkRoutine = StartCoroutine(Blink());
    }
    private IEnumerator FadeOutDestroy()
    {
        // Bring 모든 SpriteRenderer를 가져온다 from All Child Objects
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        // 초기 알파값 저장
        float[] startAlphaArray = new float[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            startAlphaArray[i] = spriteRenderers[i].color.a;

        // 모든 렌더 컴포넌트를 돌면서 Fade Out
        while (_elapsedFadeOutTime < _targetFadeOutTime)
        {
            _elapsedFadeOutTime += Time.deltaTime;
            float normalizedTime = _elapsedFadeOutTime / _targetFadeOutTime; // Normalize to 0 ~ 1

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                // 현재 스프라이트 렌더러의 알파값을 변경
                Color targetColor = spriteRenderers[i].color;
                targetColor.a = Mathf.Lerp(startAlphaArray[i], 0f, normalizedTime);
                spriteRenderers[i].color = targetColor;
            }

            yield return null;
        }

        // 오브젝트 삭제
        if (transform.root)
            Destroy(transform.root.gameObject);
        else
            Destroy(gameObject);

        yield return null;
    }
    public void StartDestroy()
    {
        StartCoroutine(FadeOutDestroy());
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
    public bool CurrentStateIs<State>() where State : Monster_StateBase
    {
        return _currentState is State;
    }
    public bool PreviousStateIs<State>() where State : Monster_StateBase
    {
        return _previousState is State;
    }

    #endregion
}
