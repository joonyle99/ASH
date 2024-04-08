using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using UnityEngine;

public enum BearAttackType
{
    Null = 0,

    // Normal Attack
    SlashRight,
    SlashLeft,
    BodySlam,
    Stomp,

    // Special Attack
    EarthQuake = 10
}

public class Bear : SemiBossBehavior, ILightCaptureListener
{
    #region Variable

    [Header("Bear")]
    [Space]

    [Header("Condition")]
    [Space]

    [Tooltip("hurt count x health unit = MaxHp")]
    [SerializeField] private int _finalTargetHurtCount;

    [Space]

    [Header("Attack Type")]
    [Tooltip("1 : Slash Right\r\n2 : Slash Left\r\n3 : BodySlam\r\n4 : Stomp")]
    [SerializeField] private Range _attackTypeRange;
    [SerializeField] private BearAttackType _currentAttack;
    [SerializeField] private BearAttackType _nextAttack;

    [Space]

    [Header("Attack Count")]
    [Tooltip("Count of attacks for EarthQuake")]
    [SerializeField] private RangeInt _attackCountRange;
    [SerializeField] private int _targetAttackCount;
    [SerializeField] private int _currentAttackCount;

    [Space]

    [Header("Hit Count")]
    [Tooltip("Count of hits for Groggy state")]
    [SerializeField] private RangeInt _hitCountRange;
    [SerializeField] private int _targetHitCount;
    [SerializeField] private int _currentHitCount;

    [Header("Skill Settings")]
    [Space]

    [Header("Slash")]
    [Space]

    [SerializeField] private Bear_Slash _slash01;
    [SerializeField] private Bear_Slash _slash02;
    private Vector2 _playerPos;

    [Header("BodySlam")]
    [Space]

    [SerializeField] private float _bodySlamLength;

    [Header("Stomp")]
    [Space]

    [SerializeField] private Bear_Stomp _stomp;
    [SerializeField] private GameObject _stompEffectPrefab;
    [SerializeField] private Bear_Stalactite _stalactitePrefab;

    [Space]

    [SerializeField] private int _stalactiteCount;
    [SerializeField] private float _ceilingHeight;
    [SerializeField] private Range _createTimeRange;
    [SerializeField] private Range _createSizeRange;
    [SerializeField] private Range _distanceRange;

    [Space]

    [SerializeField] private Range _normalStalactiteRange;
    [SerializeField] private Range _rageStalactiteRange;

    [Header("Earthquake")]
    [Space]

    [SerializeField] private Transform _waveSpawnPoint;
    [SerializeField] private Bear_GroundWave _waveSkillPrefab;
    [SerializeField] private ShakePreset _earthquakeCameraShake;

    [Space]

    [Header("VFX")]
    [Space]

    [SerializeField] private BossClearColorChangePlayer bossClearColorChangeEffect;         // ���� ������ ���ƿ��� ȿ��
    [SerializeField] private ParticleSystem[] DisintegrateEffects;                          // ���� ȿ�� ��ƼŬ

    #endregion

    #region Function

    protected override void Start()
    {
        base.Start();

        // init
        IsGodMode = true;
        RandomTargetAttackCount();
        RandomTargetHitCount();
        SetToRandomAttack();
        _stalactiteCount = Random.Range((int)_normalStalactiteRange.Start, (int)_normalStalactiteRange.End + 1);
    }
    public void FixedUpdate()
    {
        if (IsDead)
            return;

        if (CurrentStateIs<GroundMoveState>())
        {
            if (GroundMovementModule)
                GroundMovementModule.GroundWalking();
        }
    }
    public override void SetUp()
    {
        base.SetUp();

        MaxHp = _finalTargetHurtCount * MonsterDefine.BossHealthUnit;
        CurHp = MaxHp;
    }
    public override IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        if (IsGodMode || IsDead)
            return IAttackListener.AttackResult.Fail;

        // Hit Process
        HitProcess(attackInfo, false, false);

        // ü�� ����
        _currentHitCount++;
        CurHp -= MonsterDefine.BossHealthUnit;

        CheckStatus();

