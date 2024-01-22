using System.Collections;
using UnityEngine;

public class Bear : SemiBossBehavior, ILightCaptureListener
{
    public enum BearAttackType
    {
        Null = 0,

        // Normal Attack
        Slash_Right,
        Slash_Left,
        BodySlam,
        Stomp,

        // Special Attack
        EarthQuake = 10
    }

    public class BearAttackInfo
    {
        public float Damage = 1f;
        public Vector2 Force = Vector2.zero;

        public BearAttackInfo(float damage, Vector2 force)
        {
            Damage = damage;
            Force = force;
        }
    }

    [Header("Bear")]
    [Space]

    [SerializeField] private LayerMask _skillTargetLayer;
    [SerializeField] private GameObject _skillHitEffect;

    [Header("Condition")]
    [Space]

    [SerializeField] private BearAttackType _currentAttack;
    [SerializeField] private BearAttackType _nextAttack;

    [Space]

    [SerializeField] private int _minTargetCount;
    [SerializeField] private int _maxTargetCount;

    [Space]

    [SerializeField] private int _targetCount;
    [SerializeField] private int _currentCount;
    [SerializeField] private int _targetHurtCount;
    [SerializeField] private int _currentHurtCount;

    [Header("Skill - Slash")]
    [Space]

    [SerializeField] private BoxCollider2D _slashCollider;
    [SerializeField] private int _slashDamage = 20;
    [SerializeField] private float _slashForceX = 7f;
    [SerializeField] private float _slashForceY = 10f;

    [Header("Skill - Body Slam")]
    [Space]

    [SerializeField] private BoxCollider2D _bodySlamCollider;
    [SerializeField] private bool _isBodySlamming;
    [SerializeField] private int _bodySlamDamage = 20;
    [SerializeField] private float _bodySlamForceX = 7f;
    [SerializeField] private float _bodySlamForceY = 10f;

    [Header("Skill - Stomp")]
    [Space]

    [SerializeField] private BoxCollider2D _stompCollider;
    [SerializeField] private Bear_StalactiteAttack _stalactitePrefab;
    [SerializeField] private int _stalactiteCount = 5;
    [SerializeField] private int _stompDamage = 20;
    [SerializeField] private float _stompForceX = 7f;
    [SerializeField] private float _stompForceY = 10f;

    [Header("Skill - Earthquake")]
    [Space]

    [SerializeField] private BoxCollider2D _earthQuakeCollider;
    [SerializeField] private Bear_GroundWaveAttack _waveSkillPrefab;
    [SerializeField] private int _earthQuakeDamage = 20;
    [SerializeField] private float _earthQuakeForceX = 7f;
    [SerializeField] private float _earthQuakeForceY = 10f;

    private Vector2 _playerPos;
    private BoxCollider2D _bodyCollider;   // not bodyHitBox

    protected override void Awake()
    {
        base.Awake();

        _bodyCollider = GetComponent<BoxCollider2D>();
    }
    protected override void Start()
    {
        base.Start();

        // init
        RandomTargetCount();
        SetToRandomAttack();
    }
    protected override void Update()
    {
        base.Update();

        if (!IsRage)
        {
            if (CurHp <= MaxHp * 0.5f)
            {
                IsRage = true;
                Animator.SetTrigger("Shout");
            }
        }
    }
    protected override void SetUp()
    {
        base.SetUp();
    }
    public override void KnockBack(Vector2 forceVector)
    {
        base.KnockBack(forceVector);
    }
    public override IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        if (IsGodMode || IsDead)
            return IAttackListener.AttackResult.Fail;

        // Hit Process
        StartHitTimer();
        IncreaseHurtCount();
        GetComponent<SoundList>().PlaySFX("SE_Hurt");

        // Hurt
        if (_currentHurtCount >= _targetHurtCount)
        {
            CurHp -= 10000;

            // Die
            if (CurHp <= 0)
            {
                CurHp = 0;
                Animator.SetTrigger("Die");

                return IAttackListener.AttackResult.Success;
            }

            Animator.SetTrigger("Hurt");
            InitializeHurtCount();
        }

