using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class Bear : BossBehaviour, ILightCaptureListener
{
    public enum AttackType
    {
        None = 0,

        // Normal Attack
        SlashRight,
        SlashLeft,
        BodySlam,
        Stomp,

        // Ultimate Attack
        EarthQuake = 10
    }

    #region Variable

    [Header("――――――― Bear Behaviour ―――――――")]
    [Space]

    [Tooltip("1 : Slash Right\n2 : Slash Left\n3 : BodySlam\n4 : Stomp")]
    [SerializeField] private Range _attackTypeRange;
    [SerializeField] private AttackType _currentAttack;
    [SerializeField] private AttackType _nextAttack;

    [Header("____ Slash ____")]
    [Space]

    [SerializeField] private Bear_Slash _slash01;
    [SerializeField] private Bear_Slash _slash02;

    private float _slashAnimDuration;
    private Vector2 _playerPos;

    [Header("____ BodySlam ____")]
    [Space]

    [SerializeField] private float _bodySlamPower;

    private float _bodySlamAnimDuration;

    [Header("____ Stomp ____")]
    [Space]

    [SerializeField] private Bear_Stomp _stomp;
    [SerializeField] private GameObject _stompEffectPrefab;
    [SerializeField] private Bear_Stalactite _stalactitePrefab;

    [Space]

    [SerializeField] private int _stalactiteCount;
    [SerializeField] private float _ceilingHeight;

    [Space]

    [SerializeField] private float _minDistanceEach;

    [Space]

    [SerializeField] private Range _createTimeRange;
    [SerializeField] private Range _createSizeRange;
    [SerializeField] private Range _distanceRange;

    private float _stompAnimDuration;
    private List<float> _usedPosX;
    private int _allocationLimit = 30;

    [Space]

    [SerializeField] private Range _normalStalactiteRange;
    [SerializeField] private Range _rageStalactiteRange;

    [Header("____ Earthquake ____")]
    [Space]

    [SerializeField] private Transform _waveSpawnPoint;
    [SerializeField] private Bear_GroundWave _waveSkillPrefab;
    [SerializeField] private ShakePreset _earthquakeCameraShake;

    [Space]

    [SerializeField] private int _totalEarthquakeCount;
    public int TotalEarthquakeCount
    {
        get => _totalEarthquakeCount;
        private set => _totalEarthquakeCount = value;
    }

    private float _earthquakeAnimDuration;

    [Header("Cutscene")]
    [Space]

    [SerializeField] private bool _isAbleLightGuideCutscene = true;

    [Space]

    [SerializeField] private BossClearColorChangePlayer bossClearColorChangeEffect;             // 색이 서서히 돌아오는 효과
    [SerializeField] private ParticleSystem[] _disintegrateEffects;                             // 잿가루 효과 파티클
    [SerializeField] private GameObject _bossKnockDownGameObject;                               // 넉다음 이미지 오브젝트

    #endregion

    #region Function

    protected override void Awake()
    {
        base.Awake();

        // 덩쿨 스킬 애니메이션 클립의 길이 추출
        foreach (var clip in Animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "ani_bear_slash")
                _slashAnimDuration = clip.length;
            else if (clip.name == "ani_bear_stomp")
                _stompAnimDuration = clip.length;
            else if (clip.name == "ani_bear_bodyslam")
                _bodySlamAnimDuration = clip.length;
            else if (clip.name == "ani_bear_earthquake")
                _earthquakeAnimDuration = clip.length;
        }

        // 공격 판독기의 대기 이벤트 등록
        AttackEvaluator.WaitEvent -= OnAttackWaitEvent;
        AttackEvaluator.WaitEvent += OnAttackWaitEvent;

        rageTargetHurtCount = (finalTargetHurtCount + 1) / 2;
    }
    protected override void Start()
    {
        base.Start();

        SetToRandomAttack();
    }
    private void FixedUpdate()
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
    private void OnDestroy()
    {
        AttackEvaluator.WaitEvent -= OnAttackWaitEvent;
    }

    public void OnLightEnter(LightCapturer capturer, LightSource lightSource)
    {
        if (IsDead || IsGroggy || !IsCapturable)
            return;

        // 그로기 상태로 진입
        SetAnimatorTrigger("Groggy");

        if (_isAbleLightGuideCutscene)
            _isAbleLightGuideCutscene = false;
    }

    // boss base
    public override void AttackPreProcess()
    {
        // 현재 공격 상태 변경
        _currentAttack = _nextAttack;

        if (_currentAttack is AttackType.EarthQuake)
        {
            currentAttackCount = 0;
            TotalEarthquakeCount++;

            if (_totalEarthquakeCount == 3 && _isAbleLightGuideCutscene == true)
            {
                Debug.Log("Lighting Guide 컷씬 호출");

                _isAbleLightGuideCutscene = false;
                StartCoroutine(PlayCutSceneInRunning("Lighting Guide"));
            }

            IsCapturable = true;
        }
        else
            currentAttackCount++;
    }
    public override void AttackPostProcess()
    {
        if (_currentAttack is AttackType.EarthQuake)
        {
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
        // Debug.Log("bear groggy pre process");

        // 그로기 상태 진입 (더이상 손전등의 영향을 받지 않음)
        IsGroggy = true;

        // 몬스터의 MonsterBodyHit를 끈다 (플레이어를 타격할 수 없다)
        SetHitBoxAttackable(false);
    }
    public override void GroggyPostProcess()
    {
        // Debug.Log("bear groggy post process");

        // 그로기 상태 종료 (이제 손전등의 영향을 받음)
        IsGroggy = false;

        // 몬스터의 Body HitBox를 켠다 (플레이어를 타격할 수 있다)
        SetHitBoxAttackable(true);

        currentHitCount = 0;
    }

    // basic
    private void SetToRandomAttack()
    {
        int nextAttackNumber = (int)_attackTypeRange.Random();

        if (System.Enum.IsDefined(typeof(AttackType), nextAttackNumber))
        {
            _nextAttack = (AttackType)nextAttackNumber;
            Animator.SetInteger("NextAttackNumber", nextAttackNumber);
        }
        else
        {
            Debug.LogError("<color=red>Invalid AttackType generated</color>");
            _nextAttack = AttackType.None;
        }
    }
    private void SetToEarthQuake()
    {
        _nextAttack = AttackType.EarthQuake;
        Animator.SetInteger("NextAttackNumber", (int)_nextAttack);
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
        _stalactiteCount = IsRage ?
            Random.Range((int)_rageStalactiteRange.Start, (int)_rageStalactiteRange.End + 1)
            : Random.Range((int)_normalStalactiteRange.Start, (int)_normalStalactiteRange.End + 1);

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
        var player = SceneContext.Current.Player;

        var leftX = player.BodyCollider.bounds.min.x - _distanceRange.End;
        var rightX = player.BodyCollider.bounds.max.x + _distanceRange.End;

        Debug.DrawLine(new Vector3(leftX, _ceilingHeight, player.transform.position.z), new Vector3(leftX, _ceilingHeight - 3f, player.transform.position.z), Color.cyan, 3f);
        Debug.DrawLine(new Vector3(rightX, _ceilingHeight, player.transform.position.z), new Vector3(rightX, _ceilingHeight - 3f, player.transform.position.z), Color.cyan, 3f);

        yield return new WaitForSeconds(_createTimeRange.Random());

        // allocation count limit
        int allocationCount = 0;

        float newPosXInRange;
        do
        {
            /*
            // random pos range
            newPosXInRange = (dir > 0)
                ? Random.Range(player.BodyCollider.bounds.max.x + _distanceRange.Start,
                    player.BodyCollider.bounds.max.x + _distanceRange.End)
                : Random.Range(player.BodyCollider.bounds.min.x - _distanceRange.End,
                    player.BodyCollider.bounds.min.x - _distanceRange.Start);
            */

            newPosXInRange = Random.Range(leftX, rightX);

            allocationCount++;

        } while (_usedPosX.Any(usedPosX => Mathf.Abs(usedPosX - newPosXInRange) <= _minDistanceEach) && allocationCount <= _allocationLimit);
        // List<>: C#에서 제공하는 '제네릭 컬렉션 (<Type> 덕분에, 박싱 / 언박싱을 하지 않음)' 유형 중 하나로, 동적 배열을 구현
        // Any(): LINQ(Language Integrated Query) 확장 메서드
        // 컬렉션 내의 요소 중 하나라도 주어진 조건을 만족하는지 확인하는 메서드
        // 성능상의 이유로 posReallocationCount <= 10로 최대 재할당 횟수를 제한

        // store posX
        _usedPosX.Add(newPosXInRange);

        // confirm spawn position
        Vector2 spawnPosition = new Vector2(newPosXInRange, _ceilingHeight);

        // create stalactite
        var stalactite = Instantiate(_stalactitePrefab, spawnPosition, Quaternion.identity);
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

    // cutscene
    public override void ExecutePostDeathActions()
    {
        base.ExecutePostDeathActions();

        // 흑곰 사망 후 연출
        StartCoroutine(AfterBearDeathCoroutine());
    }
    public IEnumerator AfterBearDeathCoroutine()
    {
        yield return new WaitUntil(() => isEndMoveProcess);

        yield return new WaitForSeconds(2f);

        yield return ChangeImageCoroutine();
        yield return ChangeBackgroundCoroutine();

        // 최종 컷씬 재생
        cutscenePlayerList.PlayCutscene("Final CutScene");
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

        // 보스던전 BGM 틀기
        SoundManager.Instance.PlayCommonBGM("Exploration1");

        yield return new WaitUntil(() => bossClearColorChangeEffect.isEndEffect);

        var knockDownSprites = _bossKnockDownGameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sprite in knockDownSprites)
        {
            var color = sprite.color;
            color.a = 0f;
            sprite.color = color;
        }
    }

    // wait event (Attack Evaluator에 대한 대기 이벤트)
    private IEnumerator OnAttackWaitEvent()
    {
        // 해당 WaitEvent() Handler는 아직 State가 바뀌기 전에 호출되는 이벤트이므로,
        // nextAttack을 기준으로 처리해야 한다. (사실상 nextAttack이 currentAttack)
        switch (_nextAttack)
        {
            case AttackType.SlashLeft:
            case AttackType.SlashRight:
                yield return StartCoroutine(WaitEventCoroutine_Slash());
                break;
            case AttackType.Stomp:
                yield return StartCoroutine(WaitEventCoroutine_Stomp());
                break;
            case AttackType.BodySlam:
                yield return StartCoroutine(WaitEventCoroutine_BodySlam());
                break;
            case AttackType.EarthQuake:
                yield return StartCoroutine(WaitEventCoroutine_Earthquake());
                break;
        }
    }
    private IEnumerator WaitEventCoroutine_Slash()
    {
        yield return new WaitForSeconds(_slashAnimDuration);
    }
    private IEnumerator WaitEventCoroutine_Stomp()
    {
        yield return new WaitForSeconds(_stompAnimDuration);
    }
    private IEnumerator WaitEventCoroutine_BodySlam()
    {
        yield return new WaitForSeconds(_bodySlamAnimDuration);
    }
    private IEnumerator WaitEventCoroutine_Earthquake()
    {
        yield return new WaitForSeconds(_earthquakeAnimDuration);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        var current = SceneContext.Current;

        if (current == null)
            return;

        var player = current.Player;

        if (player == null)
            return;

        // 종유석 생성 높이
        Gizmos.color = Color.red;
        Vector3 pointA = new Vector3(player.transform.position.x - 25f, _ceilingHeight, player.transform.position.z);
        Vector3 pointB = new Vector3(player.transform.position.x + 25f, _ceilingHeight, player.transform.position.z);
        joonyle99.Line3D heightLine = new joonyle99.Line3D(pointA, pointB);
        Gizmos.DrawLine(heightLine.pointA, heightLine.pointB);

        Gizmos.color = Color.yellow;

        // 오른쪽 종유석 범위
        /*
        Vector3 pointC = new Vector3(player.BodyCollider.bounds.max.x + _distanceRange.Start, _ceilingHeight, player.transform.position.z);
        Vector3 pointD = new Vector3(player.BodyCollider.bounds.max.x + _distanceRange.Start, _ceilingHeight - 3f, player.transform.position.z);
        joonyle99.Line3D rightLine_left = new joonyle99.Line3D(pointC, pointD);
        Gizmos.DrawLine(rightLine_left.pointA, rightLine_left.pointB);
        */
        // Vector3 pointE = new Vector3(player.BodyCollider.bounds.max.x + _distanceRange.End, _ceilingHeight, player.transform.position.z);
        // Vector3 pointF = new Vector3(player.BodyCollider.bounds.max.x + _distanceRange.End, _ceilingHeight - 3f, player.transform.position.z);
        // joonyle99.Line3D rightLine_right = new joonyle99.Line3D(pointE, pointF);
        // Gizmos.DrawLine(rightLine_right.pointA, rightLine_right.pointB);

        // 왼쪽 종유석 범위
        /*
        Vector3 pointG = new Vector3(player.BodyCollider.bounds.min.x - _distanceRange.Start, _ceilingHeight, player.transform.position.z);
        Vector3 pointH = new Vector3(player.BodyCollider.bounds.min.x - _distanceRange.Start, _ceilingHeight - 3f, player.transform.position.z);
        joonyle99.Line3D leftLine_left = new joonyle99.Line3D(pointG, pointH);
        Gizmos.DrawLine(leftLine_left.pointA, leftLine_left.pointB);
        */
        // Vector3 pointI = new Vector3(player.BodyCollider.bounds.min.x - _distanceRange.End, _ceilingHeight, player.transform.position.z);
        // Vector3 pointJ = new Vector3(player.BodyCollider.bounds.min.x - _distanceRange.End, _ceilingHeight - 3f, player.transform.position.z);
        // joonyle99.Line3D leftLine_right = new joonyle99.Line3D(pointI, pointJ);
        // Gizmos.DrawLine(leftLine_right.pointA, leftLine_right.pointB);
    }
}