        return IAttackListener.AttackResult.Success;
    }
    public override void Die(bool isHitBoxDisable = true, bool isDeathEffect = true)
    {
        base.Die(true, false);

        StartCoroutine(SlowMotionCoroutine(5f));
    }

    public void OnLightEnter(LightCapturer capturer, LightSource lightSource)
    {
        if (IsGroggy)
            return;

        // �׷α� ���·� ����
        SetAnimatorTrigger("Groggy");
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
            _currentAttackCount = 0;
        else
            _currentAttackCount++;
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
        SetHitBoxAttackable(false);
    }
    public override void GroggyPostProcess()
    {
        // �׷α� ���� ���� (���� �������� ������ ����)
        IsGroggy = false;

        // ������ Body HitBox�� �Ҵ� (�÷��̾ Ÿ���� �� �ִ�)
        SetHitBoxAttackable(true);

        _currentHitCount = 0;

        if (IsRage)
            _stalactiteCount = Random.Range((int)_rageStalactiteRange.Start, (int)_rageStalactiteRange.End + 1);
    }

    // basic
    private void RandomTargetAttackCount()
    {
        if (_attackCountRange.Start > _attackCountRange.End)
            Debug.LogError("minTargetCount > maxTargetCount");
        else if (_attackCountRange.Start < 0)
            Debug.LogError("minTargetCount < 0");
        else if (_attackCountRange.End <= 0)
            Debug.LogError("maxTargetCount <= 0");

        // 4�� ~ 7�� ���� �� ���� ����
        _targetAttackCount = Random.Range(_attackCountRange.Start, _attackCountRange.End);
    }
    private void RandomTargetHitCount()
    {
        if (_hitCountRange.Start > _hitCountRange.End)
            Debug.LogError("minTargetCount > maxTargetCount");
        else if (_hitCountRange.Start < 0)
            Debug.LogError("minTargetCount < 0");
        else if (_hitCountRange.End <= 0)
            Debug.LogError("maxTargetCount <= 0");

        _targetHitCount = Random.Range(_hitCountRange.Start, _hitCountRange.End);
    }
    private void SetToRandomAttack()
    {
        int nextAttackNumber = (int)_attackTypeRange.Random();
        _nextAttack = (BearAttackType)nextAttackNumber;
        Animator.SetInteger("NextAttackNumber", nextAttackNumber);
    }
    private void SetToEarthQuake()
    {
        _nextAttack = BearAttackType.EarthQuake;
        Animator.SetInteger("NextAttackNumber", (int)_nextAttack);
    }
    public void CheckStatus()
    {
        if (IsDead) return;

        // �ݳ� ���·� ����
        if (!IsRage && (CurHp <= MaxHp / 2))
        {
            SetAnimatorTrigger("Shout");
        }
        // �׷α� ���� �����Ǹ� �ǰ�
        else if (_currentHitCount >= _targetHitCount)
        {
            SetAnimatorTrigger("Hurt");
        }
    }

    // slash
    public void SlashPre_AnimEvent()
    {
        // �÷��̾��� ��ġ
        var playerPos = SceneContext.Current.Player.transform.position;

        // �÷��̾ �ٶ󺸴� ����
        var dirBearToPlayer = System.Math.Sign(playerPos.x - transform.position.x);

        // �ٶ󺸴� ���⿡ �÷��̾ �ִ���
        bool isPlayerInLookDirection = dirBearToPlayer == RecentDir;

        // �ٶ󺸴� ���⿡ �÷��̾ �ִٸ�
        if (isPlayerInLookDirection)
        {
            // �÷��̾��� ��ġ�� ���
            _playerPos = playerPos;
        }
        else
        {
            _playerPos = Vector2.zero;
        }
    }
    public void Slash01_AnimEvent()
    {
        // slash effect
        var slash = Instantiate(_slash01, _playerPos, Quaternion.identity);
        if (RecentDir < 1) slash.transform.localScale = new Vector3(-Mathf.Abs(slash.transform.localScale.x), slash.transform.localScale.y, slash.transform.localScale.z);
        slash.SetActor(this);

        // slash sound
        PlaySound("Slash");
    }
    public void Slash02_AnimEvent()
    {
        // slash effect
        var slash = Instantiate(_slash02, _playerPos, Quaternion.identity);
        if (RecentDir < 1) slash.transform.localScale = new Vector3(-Mathf.Abs(slash.transform.localScale.x), slash.transform.localScale.y, slash.transform.localScale.z);
        slash.SetActor(this);

        // slash sound
        PlaySound("Slash");
    }

    // bodySlam
    public void BodySlamPre_AnimEvent()
    {
        // �÷��̾��� ��ġ
        var playerPos = SceneContext.Current.Player.transform.position;
        var dirBearToPlayer = System.Math.Sign(playerPos.x - transform.position.x);

        // ���� ��ȯ
        SetRecentDir(dirBearToPlayer);
    }
    public void BodySlam01_AnimEvent()
    {
        RigidBody2D.AddForce(_bodySlamLength * Vector2.right * RecentDir, ForceMode2D.Impulse);
    }
    public void BodySlam02_AnimEvent()
    {
        RigidBody2D.velocity = Vector2.zero;
    }

    // stomp
    public void Stomp01_AnimEvent()
    {
        // stomp effect
        Instantiate(_stompEffectPrefab, _stomp.transform.position, Quaternion.identity);
        // stomp sound
        PlaySound("Stomp");

        // create stalactite
        for (int i = 0; i < _stalactiteCount; ++i)
            StartCoroutine(CreateStalactite());
    }
    public IEnumerator CreateStalactite()
    {
        yield return new WaitForSeconds(_createTimeRange.Random());

        // random pos range
        float posXInRange = (RecentDir > 0)
            ? Random.Range(MainBodyCollider2D.bounds.max.x + _distanceRange.Start,
                MainBodyCollider2D.bounds.max.x + _distanceRange.End)
            : Random.Range(MainBodyCollider2D.bounds.min.x - _distanceRange.End,
                MainBodyCollider2D.bounds.min.x - _distanceRange.Start);
        Vector2 position = new Vector2(posXInRange, _ceilingHeight);

        // create stalactite
        var stalactite = Instantiate(_stalactitePrefab, position, Quaternion.identity);
        stalactite.SetActor(this);
        stalactite.transform.localScale *= _createSizeRange.Random();
    }

    // earthQuake
    public void Earthquake01_AnimEvent()
    {
        // earthQuake camera shake
        SceneEffectManager.Current.Camera.StartShake(_earthquakeCameraShake);
        // earthQuake sound
        PlaySound("Earthquake");

        // create wave
        GenerateGroundWave();
    }
    public void GenerateGroundWave()
    {
        // 2���� �����ĸ� �߻���Ų�� (�� / ��)
        var wave1 = Instantiate(_waveSkillPrefab, _waveSpawnPoint.position, Quaternion.identity);
        wave1.SetDir(Vector2.left);
        wave1.SetActor(this);
        var wave2 = Instantiate(_waveSkillPrefab, _waveSpawnPoint.position, Quaternion.identity);
        wave2.SetDir(Vector2.right);
        wave2.SetActor(this);
    }

    // effects
    IEnumerator SlowMotionCoroutine(float duration)
    {
        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
    public void StartDisintegrateEffect()
    {
        StartCoroutine(DisintegrateEffectCoroutine());
    }
    public IEnumerator DisintegrateEffectCoroutine()
    {
        // ������� ȿ�� ����
        foreach (var effect in DisintegrateEffects)
        {
            effect.gameObject.SetActive(true);
            effect.Play();
        }

        var endParticleTime = DisintegrateEffects[0].main.duration;
        yield return new WaitForSeconds(endParticleTime / 1.5f);

        // �˴ٿ� �̹����� ����
        SetAnimatorTrigger("DieEnd");

        yield return new WaitForSeconds(5f);

        // ���� ������ ���ƿ��� ȿ�� ����
        bossClearColorChangeEffect.PlayEffect();

        // �뷡 ����
        SoundManager.Instance.StopBGM();

        yield return new WaitUntil(() => bossClearColorChangeEffect.isEndEffect);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        // õ��
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(transform.position.x - 25f, _ceilingHeight, transform.position.z), new Vector3(transform.position.x + 25f, _ceilingHeight, transform.position.z));

        if (MainBodyCollider2D)
        {
            // ������ ������ ����
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(new Vector3(MainBodyCollider2D.bounds.max.x + _distanceRange.Start, _ceilingHeight, transform.position.z), new Vector3(MainBodyCollider2D.bounds.max.x + _distanceRange.Start, _ceilingHeight - 3f, transform.position.z));
            Gizmos.DrawLine(new Vector3(MainBodyCollider2D.bounds.max.x + _distanceRange.End, _ceilingHeight, transform.position.z), new Vector3(MainBodyCollider2D.bounds.max.x + _distanceRange.End, _ceilingHeight - 3f, transform.position.z));

            // ���� ������ ����
            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector3(MainBodyCollider2D.bounds.min.x - _distanceRange.Start, _ceilingHeight, transform.position.z), new Vector3(MainBodyCollider2D.bounds.min.x - _distanceRange.Start, _ceilingHeight - 3f, transform.position.z));
            Gizmos.DrawLine(new Vector3(MainBodyCollider2D.bounds.min.x - _distanceRange.End, _ceilingHeight, transform.position.z), new Vector3(MainBodyCollider2D.bounds.min.x - _distanceRange.End, _ceilingHeight - 3f, transform.position.z));

        }
    }
}