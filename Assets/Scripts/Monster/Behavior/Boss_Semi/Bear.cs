using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class Bear : BossBehavior, ILightCaptureListener
{
    public enum AttackType
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
    [SerializeField] private AttackType _currentAttack;
    [SerializeField] private AttackType _nextAttack;

    [Header("Skill: Slash")]
    [Space]

    [SerializeField] private Bear_Slash _slash01;
    [SerializeField] private Bear_Slash _slash02;
    private Vector2 _playerPos;

    [Header("Skill: BodySlam")]
    [Space]

    [SerializeField] private float _bodySlamPower;

    [Header("Skill: Stomp")]
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

    private List<float> _usedPosX;

    [Space]

    [SerializeField] private Range _normalStalactiteRange;
    [SerializeField] private Range _rageStalactiteRange;

    [Header("Skill: Earthquake")]
    [Space]

    [SerializeField] private Transform _waveSpawnPoint;
    [SerializeField] private Bear_GroundWave _waveSkillPrefab;
    [SerializeField] private ShakePreset _earthquakeCameraShake;

    [Header("Cutscene")]
    [Space]

    [SerializeField] private CutscenePlayerList _cutscenePlayerList;

    [Space]

    [SerializeField] private bool _isAbleLightGuide = true;
    [SerializeField] private bool _isAbleChangeRage = true;
    [SerializeField] private int _totalEarthquakeCount;
    public int TotalEarthquakeCount
    {
        get => _totalEarthquakeCount;
        private set => _totalEarthquakeCount = value;
    }
    [SerializeField] private int _totalHitCount;
    public int TotalHitCount
    {
        get => _totalHitCount;
        private set
        {
            _totalHitCount = value;

            if (_totalHitCount == finalTargetHurtCount / 2 && _isAbleChangeRage)
            {
                Debug.Log("Change RageState 컷씬 호출");

                _isAbleChangeRage = false;
                StartCoroutine(PlayCutSceneInRunning("Change RageState"));
            }
        }
    }

    [Header("After Dead")]
    [Space]

    [SerializeField] private BossClearColorChangePlayer bossClearColorChangeEffect;             // 색이 서서히 돌아오는 효과
    [SerializeField] private ParticleSystem[] _disintegrateEffects;                             // 잿가루 효과 파티클
    [SerializeField] private GameObject _bossKnockDownGameObject;                               // 넉다음 이미지 오브젝트
    [Space]

    [SerializeField] private float _distanceFromBear;                                           // 흑곰으로부터 떨어져야하는 거리

    #endregion

    #region Function

    protected override void Start()
    {
        base.Start();

        monsterData.MaxHp = finalTargetHurtCount * MonsterDefine.BossHealthUnit;
        CurHp = monsterData.MaxHp;
        IsGodMode = true;

        SetToRandomAttack();
    }
    public void FixedUpdate()
    {
        if (IsDead)
            return;

        if (CurrentStateIs<GroundMoveState>())
        {
            if (GroundMovementModule)
                GroundMovementModule.WalkGround();
        }
        else
        {
            if (GroundMovementModule)
                GroundMovementModule.AffectGravity();
        }
    }

    public override IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        if (IsGodMode || IsDead)
            return IAttackListener.AttackResult.Fail;

        // Hit Process
        HitProcess(attackInfo, false, false);

        // 체력 감소
        TotalHitCount++;
        currentHitCount++;
        CurHp -= MonsterDefine.BossHealthUnit;

        CheckHurtState();

        return IAttackListener.AttackResult.Success;
    }
    public override void Die(bool isHitBoxDisable, bool isDeathProcess)
    {
        // 보스는 사망 이펙트를 재생하지 않는다
        base.Die(true, false);

        StartCoroutine(SlowMotionCoroutine(5f));
    }

    public void OnLightEnter(LightCapturer capturer, LightSource lightSource)
    {
        if (IsDead || IsGroggy || !IsCapturable)
            return;

        // 그로기 상태로 진입
        SetAnimatorTrigger("Groggy");

        if (_isAbleLightGuide)
            _isAbleLightGuide = false;
    }

    // boss base
    public override void AttackPreProcess()
    {
        // 현재 공격 상태 변경
        _currentAttack = _nextAttack;

        if (_currentAttack is AttackType.Null || _nextAttack is AttackType.Null)
        {
            Debug.LogError("AttackType is Null");
            return;
        }

        if (_currentAttack is AttackType.EarthQuake)
        {
            // Debug.Log("Earth Quake의 Attack Pre Process");

            currentAttackCount = 0;
            TotalEarthquakeCount++;

            IsCapturable = true;
        }
        else
            currentAttackCount++;
    }
    public override void AttackPostProcess()
    {
        if (_currentAttack is AttackType.EarthQuake)
        {
            // Debug.Log("Earth Quake의 Attack Post Process");

            if (_totalEarthquakeCount == 3 && _isAbleLightGuide)
            {
                Debug.Log("Lighting Guide 컷씬 호출");

                _isAbleLightGuide = false;
                StartCoroutine(PlayCutSceneInRunning("Lighting Guide"));
            }

            IsCapturable = false;
        }

        if (currentAttackCount >= targetAttackCount)
        {
            SetToEarthQuake();
            RandomTargetAttackCount();
        }
        else
            SetToRandomAttack();
    }
    public override void GroggyPreProcess()
    {
        // 그로기 상태 진입 (더이상 손전등의 영향을 받지 않음)
        IsGroggy = true;

        // 몬스터의 MonsterBodyHit를 끈다 (플레이어를 타격할 수 없다)
        SetHitBoxAttackable(false);
    }
    public override void GroggyPostProcess()
    {
        // 그로기 상태 종료 (이제 손전등의 영향을 받음)
        IsGroggy = false;

        // 몬스터의 Body HitBox를 켠다 (플레이어를 타격할 수 있다)
        SetHitBoxAttackable(true);

        currentHitCount = 0;

        if (IsRage)
            _stalactiteCount = Random.Range((int)_rageStalactiteRange.Start, (int)_rageStalactiteRange.End + 1);
    }

    // basic
    private void SetToRandomAttack()
    {
        int nextAttackNumber = (int)_attackTypeRange.Random();
        _nextAttack = (AttackType)nextAttackNumber;
        Animator.SetInteger("NextAttackNumber", nextAttackNumber);
    }
    private void SetToEarthQuake()
    {
        _nextAttack = AttackType.EarthQuake;
        Animator.SetInteger("NextAttackNumber", (int)_nextAttack);
    }
    public void CheckHurtState()
    {
        if (IsDead) return;

        // 그로기 상태 해제되며 피격
        if (currentHitCount >= targetHitCount)
        {
            SetAnimatorTrigger("Hurt");
        }
    }

    // slash
    public void SlashPre_AnimEvent()
    {
        // 플레이어의 위치
        var playerPos = SceneContext.Current.Player.transform.position;

        // 플레이어를 바라보는 방향
        var dirBearToPlayer = System.Math.Sign(playerPos.x - transform.position.x);

        // 바라보는 방향에 플레이어가 있는지
        var isPlayerInLookDirection = dirBearToPlayer == RecentDir;

        // 바라보는 방향에 플레이어가 있다면
        if (isPlayerInLookDirection)
        {
            // 플레이어의 위치를 기억
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
        // 플레이어의 위치
        var playerPos = SceneContext.Current.Player.transform.position;
        var dirBearToPlayer = System.Math.Sign(playerPos.x - transform.position.x);

        // 방향 전환
        SetRecentDir(dirBearToPlayer);
    }
    public void BodySlam01_AnimEvent()
    {
        RigidBody2D.AddForce(_bodySlamPower * Vector2.right * RecentDir, ForceMode2D.Impulse);
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
        // List<>: C#에서 제공하는 '제네릭 컬렉션 (<Type> 덕분에, 박싱 / 언박싱을 하지 않음)' 유형 중 하나로, 동적 배열을 구현
        // Any(): LINQ(Language Integrated Query) 확장 메서드
        // 컬렉션 내의 요소 중 하나라도 주어진 조건을 만족하는지 확인하는 메서드
        // 성능상의 이유로 posReallocationCount <= 10로 최대 재할당 횟수를 제한

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
        SceneEffectManager.Instance.Camera.StartShake(_earthquakeCameraShake);
        // earthQuake sound
        PlaySound("Earthquake");

        // create wave
        GenerateGroundWave();
    }
    public void GenerateGroundWave()
    {
        // 2개의 지면파를 발생시킨다 (좌 / 우)
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
        // 병렬적으로 코루틴을 실행
        StartCoroutine(PlayerMoveCoroutine());
        StartCoroutine(AfterDeathCoroutine());
    }
    public IEnumerator PlayerMoveCoroutine()
    {
        yield return new WaitForSeconds(1f);

        var player = SceneContext.Current.Player;
        var playerPosX = player.transform.position.x;
        Debug.DrawRay(player.transform.position, Vector3.down * 5f, Color.cyan, 10f);

        var bearToPlayerDir = System.Math.Sign(playerPosX - transform.position.x);
        Debug.DrawRay(transform.position + Vector3.up, Vector3.right * bearToPlayerDir * _distanceFromBear, Color.cyan, 10f);

        var playerMoveTargetPosX = transform.position.x + (bearToPlayerDir) * _distanceFromBear;
        Debug.DrawRay(new Vector3(playerMoveTargetPosX, transform.position.y, transform.position.z), Vector3.down * 5f, Color.cyan, 10f);

        var playerMoveDir = System.Math.Sign(playerMoveTargetPosX - playerPosX);
        Debug.DrawRay(player.transform.position, Vector3.right * playerMoveDir * 5f, Color.cyan, 10f);

        yield return StartCoroutine(MoveCoroutine(playerMoveDir, playerMoveTargetPosX));

        // 만약 플레이어가 뒤돌고 있다면 방향을 돌려준다
        if (bearToPlayerDir == player.RecentDir)
        {
            var dirForLookToBear = (-1) * playerMoveDir;
            yield return StartCoroutine(MoveCoroutine(dirForLookToBear, playerMoveTargetPosX + dirForLookToBear * 0.1f));
        }

        InputManager.Instance.ChangeToStayStillSetter();
    }
    public IEnumerator MoveCoroutine(int moveDir, float targetPosX)
    {
        var isRight = moveDir > 0;

        if (isRight)
            InputManager.Instance.ChangeToMoveRightSetter();
        else
            InputManager.Instance.ChangeToMoveLeftSetter();

        yield return new WaitUntil(() => System.Math.Abs(targetPosX - SceneContext.Current.Player.transform.position.x) < 0.1f);
    }
    public IEnumerator AfterDeathCoroutine()
    {
        yield return StartCoroutine(ChangeImageCoroutine());
        yield return StartCoroutine(ChangeBackgroundCoroutine());

        // 최종 컷씬 재생
        _cutscenePlayerList.PlayCutscene("Final CutScene");
    }
    public IEnumerator ChangeImageCoroutine()
    {
        // 사망 이미지로 변경하기 위한 가림막 효과
        foreach (var effect in _disintegrateEffects)
            effect.gameObject.SetActive(true);  // play on awake effect

        // 파티클이 어느정도 나올때까지 대기
        var endParticleTime = _disintegrateEffects[0].main.duration;
        yield return new WaitForSeconds(endParticleTime / 2f);

        // 넉다운 이미지로 변경
        SetAnimatorTrigger("DieEnd");

        yield return new WaitForSeconds(5f);
    }
    public IEnumerator ChangeBackgroundCoroutine()
    {
        // 색이 서서히 돌아오는 효과 시작
        bossClearColorChangeEffect.PlayEffect();

        // 배경음악 정지
        SoundManager.Instance.StopBGM();

        yield return new WaitUntil(() => bossClearColorChangeEffect.isEndEffect);

        var knockDownSprites = _bossKnockDownGameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sprite in knockDownSprites)
        {
            var color = sprite.color;
            color.a = 0f;
            sprite.color = color;
        }
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
        Destroy(transform.parent ? transform.parent.gameObject : gameObject);
    }
    private IEnumerator PlayCutSceneInRunning(string cutsceneName)
    {
        yield return new WaitUntil(CurrentStateIs<Monster_IdleState>);

        _cutscenePlayerList.PlayCutscene(cutsceneName);
    }
    private IEnumerator PlayCutSceneInRunning(CutscenePlayer cutscenePlayer)
    {
        yield return new WaitUntil(CurrentStateIs<Monster_IdleState>);

        _cutscenePlayerList.PlayCutscene(cutscenePlayer);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        // 종유석 생성 범위
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(transform.position.x - 25f, _ceilingHeight, transform.position.z), new Vector3(transform.position.x + 25f, _ceilingHeight, transform.position.z));

        if (! MainBodyCollider2D) return;

        // 오른쪽 종유석 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(MainBodyCollider2D.bounds.max.x + _distanceRange.Start, _ceilingHeight, transform.position.z), new Vector3(MainBodyCollider2D.bounds.max.x + _distanceRange.Start, _ceilingHeight - 3f, transform.position.z));
        Gizmos.DrawLine(new Vector3(MainBodyCollider2D.bounds.max.x + _distanceRange.End, _ceilingHeight, transform.position.z), new Vector3(MainBodyCollider2D.bounds.max.x + _distanceRange.End, _ceilingHeight - 3f, transform.position.z));

        // 왼쪽 종유석 범위
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(MainBodyCollider2D.bounds.min.x - _distanceRange.Start, _ceilingHeight, transform.position.z), new Vector3(MainBodyCollider2D.bounds.min.x - _distanceRange.Start, _ceilingHeight - 3f, transform.position.z));
        Gizmos.DrawLine(new Vector3(MainBodyCollider2D.bounds.min.x - _distanceRange.End, _ceilingHeight, transform.position.z), new Vector3(MainBodyCollider2D.bounds.min.x - _distanceRange.End, _ceilingHeight - 3f, transform.position.z));
    }
}