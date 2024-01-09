using System.Collections;
using System.Threading;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 몬스터의 기본 행동을 정의
/// </summary>
public abstract class MonsterBehavior : MonoBehaviour, IAttackListener
{
    #region Attribute

    // Basic Component
    public Rigidbody2D RigidBody { get; private set; }
    public Animator Animator { get; private set; }

    [Header("State")]
    [Space]

    [SerializeField] private Monster_StateBase _currentState;

    [Space]

    [SerializeField] private Monster_StateBase _initialState;
    [SerializeField] private Monster_StateBase _previousState;

    [Header("Module")]
    [Space]

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
    [SerializeField] private GroundPatrolEvaluator _groundPatrolEvaluator;
    public GroundPatrolEvaluator GroundPatrolEvaluator
    {
        get { return _groundPatrolEvaluator; }
    }
    [SerializeField] private FloatingChaseEvaluator _floatingChaseEvaluator;
    public FloatingChaseEvaluator FloatingChaseEvaluator
    {
        get { return _floatingChaseEvaluator; }
    }
    [SerializeField] private AttackEvaluator _attackEvaluator;
    public AttackEvaluator AttackEvaluator
    {
        get { return _attackEvaluator; }
    }

    [Header("Condition")]
    [Space]

    [SerializeField] private int _defaultDir = 1;
    [SerializeField] private int _recentDir;
    public int RecentDir
    {
        get => _recentDir;
        set => _recentDir = value;
    }
    [SerializeField] private bool _isInAir;
    public bool IsInAir
    {
        get => _isInAir;
        set => _isInAir = value;
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
    [SerializeField] private MonsterDefine.MONSTER_LEVEL _monsterLevel;
    public MonsterDefine.MONSTER_LEVEL MonsterLevel
    {
        get => _monsterLevel;
        protected set => _monsterLevel = value;
    }
    [SerializeField] private MonsterDefine.MONSTER_BEHAV _monsterBehav;
    public MonsterDefine.MONSTER_BEHAV MonsterBehav
    {
        get => _monsterBehav;
        protected set => _monsterBehav = value;
    }

    [Header("Ground Check")]
    [Space]

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _groundCheckTrans;
    [SerializeField] private Vector2 _groundCheckBoxSize;
    public RaycastHit2D GroundRayHit;

    // Blink
    private Material _whiteFlashMaterial;
    private Material _superArmorMaterial;
    private float _blinkDuration = 0.08f;
    private SpriteRenderer[] _spriteRenderers;
    private Material[] _originalMaterials;
    private Coroutine _whiteFlashRoutine;
    private Coroutine _superArmorRoutine;

    // Fade Out
    private float _targetFadeOutTime = 2f;
    private float _elapsedFadeOutTime = 0f;

    #endregion

    #region Function

    protected virtual void Awake()
    {
        // Basic Component
        RigidBody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();

        // Module
        _navMeshMove = GetComponent<NavMeshMove>();
        _floatingPatrolEvaluator = GetComponent<FloatingPatrolEvaluator>();
        _groundPatrolEvaluator = GetComponent<GroundPatrolEvaluator>();
        _floatingChaseEvaluator = GetComponent<FloatingChaseEvaluator>();
        _attackEvaluator = GetComponent<AttackEvaluator>();

        // Material
        LoadBlinkMaterial();
        SaveOriginalMaterial();

        // Init State
        InitState();
    }
    protected virtual void Start()
    {
        // 몬스터 속성 설정
        SetUp();

        // 바라보는 방향 설정
        _recentDir = _defaultDir;
    }
    protected virtual void Update()
    {
        if (IsDead)
            return;

        // ground behavior
        if (_monsterBehav == MonsterDefine.MONSTER_BEHAV.Ground)
        {
            if (!_isInAir)
            {
                GroundRayHit = Physics2D.BoxCast(_groundCheckTrans.position, _groundCheckBoxSize, 0f, Vector2.zero, 0f,
                    _groundLayer);
                if (!GroundRayHit)
                {
                    // Ground -> Air
                    _isInAir = true;
                    Animator.SetTrigger("InAir");
                }
            }
            else
            {
                GroundRayHit = Physics2D.BoxCast(_groundCheckTrans.position, _groundCheckBoxSize, 0f, Vector2.zero, 0f,
                    _groundLayer);
                if (GroundRayHit)
                {
                    // Air -> Ground
                    _isInAir = false;
                    Animator.SetTrigger("Idle");
                }
            }

            // change direction
            if (GroundPatrolEvaluator.IsWallCheck())
                UpdateImageFlip();
        }
        // fly behavior
        else if (_monsterBehav == MonsterDefine.MONSTER_BEHAV.Fly)
            _isInAir = true;

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

        // 몬스터의 최대 체력
        MaxHp = _monsterData.MaxHp;
        CurHp = MaxHp;

        // 몬스터의 이동속도 설정
        MoveSpeed = _monsterData.MoveSpeed;

        // 몬스터의 크기 타입
        MonsterSize = _monsterData.MonsterSize;

        // 몬스터의 레벨
        MonsterLevel = _monsterData.MonsterLevel;

        // 몬스터의 행동 타입
        MonsterBehav = _monsterData.MonsterBehav;
    }

    public virtual void KnockBack(Vector2 forceVector)
    {
        RigidBody.velocity = Vector2.zero;

        // Monster의 Mass에 따른 forceVector 보정
        float ratio = RigidBody.mass / 1.0f;
        Vector2 newForceVector = forceVector * ratio;

        var navMesh = GetComponent<NavMeshAgent>();

        // navMesh의 KnockBack
        if (navMesh)
            navMesh.velocity = forceVector / 2.0f;
        // rigidbody의 KnockBack
        else
            RigidBody.AddForce(newForceVector, ForceMode2D.Impulse);
    }

    public virtual void OnHitted(AttackInfo attackInfo)
    {
        if (IsGodMode || IsDead)
            return;

        // Damage
        CurHp -= (int)attackInfo.Damage;

        // Hit
        StartIsHitTimer();
        KnockBack(attackInfo.Force);
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
        // Disable Hit Box
        SetActiveHitBoxGameObject(false);

        // 사라지기 시작
        StartDestroy();
    }

    // hitBox & Collider
    public void SetActiveHitBoxGameObject(bool isBool)
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
            hitBox.layer = isBool ? LayerMask.NameToLayer("MonsterHitBox") : LayerMask.NameToLayer("Default");
        }
    }

    // basic
    public void UpdateImageFlip()
    {
        RecentDir *= -1;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
    private void CheckDie()
    {
        if (CurHp <= 0)
        {
            CurHp = 0;
            Animator.SetTrigger("Die");
        }
    }
    private IEnumerator IsHitTimer()
    {
        IsHit = true;
        yield return new WaitForSeconds(0.1f);
        IsHit = false;
    }
    public void StartIsHitTimer()
    {
        StartCoroutine(IsHitTimer());
    }
    public void EndState()
    {
        Animator.SetTrigger("Idle");
    }

    // effect
    private void LoadBlinkMaterial()
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
        SaveSpriteRenderers();

        _originalMaterials = new Material[_spriteRenderers.Length];
        for (int i = 0; i < _originalMaterials.Length; i++)
            _originalMaterials[i] = _spriteRenderers[i].material;
    }
    private void ResetMaterial()
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
            ResetMaterial();
            StopCoroutine(this._whiteFlashRoutine);
        }
        this._whiteFlashRoutine = StartCoroutine(WhiteFlash());
    }
    private IEnumerator SuperArmorFlash()
    {
        while (IsGodMode)
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
            ResetMaterial();
            StopCoroutine(this._superArmorRoutine);
        }
        this._superArmorRoutine = StartCoroutine(SuperArmorFlash());
    }
    private IEnumerator FadeOutDestroy()
    {
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

        if (transform.root) Destroy(transform.root.gameObject);
        else Destroy(gameObject);

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

    private void OnDrawGizmosSelected()
    {
        // ground behavior
        if (_monsterBehav == MonsterDefine.MONSTER_BEHAV.Ground)
        {
            // Draw Ground Check
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_groundCheckTrans.position, new Vector3(_groundCheckBoxSize.x, _groundCheckBoxSize.y, 0f));
        }
    }


    #endregion
}
