using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using UnityEngine;

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

    [SerializeField] private BearAttackType _currentAttack;
    [SerializeField] private BearAttackType _nextAttack;

    [Space]

    [Tooltip("1 : Slash Right\r\n2 : Slash Left\r\n3 : BodySlam\r\n4 : Stomp")]
    [SerializeField] private Range _attackTypeRange;
    [Tooltip("Count of attacks for EarthQuake")]
    [SerializeField] private RangeInt _attackCountRange;

    [Space]

    [SerializeField] private int _targetAttackCount;
    [SerializeField] private int _currentAttackCount;

    [Space]

    [Tooltip("Count of hits for Groggy state")]
    [SerializeField] private int _targetHitCount;
    [SerializeField] private int _currentHitCount;

    [Header("Skill Settings")]
    [Space]

    [Header("Slash")]
    [Space]

    [SerializeField] private Bear_Slash _slash01;
    [SerializeField] private Bear_Slash _slash02;
    private Vector2 _playerPos;

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

    [SerializeField] private GameObject LightingStone;                              // �̸��� ���� ����
    [SerializeField] private BossClearColorChangePlayer bossClearColorChangeEffect; // ���� ������ ���ƿ��� ȿ��
    [SerializeField] private ParticleSystem[] DisintegrateEffects;                  // ���� ȿ�� ��ƼŬ

    [Header("Cutscene")]
    [Space]

    [SerializeField] private CutscenePlayer lightingGuide;
    [SerializeField] private CutscenePlayer attackSuccess;
    [SerializeField] private CutscenePlayer endCutscene;

    public static bool isCutScene_LightingGuide;
    public static bool isCutScene_AttackSuccess;
    public static bool isCutScene_End;

    [Header("ETC")]
    [Space]

    public int earthQuakeCount = 0;
    public bool isNeverGroogy = true;
    public bool isNeverRage = true;

    #endregion

    #region Function

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

        /*
        if (!isCutScene_LightingGuide && lightingGuide)
        {
            if (earthQuakeCount >= 3 && isNeverGroogy)
            {
                StartCoroutine(LightingGuideCoroutine());
            }
        }

        if (!isCutScene_AttackSuccess && attackSuccess)
        {
            if (!isNeverRage)
            {
                StartCoroutine(AttackSuccessCoroutine());
            }
        }
        */
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

        // ��� ���� �ǰ� ó��
        CurHp -= MonsterDefine.BossHealthUnit;
        IncreaseHurtCount();

        // Die
        if (CurHp <= 0)
        {
            CurHp = 0;

            Die(true, false);

            StartCoroutine(SlowMotionCoroutine(5f));

            return IAttackListener.AttackResult.Success;
        }

        // Rage
        if (!IsRage && (CurHp <= MaxHp * 0.5f))
        {
            SetAnimatorTrigger("Hurt");
            isNeverRage = false;
            return IAttackListener.AttackResult.Success;
        }

        // Hurt
        if (_currentHitCount >= _targetHitCount)
            SetAnimatorTrigger("Hurt");

        return IAttackListener.AttackResult.Success;
    }
    public override void Die(bool isHitBoxDisable = true, bool isDeathEffect = true)
    {
        base.Die(true, false);
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
        {
            InitializeAttackCount();
            earthQuakeCount++;
        }
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

        if (isNeverGroogy)
            isNeverGroogy = false;

        // ������ MonsterBodyHit�� ���� (�÷��̾ Ÿ���� �� ����)
        SetHitBoxAttackable(false);
    }
    public override void GroggyPostProcess()
    {
        // �׷α� ���� ���� (���� �������� ������ ����)
        IsGroggy = false;

        // ������ MonsterBodyHit�� �Ҵ� (�÷��̾ Ÿ���� �� �ִ�)
        SetHitBoxAttackable(true);

        InitializeHurtCount();

        if (IsRage)
            SetStalactiteToRageCount();
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
        _currentHitCount++;
    }
    public void InitializeHurtCount()
    {
        _currentHitCount = 0;
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
    }
    public void Slash01_AnimEvent()
    {
        var slash = Instantiate(_slash01, _playerPos, Quaternion.identity);
        if (RecentDir < 1) slash.transform.localScale = new Vector3(-Mathf.Abs(slash.transform.localScale.x), slash.transform.localScale.y, slash.transform.localScale.z);
        slash.SetActor(this);

        PlaySound("Slash");

        // �÷��̾� ��ġ �ʱ�ȭ
        _playerPos = Vector2.zero;
    }
    public void Slash02_AnimEvent()
    {
        var slash = Instantiate(_slash02, _playerPos, Quaternion.identity);
        if (RecentDir < 1) slash.transform.localScale = new Vector3(-Mathf.Abs(slash.transform.localScale.x), slash.transform.localScale.y, slash.transform.localScale.z);
        slash.SetActor(this);

        PlaySound("Slash");

        // �÷��̾� ��ġ �ʱ�ȭ
        _playerPos = Vector2.zero;
    }

    // bodySlam
    public void BodySlamPre_AnimEvent()
    {
        var playerPos = SceneContext.Current.Player.transform.position;
        var dir = System.Math.Sign(playerPos.x - transform.position.x);

        // ���� ��ȯ
        SetRecentDir(dir);
    }
    public void BodySlam01_AnimEvent()
    {
        RigidBody2D.AddForce(60f * Vector2.right * RecentDir, ForceMode2D.Impulse);
    }
    public void BodySlam02_AnimEvent()
    {
        RigidBody2D.velocity = Vector2.zero;
    }

    // stomp
    public void Stomp01_AnimEvent()
    {
        Instantiate(_stompEffectPrefab, _stomp.transform.position, Quaternion.identity);
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

        // random size
        stalactite.transform.localScale *= _createSizeRange.Random();
    }

    // earthQuake
    public void Earthquake01_AnimEvent()
    {
        // ������ ����
        GenerateGroundWave();
        PlaySound("Earthquake");
        SceneEffectManager.Current.Camera.StartShake(_earthquakeCameraShake);
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

        if (!isCutScene_End && endCutscene)
        {
            isCutScene_End = true;
            endCutscene.Play();
        }
    }

    // cutscene
    public IEnumerator LightingGuideCoroutine()
    {
        isCutScene_LightingGuide = true;

        yield return new WaitUntil(() => CurrentStateIs<Monster_IdleState>());

        lightingGuide.Play();
    }
    public IEnumerator AttackSuccessCoroutine()
    {
        isCutScene_AttackSuccess = true;

        if (AttackEvaluator.IsUsable)
            AttackEvaluator.IsUsable = false;

        yield return new WaitUntil(() => CurrentStateIs<Monster_IdleState>());

        attackSuccess.Play();
    }
    IEnumerator SlowMotionCoroutine(float duration)
    {
        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
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