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
    private Monster_StateBase _initialState;
    private Monster_StateBase _previousState;

    [Header("Module")]
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
    [SerializeField] private FloatingPatrolEvaluator _floatingPatrolEvaluator;
    public FloatingPatrolEvaluator FloatingPatrolEvaluator
    {
        get => _floatingPatrolEvaluator;
        private set => _floatingPatrolEvaluator = value;
    }
    [SerializeField] private FloatingChaseEvaluator _floatingChaseEvaluator;
    public FloatingChaseEvaluator FloatingChaseEvaluator
    {
        get => _floatingChaseEvaluator;
        private set => _floatingChaseEvaluator = value;
    }
    [SerializeField] private NavMeshMove _navMeshMove;
    public NavMeshMove NavMeshMove
    {
        get => _navMeshMove;
        private set => _navMeshMove = value;
    }
    [SerializeField] private AttackEvaluator _attackEvaluator;
    public AttackEvaluator AttackEvaluator
    {
        get => _attackEvaluator;
        private set => _attackEvaluator = value;
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
    [SerializeField] private Vector2 _jumpForce;
    public Vector2 JumpForce
    {
        get => _jumpForce;
        protected set => _jumpForce = value;
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
    private readonly float _blinkDuration = 0.08f;
    private SpriteRenderer[] _spriteRenderers;
    private Material[] _originalMaterials;
    private Coroutine _whiteFlashRoutine;
    private Coroutine _superArmorRoutine;

    // Fade Out
    private readonly float _targetFadeOutTime = 2f;
    private float _elapsedFadeOutTime;

    #endregion

    #region Function

    protected virtual void Awake()
    {
        // Basic Component
        RigidBody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();

        // Module
        GroundPatrolEvaluator = GetComponent<GroundPatrolEvaluator>();
        GroundChaseEvaluator = GetComponent<GroundChaseEvaluator>();
        FloatingPatrolEvaluator = GetComponent<FloatingPatrolEvaluator>();
        FloatingChaseEvaluator = GetComponent<FloatingChaseEvaluator>();
        NavMeshMove = GetComponent<NavMeshMove>();
        AttackEvaluator = GetComponent<AttackEvaluator>();

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
        RecentDir = DefaultDir;
    }
    protected virtual void Update()
    {
        if (IsDead)
            return;

        /*
        // ground behavior
        if (MonsterBehav == MonsterDefine.MONSTER_BEHAV.GroundWalk)
        {
            GroundRayHit = Physics2D.BoxCast(_groundCheckTrans.position, _groundCheckBoxSize, 0f, Vector2.zero, 0f,
                _groundLayer);

            // Ground -> Air
            if (IsInAir)
            {
                if (!GroundRayHit)
                {
                    IsGround = false;
                    IsInAir = true;
                    Animator.SetTrigger("InAir");
                }
            }
            // Air -> Ground
            else
            {
                if (GroundRayHit)
                {
                    IsGround = true;
                    IsInAir = false;
                    Animator.SetTrigger("Idle");
                }
            }

            // flip after wall check
            if (GroundPatrolEvaluator.IsWallCheck())
                SetRecentDir(-RecentDir);
        }
        // fly behavior
        else if (MonsterBehav == MonsterDefine.MONSTER_BEHAV.Fly)
        {
            IsGround = false;
            IsInAir = true;
        }
        */

        IsCheckDie();
    }
    protected virtual void FixedUpdate()
    {

    }
    protected virtual void SetUp()
    {
        // 몬스터의 이름
        MonsterName = _monsterData.MonsterName;

        // 몬스터의 최대 체력
        MaxHp = _monsterData.MaxHp;
        CurHp = MaxHp;

        // 몬스터의 이동속도
        MoveSpeed = _monsterData.MoveSpeed;

        // 몬스터의 점프파워
        JumpForce = _monsterData.JumpForce;

        // 몬스터의 행동 타입
        MonsterBehav = _monsterData.MonsterBehav;
    }
    public virtual void KnockBack(Vector2 forceVector)
    {
        var navMesh = GetComponent<NavMeshAgent>();
        if (navMesh)
            navMesh.velocity = forceVector / 2.0f;
        else
        {
            // 속도 초기화
            RigidBody.velocity = Vector2.zero;

            // Monster의 Mass에 따른 보정
            forceVector *= RigidBody.mass / 1.0f;
            RigidBody.AddForce(forceVector, ForceMode2D.Impulse);
        }
    }
    public virtual void OnHit(AttackInfo attackInfo)
    {
        if (IsGodMode || IsDead)
            return;

        // Damage
        CurHp -= (int)attackInfo.Damage;

        // Hit
        StartHitTimer();
        KnockBack(attackInfo.Force);
        GetComponent<SoundList>().PlaySFX("SE_Hurt");

        // Change to Die State
        if (IsCheckDie())
            return;

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

    // hitBox
    public void SetActiveHitBoxGameObject(bool isBool)
    {
        // Disable Hit Box
        GameObject hitBox = GetComponentInChildren<MonsterBodyHit>(true).gameObject;

        if (hitBox)
            hitBox.SetActive(isBool);
    }
    public void SetTriggerHitBox(bool isBool)
    {
        // Disable Hit Box
        GameObject hitBox = GetComponentInChildren<MonsterBodyHit>(true).gameObject;

        if (hitBox)
        {
            Collider2D hitBoxCollider = hitBox.GetComponent<Collider2D>();
            hitBoxCollider.isTrigger = isBool;
            hitBox.layer = isBool ? LayerMask.NameToLayer("MonsterHitBox") : LayerMask.NameToLayer("Default");
        }
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
    private bool IsCheckDie()
    {
        if (CurHp <= 0)
        {
            CurHp = 0;
            Animator.SetTrigger("Die");

            return true;
        }

        return false;
    }

    // blink
    private void LoadBlinkMaterial()
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

    // hit
    private IEnumerator HitTimer()
    {
        IsHit = true;
        yield return new WaitForSeconds(0.1f);
        IsHit = false;
    }
    public void StartHitTimer()
    {
        StartCoroutine(HitTimer());
    }

    // die
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
    public bool CurrentStateIs<TState>() where TState : Monster_StateBase
    {
        return _currentState is TState;
    }
    public bool PreviousStateIs<TState>() where TState : Monster_StateBase
    {
        return _previousState is TState;
    }

    private void OnDrawGizmosSelected()
    {
        // ground behavior
        if (MonsterBehav == MonsterDefine.MONSTER_BEHAV.GroundWalk)
        {
            // Draw Ground Check
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_groundCheckTrans.position, new Vector3(_groundCheckBoxSize.x, _groundCheckBoxSize.y, 0f));
        }
    }

    #endregion
}
