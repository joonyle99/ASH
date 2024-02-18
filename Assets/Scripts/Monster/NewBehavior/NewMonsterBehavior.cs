using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 몬스터의 기본 행동을 정의
/// </summary>
public class NewMonsterBehavior : StateMachineBase, IAttackListener
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

    [Header("MonsterBehavior")]
    [Space]

    [SerializeField] private Transform _centerOfMass;

    [Space]

    [Header("Condition")]
    [Space]

    [SerializeField] private int _defaultDir = 1;
    [SerializeField] private int _recentDir;
    [SerializeField] private bool _isGround;
    [SerializeField] private bool _isInAir;
    [SerializeField] private bool _isSuperArmor;
    [SerializeField] private bool _isHide;
    [SerializeField] private bool _isGodMode;
    [SerializeField] private bool _isHit;
    [SerializeField] private bool _isHurt;
    [SerializeField] private bool _isDead;

    [Header("Monster Data")]
    [Space]

    [SerializeField] private MonsterData _monsterData;

    [Space]

    [SerializeField] private string _monsterName;
    [SerializeField] private int _maxHp;
    [SerializeField] private int _curHp;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private Vector2 _jumpForce;
    [SerializeField] private MonsterDefine.RankType _rankType;
    [SerializeField] private MonsterDefine.MoveType _moveType;

    [Header("BoxCast Attack")]
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
    private readonly float _flashInterval = 0.08f;
    private readonly float _flashDuration = 0.9f;
    private bool _isFlashing;
    private SpriteRenderer[] _spriteRenderers;
    private Material[] _originalMaterials;
    private Coroutine _flashTimerRoutine;
    private Coroutine _whiteFlashRoutine;
    private Coroutine _superArmorRoutine;

    #endregion

    #region Property

    public int DefaultDir
    {
        get => _defaultDir;
        private set => _defaultDir = value;
    }
    public int RecentDir
    {
        get => _recentDir;
        set => _recentDir = value;
    }
    public bool IsGround
    {
        get => _isGround;
        set => _isGround = value;
    }
    public bool IsInAir
    {
        get => _isInAir;
        set => _isInAir = value;
    }
    public bool IsSuperArmor
    {
        get => _isSuperArmor;
        set => _isSuperArmor = value;
    }
    public bool IsHide
    {
        get => _isHide;
        set => _isHide = value;
    }
    public bool IsGodMode
    {
        get => _isGodMode;
        set => _isGodMode = value;
    }
    public bool IsHit
    {
        get => _isHit;
        set => _isHit = value;
    }
    public bool IsHurt
    {
        get => _isHurt;
        set => _isHurt = value;
    }
    public bool IsDead
    {
        get => _isDead;
        set => _isDead = value;
    }


    public string MonsterName
    {
        get => _monsterName;
        set => _monsterName = value;
    }
    public int MaxHp
    {
        get => _maxHp;
        set => _maxHp = value;
    }
    public int CurHp
    {
        get => _curHp;
        set => _curHp = value;
    }
    public float MoveSpeed
    {
        get => _moveSpeed;
        set => _moveSpeed = value;
    }
    public float Acceleration
    {
        get => _acceleration;
        set => _acceleration = value;
    }
    public Vector2 JumpForce
    {
        get => _jumpForce;
        set => _jumpForce = value;
    }
    public MonsterDefine.RankType RankType
    {
        get => _rankType;
        protected set => _rankType = value;
    }
    public MonsterDefine.MoveType MoveType
    {
        get => _moveType;
        protected set => _moveType = value;
    }

    #endregion

    #region Function

    protected override void Awake()
    {
        base.Awake();

        // Sprite Renderer / Original Material
        LoadFlashMaterial();
        SaveSpriteRenderers();
        SaveOriginalMaterial();
    }
    protected override void Start()
    {
        base.Start();

        // 몬스터 속성 설정
        SetUp();

        // 바라보는 방향 설정
        RecentDir = DefaultDir;

        // 무게중심 설정
        if (!_centerOfMass) _centerOfMass = this.transform;
        Rigidbody.centerOfMass = _centerOfMass.localPosition;
    }
    protected override void Update()
    {
        base.Update();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected void SetUp()
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

    // Flash
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

    // Hurt / Die
    public virtual void KnockBack(Vector2 forceVector)
    {
        var navMesh = GetComponent<NavMeshAgent>();

        if (navMesh)
            navMesh.velocity = forceVector / 2.0f;
        else
        {
            // 속도 초기화
            Rigidbody.velocity = Vector2.zero;

            // Monster의 Mass에 따른 보정
            forceVector *= Rigidbody.mass / 1.0f;
            Rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
        }
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

    // hitBox
    public void TurnOffHitBox()
    {
        var hitBox = GetComponentInChildren<MonsterBodyHitModule>();

        if (hitBox)
            hitBox.gameObject.SetActive(false);
    }

    #endregion
}
