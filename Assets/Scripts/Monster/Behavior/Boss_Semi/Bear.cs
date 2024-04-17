using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class Bear : BossBehavior, ILightCaptureListener
{
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

    #region Variable

    [Header("Bear")]
    [Space]

    [Tooltip("1 : Slash Right\r\n2 : Slash Left\r\n3 : BodySlam\r\n4 : Stomp")]
    [SerializeField] private Range _attackTypeRange;
    [SerializeField] private BearAttackType _currentAttack;
    [SerializeField] private BearAttackType _nextAttack;

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
    [SerializeField] private float _minDistanceEach;
    [SerializeField] private List<float> _usedPosX;

    [Space]

    [SerializeField] private Range _normalStalactiteRange;
    [SerializeField] private Range _rageStalactiteRange;

    [Header("Earthquake")]
    [Space]

    [SerializeField] private Transform _waveSpawnPoint;
    [SerializeField] private Bear_GroundWave _waveSkillPrefab;
    [SerializeField] private ShakePreset _earthquakeCameraShake;

    [Header("Die End VFX")]
    [Space]

    [SerializeField] private BossClearColorChangePlayer bossClearColorChangeEffect;         // ���� ������ ���ƿ��� ȿ��
    [SerializeField] private ParticleSystem[] DisintegrateEffects;                          // ���� ȿ�� ��ƼŬ

    [Header("Cutscene")]
    [Space]

    [SerializeField] private CutscenePlayerList _cutscenePlayerList;

    [Space]

    [SerializeField]
    private bool _isLightingGuide;
    [SerializeField] private int _earthquakeCount;
    public int EarthquakeCount
    {
        get => _earthquakeCount;
        private set => _earthquakeCount = value;
    }

    [Space]

    [SerializeField]
    private bool _is9thAttackSuccess;
    [SerializeField] private int _totalHitCount;
    public int TotalHitCount
    {
        get => _totalHitCount;
        private set
        {
            _totalHitCount = value;

            if (_totalHitCount >= 9 && !_is9thAttackSuccess)
            {
                _is9thAttackSuccess = true;

                // 9��° ���� ���� �ƾ� ����
                StartCoroutine(PlayCutSceneCoroutine("9th Attack Success"));
            }
        }
    }

    [Space]

    [SerializeField] private float _distanceFromTarget;
    [SerializeField] InputSetterScriptableObject _moveRightInputSetter;
    [SerializeField] InputSetterScriptableObject _moveLeftInputSetter;
    [SerializeField] InputSetterScriptableObject _stayStillInputSetter;

    #endregion

    #region Function

    protected override void Start()
    {
        base.Start();

        monsterData.MaxHp = finalTargetHurtCount * MonsterDefine.BossHealthUnit;
        CurHp = monsterData.MaxHp;
        IsGodMode = true;

        RandomTargetAttackCount();
        RandomTargetHitCount();
        SetToRandomAttack();
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
    public override IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        if (IsGodMode || IsDead)
            return IAttackListener.AttackResult.Fail;

        // Hit Process
        HitProcess(attackInfo, false, false);

        // ü�� ����
        TotalHitCount++;
        currentHitCount++;
        CurHp -= MonsterDefine.BossHealthUnit;

        CheckHurtState();

        return IAttackListener.AttackResult.Success;
    }
    public override void Die(bool isHitBoxDisable = true, bool isDeathProcess = true)
    {
        // ����� ��� ����Ʈ�� ������� �ʴ´�
        base.Die(true, false);

        StartCoroutine(SlowMotionCoroutine(5f));
    }

    public void OnLightEnter(LightCapturer capturer, LightSource lightSource)
    {
        if (IsGroggy)
            return;

        // �׷α� ���·� ����
        SetAnimatorTrigger("Groggy");

        if (!_isLightingGuide)
            _isLightingGuide = true;
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
            currentAttackCount = 0;
            EarthquakeCount++;
        }
        else
            currentAttackCount++;
    }
    public override void AttackPostProcess()
    {
        if (currentAttackCount >= targetAttackCount)
        {
            SetToEarthQuake();
            RandomTargetAttackCount();
        }
        else
            SetToRandomAttack();

        if (EarthquakeCount >= 3 && !_isLightingGuide)
        {
            _isLightingGuide = true;
            StartCoroutine(PlayCutSceneCoroutine("Lighting Guide"));
        }
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

        currentHitCount = 0;

        if (IsRage)
            _stalactiteCount = Random.Range((int)_rageStalactiteRange.Start, (int)_rageStalactiteRange.End + 1);
    }

    // basic
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
    public void CheckHurtState()
    {
        if (IsDead) return;

        // �׷α� ���� �����Ǹ� �ǰ�
        if (currentHitCount >= targetHitCount)
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
        // set stalactite count
        _stalactiteCount = Random.Range((int)_normalStalactiteRange.Start, (int)_normalStalactiteRange.End + 1);

        // init stored stalactite pos
        _usedPosX = new List<float>();

        // stomp effect
        Instantiate(_stompEffectPrefab, _stomp.transform.position, Quaternion.identity);
        // stomp sound
        PlaySound("Stomp");

        // create stalactite
        for (int i = 0; i < _stalactiteCount; ++i)
        {
            StartCoroutine(CreateStalactite(RecentDir));
        }
    }
    public IEnumerator CreateStalactite(int dir)
    {
        yield return new WaitForSeconds(_createTimeRange.Random());

        // reallocation count limit
        int posReallocationCount = 0;

        float newPosXInRange;
        do
        {
            // random pos range
            newPosXInRange = (dir > 0)
                ? Random.Range(MainBodyCollider2D.bounds.max.x + _distanceRange.Start,
                    MainBodyCollider2D.bounds.max.x + _distanceRange.End)
                : Random.Range(MainBodyCollider2D.bounds.min.x - _distanceRange.End,
                    MainBodyCollider2D.bounds.min.x - _distanceRange.Start);

            posReallocationCount++;

        } while (_usedPosX.Any(usedPosX => Mathf.Abs(usedPosX - newPosXInRange) <= _minDistanceEach) && posReallocationCount <= 10);
        // List<>: C#���� �����ϴ� '���׸� �÷��� (<Type> ���п�, �ڽ� / ��ڽ��� ���� ����)' ���� �� �ϳ���, ���� �迭�� ����
        // Any(): LINQ(Language Integrated Query) Ȯ�� �޼���
        // �÷��� ���� ��� �� �ϳ��� �־��� ������ �����ϴ��� Ȯ���ϴ� �޼���
        // ���ɻ��� ������ posReallocationCount <= 10�� �ִ� ���Ҵ� Ƚ���� ����

        // store posX
        _usedPosX.Add(newPosXInRange);

        // confirm position
        Vector2 position = new Vector2(newPosXInRange, _ceilingHeight);

        // create stalactite
        var stalactite = Instantiate(_stalactitePrefab, position, Quaternion.identity);
        stalactite.transform.localScale *= _createSizeRange.Random();
        stalactite.SetActor(this);
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
    public IEnumerator SlowMotionCoroutine(float duration)
    {
        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
    public void StartAfterDeath()
    {
        // ���������� �ڷ�ƾ�� ����
        StartCoroutine(PlayerMoveCoroutine());
        StartCoroutine(AfterDeathCoroutine());
    }
    public IEnumerator PlayerMoveCoroutine()
    {
        yield return new WaitForSeconds(1f);

        var player = SceneContext.Current.Player;
        var playerPosX = player.transform.position.x;
        var bearToPlayerDir = System.Math.Sign(playerPosX - transform.position.x);
        var targetPosX = transform.position.x + (bearToPlayerDir) * _distanceFromTarget;

        var playerMoveDir1 = System.Math.Sign(targetPosX - playerPosX);
        yield return StartCoroutine(MoveCoroutine(playerMoveDir1, targetPosX));

        // ���� �÷��̾ �ڵ��� �ִٸ� ������ �����ش�
        if (bearToPlayerDir == player.RecentDir)
        {
            var playerMoveDir2 = (-1) * playerMoveDir1;
            yield return StartCoroutine(MoveCoroutine(playerMoveDir2, targetPosX + playerMoveDir2 * 0.5f));
        }

        InputManager.Instance.ChangeInputSetter(_stayStillInputSetter);
    }
    public IEnumerator MoveCoroutine(int moveDir, float targetPosX)
    {
        InputManager.Instance.ChangeInputSetter(moveDir > 0 ? _moveRightInputSetter : _moveLeftInputSetter);
        yield return new WaitUntil(() => System.Math.Abs(targetPosX - SceneContext.Current.Player.transform.position.x) < 0.1f);
    }
    public IEnumerator AfterDeathCoroutine()
    {
        yield return StartCoroutine(ChangeImageCoroutine());
        yield return StartCoroutine(ChangeBackgroundCoroutine());

        // ��� �ƾ� ����
        _cutscenePlayerList.PlayCutscene("Bear Die");
    }
    public IEnumerator ChangeImageCoroutine()
    {
        // ��� �̹����� �����ϱ� ���� ������ ȿ��
        foreach (var effect in DisintegrateEffects)
            effect.gameObject.SetActive(true);  // play on awake effect

        // ��ƼŬ�� ������� ���ö����� ���
        var endParticleTime = DisintegrateEffects[0].main.duration;
        yield return new WaitForSeconds(endParticleTime / 2f);

        // �˴ٿ� �̹����� ����
        SetAnimatorTrigger("DieEnd");

        yield return new WaitForSeconds(5f);
    }
    public IEnumerator ChangeBackgroundCoroutine()
    {
        // ���� ������ ���ƿ��� ȿ�� ����
        bossClearColorChangeEffect.PlayEffect();

        // ������� ����
        SoundManager.Instance.StopBGM();

        yield return new WaitUntil(() => bossClearColorChangeEffect.isEndEffect);
    }
    public void DisintegrateEffect()
    {
        StartCoroutine(DisintegrateEffectCoroutine());
    }
    public IEnumerator DisintegrateEffectCoroutine()
    {
        var effect = GetComponent<DisintegrateEffect>();
        effect.Play();
        yield return new WaitUntil(() => effect.IsEffectDone);
        Destroy(transform.root ? transform.root.gameObject : gameObject);
    }
    public IEnumerator PlayCutSceneCoroutine(string name)
    {
        // Debug.Log("9��° ���� ����, �ƾ� ������ ����մϴ�");

        yield return new WaitUntil(CurrentStateIs<Monster_IdleState>);

        // Debug.Log("Monster_IdleState�̹Ƿ� �ƾ��� �����մϴ�");

        _cutscenePlayerList.PlayCutscene(name);
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