        return IAttackListener.AttackResult.Success;
    }
    public override void Die()
    {
        base.Die();
    }

    private void FixedUpdate()
    {
        if (_isBodySlamming)
        {
            BearAttackInfo bodySlamInfo = new BearAttackInfo(_bodySlamDamage, new Vector2(_bodySlamForceX, _bodySlamForceY));
            BearAttack(_bodySlamCollider.transform.position, _bodySlamCollider.bounds.size, bodySlamInfo, _skillTargetLayer);
        }
    }

    public void OnLightEnter(LightCapturer capturer, LightSource lightSource)
    {
        if (IsGroggy)
            return;

        // 그로기 상태로 진입
        Animator.SetTrigger("Groggy");
    }
    public void OnLightStay(LightCapturer capturer, LightSource lightSource)
    {

    }
    public void OnLightExit(LightCapturer capturer, LightSource lightSource)
    {

    }

    // base
    public override void AttackPreProcess()
    {
        // 현재 공격 상태 변경
        _currentAttack = _nextAttack;

        if (_currentAttack is BearAttackType.Null || _nextAttack is BearAttackType.Null)
        {
            Debug.LogError("BearAttackType is Null");
            return;
        }

        if (_currentAttack is BearAttackType.EarthQuake)
            _currentCount = 0;
        else
            _currentCount++;
    }
    public override void AttackPostProcess()
    {
        if (_currentCount >= _targetCount)
        {
            SetToEarthQuake();
            RandomTargetCount();
        }
        else
            SetToRandomAttack();
    }

    // basic
    private void RandomTargetCount()
    {
        if (_minTargetCount > _maxTargetCount)
            Debug.LogError("minTargetCount > maxTargetCount");
        else if (_minTargetCount < 0)
            Debug.LogError("minTargetCount < 0");
        else if (_maxTargetCount <= 0)
            Debug.LogError("maxTargetCount <= 0");

        // 4번 ~ 7번 공격 후 지진 공격
        _targetCount = Random.Range(_minTargetCount, _maxTargetCount);
    }
    private void SetToRandomAttack()
    {
        int nextAttackNumber = Random.Range(1, 5); // 1 ~ 4
        _nextAttack = (BearAttackType)nextAttackNumber;
        Animator.SetInteger("NextAttackNumber", nextAttackNumber);
    }
    private void SetToEarthQuake()
    {
        _nextAttack = BearAttackType.EarthQuake;
        Animator.SetInteger("NextAttackNumber", (int)_nextAttack);
    }
    public void IncreaseHurtCount()
    {
        _currentHurtCount++;
        Animator.SetInteger("HurtCount", _currentHurtCount);
    }
    public void InitializeHurtCount()
    {
        _currentHurtCount = 0;
        Animator.SetInteger("HurtCount", _currentHurtCount);
    }
    public void BearAttack(Vector2 targetPosition, Vector2 attackBoxSize, BearAttackInfo attackinfo, LayerMask targetLayer)
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
                    Instantiate(_skillHitEffect, rayCastHit.point + Random.insideUnitCircle * 0.3f, Quaternion.identity);
            }
        }
    }

    // skill
    public void Slash01_AnimEvent()
    {
        var playerPos = SceneContext.Current.Player.transform.position;

        // 바라보는 방향에 플레이어가 있다면
        if ((RecentDir > 0 && (playerPos.x > transform.position.x)) || (RecentDir < 0 && (playerPos.x < transform.position.x)))
        {
            // 플레이어의 위치를 기억
            _playerPos = playerPos;
        }
    }
    public void Slash02_AnimEvent()
    {
        Debug.DrawRay(_playerPos, new Vector2(0f, _slashCollider.bounds.extents.y), Color.red, 0.15f);
        Debug.DrawRay(_playerPos, new Vector2(0f, -_slashCollider.bounds.extents.y), Color.red, 0.15f);
        Debug.DrawRay(_playerPos, new Vector2(_slashCollider.bounds.extents.x, 0f), Color.red, 0.15f);
        Debug.DrawRay(_playerPos, new Vector2(-_slashCollider.bounds.extents.x, 0f), Color.red, 0.15f);

        BearAttackInfo slashInfo = new BearAttackInfo(_slashDamage, new Vector2(_slashForceX, _slashForceY));
        BearAttack(_playerPos, _slashCollider.bounds.size, slashInfo, _skillTargetLayer);
    }

    public void BodySlam01_AnimEvent()
    {
        var playerPos = SceneContext.Current.Player.transform.position;
        var dir = System.Math.Sign(playerPos.x - transform.position.x);

        SetRecentDir(dir);
    }
    public void BodySlam02_AnimEvent()
    {
        _isBodySlamming = true;
        Rigidbody.AddForce(300f * Vector2.right * RecentDir, ForceMode2D.Impulse);
    }
    public void BodySlam03_AnimEvent()
    {
        Rigidbody.velocity = Vector2.zero;
        _isBodySlamming = false;
    }

    public void Stomp01_AnimEvent()
    {
        BearAttackInfo stompInfo = new BearAttackInfo(_stompDamage, new Vector2(_stompForceX, _stompForceY));
        BearAttack(_stompCollider.transform.position, _stompCollider.bounds.size, stompInfo, _skillTargetLayer);

        // 종유석 생성
        for (int i = 0; i < _stalactiteCount; ++i)
            StartCoroutine(CreateStalactite());
    }
    public IEnumerator CreateStalactite()
    {
        var fallingStartTime = Random.Range(0.2f, 1.5f);

        yield return new WaitForSeconds(fallingStartTime);

        // 종유석을 천장에서 랜덤 위치에 생성한다
        Vector2 randomPos = (Random.value > 0.5f) ? new Vector2(Random.Range(-150f, _bodyCollider.bounds.min.x - 2.0f), 18.3f) : new Vector2(Random.Range(_bodyCollider.bounds.max.x + 2.0f, -125f), 18.3f);
        Instantiate(_stalactitePrefab, randomPos, Quaternion.identity);
    }

    public void Earthquake01_AnimEvent()
    {
        BearAttackInfo earthQuakeInfo = new BearAttackInfo(_earthQuakeDamage, new Vector2(_earthQuakeForceX, _earthQuakeForceY));
        BearAttack(_earthQuakeCollider.transform.position, _earthQuakeCollider.bounds.size, earthQuakeInfo, _skillTargetLayer);

        // 지면파 생성
        GenerateGroundWave();
    }
    public void GenerateGroundWave()
    {
        // 2개의 지면파를 발생시킨다 (좌 / 우)
        var wave1 = Instantiate(_waveSkillPrefab, _earthQuakeCollider.transform.position, Quaternion.identity);
        wave1.SetDir(Vector2.left);
        var wave2 = Instantiate(_waveSkillPrefab, _earthQuakeCollider.transform.position, Quaternion.identity);
        wave2.SetDir(Vector2.right);
    }
}