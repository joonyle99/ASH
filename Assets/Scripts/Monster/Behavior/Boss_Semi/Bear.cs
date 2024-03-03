using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
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

    [Header("Bear")]
    [Space]

    [Header("Condition")]
    [Space]

    [SerializeField] private int _finalTargetHurtCount;

    [Space]

    [SerializeField] private BearAttackType _currentAttack;
    [SerializeField] private BearAttackType _nextAttack;
    [Tooltip("1 : Slash Right\r\n2 : Slash Left\r\n3 : BodySlam\r\n4 : Stomp")]
    [SerializeField] private Range _randomAttackRange;

    [Space]

    [SerializeField] private int _minTargetAttackCount;
    [SerializeField] private int _maxTargetAttackCount;

    [Space]

    [SerializeField] private int _targetAttackCount;
    [SerializeField] private int _currentAttackCount;
    [SerializeField] private int _targetHurtCount;
    [SerializeField] private int _currentHurtCount;

    [Header("Skill")]
    [Space]

    [SerializeField] private Bear_Slash _slash1Prefab;
    [SerializeField] private Bear_Slash _slash2Prefab;

    [Space]

    [SerializeField] private BoxCollider2D _bodySlamCollider;
    [SerializeField] private bool _isBodySlamming;

    [Space]

    [SerializeField] private int _bodySlamDamage = 20;
    [SerializeField] private float _bodySlamForceX = 7f;
    [SerializeField] private float _bodySlamForceY = 10f;

    [Header("Stomp skill")]

    [SerializeField] private Collider2D _stompCollider;
    [SerializeField] private Bear_Stalactite _stalactitePrefab;
    [SerializeField] private GameObject _stompEffectPrefab;

    [Space]

    [SerializeField] private int _stalactiteCount;
    [SerializeField] private Range _createTimeRange;
    [SerializeField] private Range _createSizeRange;

    [Space]

    [SerializeField] private float _ceilingHeight = 18.3f;
    [SerializeField] private Range _distanceRange;

    [Space]

    [SerializeField] private Range _normalStalactiteRange;
    [SerializeField] private Range _rageStalactiteRange;

    [Space]

    [SerializeField] private int _stompDamage = 20;
    [SerializeField] private float _stompForceX = 7f;
    [SerializeField] private float _stompForceY = 10f;

    [Header("Earthquake skill")]

    [SerializeField] private BoxCollider2D _earthQuakeCollider;
    [SerializeField] private Transform _waveSpawnPoint;
    [SerializeField] private Bear_GroundWave _waveSkillPrefab;
    [SerializeField] private ShakePreset _earthquakeCameraShake;

    [Space]

    [SerializeField] private int _earthQuakeDamage = 20;
    [SerializeField] private float _earthQuakeForceX = 7f;
    [SerializeField] private float _earthQuakeForceY = 10f;

    [SerializeField] private SoundList _soundList;

    [SerializeField] private BoxCollider2D _bodyCollider;           // not bodyHitBox
    [SerializeField] private GameObject LightingStone;              // �̸��� ���� ����
    [SerializeField] private ParticleSystem[] DisintegrateEffects;      // ���� ȿ�� ��ƼŬ

    private Vector2 _playerPos;
    private int healtUnit = 10000;

    protected override void Awake()
    {
        base.Awake();

        _bodyCollider = GetComponent<BoxCollider2D>();
    }
    protected override void Start()
    {
        base.Start();

        // init
        RandomTargetAttackCount();
        SetToRandomAttack();
        SetStalactiteToNormalCount();
    }
    protected override void Update()
    {
        base.Update();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (IsDead)
            return;

        if (CurrentStateIs<GroundMoveState>())
        {
            // ground walking
            GroundWalking();
        }

        if (CurrentStateIs<SemiBoss_AttackState>())
        {
            if (_isBodySlamming)
            {
                MonsterAttackInfo bodySlamInfo = new MonsterAttackInfo(_bodySlamDamage, new Vector2(_bodySlamForceX, _bodySlamForceY));
                BoxCastAttack(_bodySlamCollider.transform.position, _bodySlamCollider.bounds.size, bodySlamInfo, _attackTargetLayer);
                // Debug.Log("BodySlam Attack");
            }
        }
    }
    protected override void SetUp()
    {
        base.SetUp();

        MaxHp = _finalTargetHurtCount * healtUnit;
        CurHp = MaxHp;
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
        HitProcess(attackInfo, false, false);

        // ��� ���� �ǰ� ó��
        CurHp -= healtUnit;
        IncreaseHurtCount();

        // Die
        if (CurHp <= 0)
        {
            CurHp = 0;
            Die();

            return IAttackListener.AttackResult.Success;
        }

        // Rage
        if (!IsRage)
        {
            if (CurHp <= MaxHp * 0.5f)
            {
                Animator.SetTrigger("Shout");

                return IAttackListener.AttackResult.Success;
            }
        }

        // Hurt
        if (_currentHurtCount >= _targetHurtCount)
            Animator.SetTrigger("Hurt");

        return IAttackListener.AttackResult.Success;
    }
    public override void Die()
    {
        IsDead = true;

        Animator.SetTrigger("Die");

        // Disable Hit Box
        TurnOffHitBox();
    }

    public void OnLightEnter(LightCapturer capturer, LightSource lightSource)
    {
        if (IsGroggy)
            return;

        // �׷α� ���·� ����
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
        // ���� ���� ���� ����
        _currentAttack = _nextAttack;

        if (_currentAttack is BearAttackType.Null || _nextAttack is BearAttackType.Null)
        {
            Debug.LogError("BearAttackType is Null");
            return;
        }

        if (_currentAttack is BearAttackType.EarthQuake)
            InitializeAttackCount();
        else
            IncreaseAttackCount();
    }
    public override void AttackPostProcess()
    {
        if (_currentAttackCount >= _targetAttackCount)
        {
            SetToEarthQuake();
            RandomTargetAttackCount();
        }
        else
            SetToRandomAttack();
    }
    public override void GroggyPreProcess()
    {
        // �׷α� ���� ���� (���̻� �������� ������ ���� ����)
        IsGroggy = true;

        // ������ MonsterBodyHit�� ���� (�÷��̾ Ÿ���� �� ����)
        SetIsAttackableHitBox(false);
    }
    public override void GroggyPostProcess()
    {
        // �׷α� ���� ���� (���� �������� ������ ����)
        IsGroggy = false;

        // ������ MonsterBodyHit�� �Ҵ� (�÷��̾ Ÿ���� �� �ִ�)
        SetIsAttackableHitBox(true);

        InitializeHurtCount();

        if (IsRage)
            SetStalactiteToRageCount();
    }

    // basic
    private void RandomTargetAttackCount()
    {
        if (_minTargetAttackCount > _maxTargetAttackCount)
            Debug.LogError("minTargetCount > maxTargetCount");
        else if (_minTargetAttackCount < 0)
            Debug.LogError("minTargetCount < 0");
        else if (_maxTargetAttackCount <= 0)
            Debug.LogError("maxTargetCount <= 0");

        // 4�� ~ 7�� ���� �� ���� ����
        _targetAttackCount = Random.Range(_minTargetAttackCount, _maxTargetAttackCount);
    }
    private void SetToRandomAttack()
    {
        int nextAttackNumber = (int)_randomAttackRange.Random();
        _nextAttack = (BearAttackType)nextAttackNumber;
        Animator.SetInteger("NextAttackNumber", nextAttackNumber);
    }
    private void SetToEarthQuake()
    {
        _nextAttack = BearAttackType.EarthQuake;
        Animator.SetInteger("NextAttackNumber", (int)_nextAttack);
    }
    public void IncreaseAttackCount()
    {
        _currentAttackCount++;
    }
    public void InitializeAttackCount()
    {
        _currentAttackCount = 0;
    }
    public void IncreaseHurtCount()
    {
        _currentHurtCount++;
    }
    public void InitializeHurtCount()
    {
        _currentHurtCount = 0;
    }
    public void SetStalactiteToNormalCount()
    {
        _stalactiteCount = Random.Range((int)_normalStalactiteRange.Start, (int)_normalStalactiteRange.End + 1);
    }
    public void SetStalactiteToRageCount()
    {
        _stalactiteCount = Random.Range((int)_rageStalactiteRange.Start, (int)_rageStalactiteRange.End + 1);
    }

    // slash
    public void Slash01_AnimEvent()
    {
        // �÷��̾��� ��ġ
        var playerPos = SceneContext.Current.Player.transform.position;

        // ���Ͱ� �÷��̾ �ٶ󺸴� ����
        var dirBearToPlayer = System.Math.Sign(playerPos.x - transform.position.x);
        // ���Ͱ� �ٶ󺸴� ���⿡ �÷��̾ �ִ���
        bool isPlayerinLookDirection = (dirBearToPlayer == RecentDir);

        // �ٶ󺸴� ���⿡ �÷��̾ �ִٸ�
        if (isPlayerinLookDirection)
        {
            // �÷��̾��� ��ġ�� ���
            _playerPos = playerPos;
        }
    }
    public void Slash02_AnimEvent()
    {
        // TODO : ���⼭ ������ ����
        var slashEffect = Instantiate(_slash1Prefab, _playerPos, Quaternion.identity);
        //Destroy(slashEffect.gameObject, 0.25f);
        if (RecentDir < 1)
            slashEffect.transform.localScale = new Vector3(-Mathf.Abs(slashEffect.transform.localScale.x), slashEffect.transform.localScale.y, slashEffect.transform.localScale.z);

        _soundList.PlaySFX("Slash");
        // �÷��̾� ��ġ �ʱ�ȭ
        _playerPos = Vector2.zero;
    }
    public void Slash02_02_AnimEvent()
    {
        // TODO : ���⼭ ������ ����
        var slashEffect = Instantiate(_slash2Prefab, _playerPos, Quaternion.identity);
        if (RecentDir < 1)
            slashEffect.transform.localScale = new Vector3(-Mathf.Abs(slashEffect.transform.localScale.x), slashEffect.transform.localScale.y, slashEffect.transform.localScale.z);
        //Destroy(slashEffect.gameObject, 0.25f);

        _soundList.PlaySFX("Slash");
        // �÷��̾� ��ġ �ʱ�ȭ
        _playerPos = Vector2.zero;
    }

    // bodySlam
    public void BodySlam01_AnimEvent()
    {
        var playerPos = SceneContext.Current.Player.transform.position;
        var dir = System.Math.Sign(playerPos.x - transform.position.x);
        SetRecentDir(dir);
        // StartSetRecentDirAfterGrounded(dir);
    }
    public void BodySlam02_AnimEvent()
    {
        _isBodySlamming = true;
        Rigidbody.AddForce(60f * Vector2.right * RecentDir, ForceMode2D.Impulse);
    }
    public void BodySlam03_AnimEvent()
    {
        Rigidbody.velocity = Vector2.zero;
        _isBodySlamming = false;
    }

    // stomp
    public void StompEffect()
    {
        Instantiate(_stompEffectPrefab, _stompCollider.transform.position, Quaternion.identity);
        _soundList.PlaySFX("Stomp");
    }
    public void Stomp01_AnimEvent()
    {
        // stomp attack
        MonsterAttackInfo stompInfo = new MonsterAttackInfo(_stompDamage, new Vector2(_stompForceX, _stompForceY));
        CastAttack(_stompCollider, _stompCollider.transform.position, _stompCollider.bounds.size, stompInfo, _attackTargetLayer);
        StompEffect();

        // create stalactite
        for (int i = 0; i < _stalactiteCount; ++i)
            StartCoroutine(CreateStalactite());
    }
    public IEnumerator CreateStalactite()
    {
        yield return new WaitForSeconds(_createTimeRange.Random());

        // random pos range
        float posXInRange = (RecentDir > 0)
            ? Random.Range(_bodyCollider.bounds.max.x + _distanceRange.Start,
                _bodyCollider.bounds.max.x + _distanceRange.End)
            : Random.Range(_bodyCollider.bounds.min.x - _distanceRange.End,
                _bodyCollider.bounds.min.x - _distanceRange.Start);
        Vector2 position = new Vector2(posXInRange, _ceilingHeight);

        // create stalactite
        var stalactite = Instantiate(_stalactitePrefab, position, Quaternion.identity);

        // random size
        stalactite.transform.localScale *= _createSizeRange.Random();
    }

    // earthQuake
    public void Earthquake01_AnimEvent()
    {
        MonsterAttackInfo earthQuakeInfo = new MonsterAttackInfo(_earthQuakeDamage, new Vector2(_earthQuakeForceX, _earthQuakeForceY));
        BoxCastAttack(_earthQuakeCollider.transform.position, _earthQuakeCollider.bounds.size, earthQuakeInfo, _attackTargetLayer);

        // ������ ����
        GenerateGroundWave();
        _soundList.PlaySFX("Earthquake");
        SceneEffectManager.Current.Camera.StartShake(_earthquakeCameraShake);
    }
    public void GenerateGroundWave()
    {
        // 2���� �����ĸ� �߻���Ų�� (�� / ��)
        var wave1 = Instantiate(_waveSkillPrefab, _waveSpawnPoint.position, Quaternion.identity);
        wave1.SetDir(Vector2.left);
        var wave2 = Instantiate(_waveSkillPrefab, _waveSpawnPoint.position, Quaternion.identity);
        wave2.SetDir(Vector2.right);
    }

    public void PlaySound_AnimEvent(string key)
    {
        _soundList.PlaySFX(key);
    }

    public void StartDisintegrateEffect()
    {
        foreach (var effect in DisintegrateEffects)
        {
            effect.gameObject.SetActive(true);
            effect.Play();
        }
    }

    private void OnDrawGizmosSelected()
    {
        // õ��
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(transform.position.x - 25f, _ceilingHeight, transform.position.z), new Vector3(transform.position.x + 25f, _ceilingHeight, transform.position.z));

        // ������ ������ ����
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(_bodyCollider.bounds.max.x + _distanceRange.Start, _ceilingHeight, transform.position.z), new Vector3(_bodyCollider.bounds.max.x + _distanceRange.Start, _ceilingHeight - 3f, transform.position.z));
        Gizmos.DrawLine(new Vector3(_bodyCollider.bounds.max.x + _distanceRange.End, _ceilingHeight, transform.position.z), new Vector3(_bodyCollider.bounds.max.x + _distanceRange.End, _ceilingHeight - 3f, transform.position.z));

        // ���� ������ ����
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(_bodyCollider.bounds.min.x - _distanceRange.Start, _ceilingHeight, transform.position.z), new Vector3(_bodyCollider.bounds.min.x - _distanceRange.Start, _ceilingHeight - 3f, transform.position.z));
        Gizmos.DrawLine(new Vector3(_bodyCollider.bounds.min.x - _distanceRange.End, _ceilingHeight, transform.position.z), new Vector3(_bodyCollider.bounds.min.x - _distanceRange.End, _ceilingHeight - 3f, transform.position.z));
    }
}