using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BlackPanther : BossBehaviour, ILightCaptureListener
{
    public enum AttackType
    {
        None = 0,

        Rush1,
        VineMissile,
        Rush2,
        VinePillar,
    }

    #region Variable

    [Header("――――――― BlackPanther Behaviour ―――――――")]
    [Space]

    [Tooltip("1: Rush1\n2 : VineMissile\n3 : Rush2\n4 : VinePillar")]
    [SerializeField] private AttackType _firstAttack;
    [SerializeField] private AttackType _currentAttack;
    [SerializeField] private AttackType _nextAttack;

    [Header("____ Rush ____")]
    [Space]

    [SerializeField] private float _rushBackDist = 10f;
    [SerializeField] private Range _rushableRange;

    [Header("____ VineMissile ____")]
    [Space]

    [SerializeField] private BlackPanther_VineMissile _missile;
    [SerializeField] private Transform _missileSpawnPoint;
    [SerializeField] private float _missileSpeed;

    [Space]

    [SerializeField] private int _totalMissileCount;
    public int TotalMissileCount
    {
        get => _totalMissileCount;
        private set => _totalMissileCount = value;
    }

    private bool _isInterruptVineMissile = false;

    [Header("VineMissile - VFX")]
    [Space]

    [SerializeField] private ParticleHelper _smokeEffect;
    [SerializeField] private ParticleHelper _sparkEffect;

    private Vector2 _targetPos;
    private BlackPanther_VineMissile _currentMissile;

    private float _vineMissileAnimDuration;

    [Header("____ VinePillar ____")]
    [Space]

    [SerializeField] private BlackPanther_VinePillar _pillar;
    [SerializeField] private int _pillarCount;
    [SerializeField] private float _floorHeight;
    [SerializeField] private float _pillarFarDist;
    [SerializeField] private Range _createTimeRange;

    private List<float> _usedPosX;
    private int _allocationLimit = 30;

    [Header("VinePillar - VFX")]
    [Space]

    [SerializeField] private ParticleHelper _dustEffect;
    [SerializeField] private float _dustDistFromPillar;

    private float _vinePillarAnimDuration;

    [Header("Cutscene")]
    [Space]

    [SerializeField] private ParticleHelper _shinningEffect;
    [SerializeField] private ParticleHelper _twinkleEffect;

    public bool IsLightingHintInRage => IsRage && (TotalMissileCount % 3) == 0;

    #endregion

    #region Function

    protected override void Awake()
    {
        base.Awake();

        // 덩쿨 스킬 애니메이션 클립의 길이 추출
        foreach (var clip in Animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "ani_panther_vineMissile")
                _vineMissileAnimDuration = clip.length;
            else if (clip.name == "ani_panther_vinePillar")
                _vinePillarAnimDuration = clip.length;
        }

        // 공격 판독기의 대기 이벤트 등록
        AttackEvaluator.WaitEvent -= OnAttackWaitEvent;
        AttackEvaluator.WaitEvent += OnAttackWaitEvent;

        rageTargetHurtCount = finalTargetHurtCount - 6;
    }
    protected override void Start()
    {
        base.Start();

        SetToFirstAttack();

        if (AttackEvaluator)
            AttackEvaluator.IsUsable = false;

        IsGodMode = false;
    }
    private void FixedUpdate()
    {
        if (IsDead)
            return;

        if (CurrentStateIs<Monster_WalkState>())
        {
            if (GroundMovementModule)
                GroundMovementModule.WalkGround(1.3f);
        }
        else if (CurrentStateIs<BossAttackState>())
        {
            if (_currentAttack == AttackType.Rush1 || _currentAttack == AttackType.Rush2)
            {
                if (GroundMovementModule)
                    GroundMovementModule.WalkGround();
            }
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
    }

    // boss base
    public override void AttackPreProcess()
    {
        // 현재 공격 상태 변경
        _currentAttack = _nextAttack;

        currentAttackCount++;

        if (IsRage && _currentAttack is AttackType.VineMissile)
        {
            TotalMissileCount++;
        }
    }
    public override void AttackPostProcess()
    {
        SetToNextAttack();
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
    }

    // basic
    private void SetToFirstAttack()
    {
        int firstAttackNumber = (int)_firstAttack;
        _nextAttack = (AttackType)firstAttackNumber;
        Animator.SetInteger("NextAttackNumber", firstAttackNumber);
    }
    private void SetToNextAttack()
    {
        // 1, 2, 3, 4 / 1, 2, 3, 4 ...
        int nextAttackNumber = (int)_currentAttack + 1;

        // AttackType의 마지막 값을 넘어가면 첫 번째 값으로 돌아갑니다.
        if (nextAttackNumber > (int)AttackType.VinePillar)
            nextAttackNumber = (int)AttackType.Rush1;

        // 다음 공격 타입을 설정합니다.
        _nextAttack = (AttackType)nextAttackNumber;
        Animator.SetInteger("NextAttackNumber", nextAttackNumber);
    }

    // vine missile
    public void VineMissile01_AnimEvent()
    {
        if (_isInterruptVineMissile) return;

        _targetPos = SceneContext.Current.Player.HeartCollider.bounds.center;

        // 오른쪽을 보고 있으면 플레이어가 오른쪽에 있을 때만 미사일을 발사한다
        // 왼쪽을 보고 있으면 플레이어가 왼쪽에 있을 때만 미사일을 발사한다
        bool isDifferentDir = (RecentDir > 0 && _missileSpawnPoint.position.x > _targetPos.x) ||
                            (RecentDir < 0 && _missileSpawnPoint.position.x < _targetPos.x);
        if (isDifferentDir)
        {
            _isInterruptVineMissile = true;
            SetAnimatorTrigger("StopVineMissile");
            return;
        }

        var smoke = Instantiate(_smokeEffect, _missileSpawnPoint.position, Quaternion.identity);
        smoke.Play();

        var dir = (_targetPos - (Vector2)_missileSpawnPoint.position).normalized;

        // TODO: dir의 x 방향이 몬스터가 바라보는 방향과 다르다면..?

        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        _currentMissile = Instantiate(_missile, _missileSpawnPoint.position, Quaternion.Euler(0f, 0f, angle));
        _currentMissile.SetActor(this);
        _currentMissile.Shoot(dir, _missileSpeed);

        var spark = Instantiate(_sparkEffect, _missileSpawnPoint.position, Quaternion.Euler(0f, -90f * RecentDir, 0f));
        spark.Play();
    }

    // vine pillar
    public void VinePillar01_AnimEvent()
    {
        var player = SceneContext.Current.Player;

        if (player == null)
        {
            Debug.LogError("Player is null");
            return;
        }

        _usedPosX = new List<float>();

        // 넝쿨 기둥 생성 위치 설정 로직
        for (int i = 0; i < _pillarCount; ++i)
        {
            var min = player.transform.position.x - _pillarFarDist;
            var max = player.transform.position.x + _pillarFarDist;
            var dist = max - min;
            var unitDist = dist / (_pillarCount - 1);
            var spawnPos = min + unitDist * i;

            _usedPosX.Add(spawnPos);
        }

        // 넝쿨 기둥 생성 전, 위험을 알리는 흙 이펙트
        foreach (var posX in _usedPosX)
        {
            var leftPos = new Vector2(posX - _dustDistFromPillar, _floorHeight);
            var rightPos = new Vector2(posX + _dustDistFromPillar, _floorHeight);

            // for debug
            joonyle99.Util.DebugDrawX(new Vector2(posX, _floorHeight));

            var leftDust = Instantiate(_dustEffect, leftPos, Quaternion.Euler(0f, 0f, 180f));
            var rightDust = Instantiate(_dustEffect, rightPos, Quaternion.identity);

            leftDust.Play();
            rightDust.Play();
        }

        // 넝쿨 기둥 생성
        foreach (var posX in _usedPosX)
        {
            StartCoroutine(CreateVinePillar(posX));
        }
    }
    public IEnumerator CreateVinePillar(float createPosX)
    {
        yield return new WaitForSeconds(_createTimeRange.Random());

        var pos = new Vector2(createPosX, _floorHeight);
        var pillar = Instantiate(_pillar, pos, Quaternion.identity);
    }

    // wait event (Attack Evaluator에 대한 대기 이벤트)
    private IEnumerator OnAttackWaitEvent()
    {
        // 해당 WaitEvent() Handler는 아직 State가 바뀌기 전에 호출되는 이벤트이므로,
        // nextAttack을 기준으로 처리해야 한다. (사실상 nextAttack이 currentAttack)
        switch (_nextAttack)
        {
            case AttackType.Rush1:
            case AttackType.Rush2:
                yield return WaitEventCoroutine_Rush();
                break;
            case AttackType.VineMissile:
                yield return WaitEventCoroutine_VineMissile();

                // Light Hint
                if (IsLightingHintInRage)
                {
                    yield return LightingHintCoroutine();
                }

                break;
            case AttackType.VinePillar:
                yield return WaitEventCoroutine_VinePillar();
                break;
        }
    }
    private IEnumerator WaitEventCoroutine_Rush()
    {
        // 추격 위치 설정
        var player = SceneContext.Current.Player;
        var direction = Mathf.Sign(player.transform.position.x - transform.position.x);
        float targetPosX = Mathf.Clamp(player.transform.position.x + direction * _rushBackDist, _rushableRange.Start, _rushableRange.End);

        // 디버그 코드
        Vector3 startPoint = new Vector3(targetPosX, transform.position.y, transform.position.z);
        Vector3 endPoint = new Vector3(targetPosX, transform.position.y + 5f, transform.position.z);
        Debug.DrawLine(startPoint, endPoint, Color.cyan, 1f);

        // 대상 뒷편으로 지나갈 때까지 기다린다
        yield return new WaitUntil(() => Mathf.Abs(transform.position.x - targetPosX) < 0.5f);

        Animator.SetTrigger("Stop");

        // 멈춘 후, 대상을 향해 바라본다
        var targetCollider2 = GroundChaseEvaluator.IsTargetWithinRange();
        if (targetCollider2) StartSetRecentDirAfterGrounded(GroundChaseEvaluator.ChaseDir);
    }
    private IEnumerator WaitEventCoroutine_VineMissile()
    {
        // 애니메이션이 끝날때까지 기다려야 한다

        // 다른 WaitEvent와는 조금 다른 형태로, 언제든지 종료하기 위해 WaitForSeconds를 사용하지 않는다

        float eTime = 0f;
        while (eTime < _vineMissileAnimDuration)
        {
            if (_isInterruptVineMissile)
            {
                _isInterruptVineMissile = false;
                yield break;
            }

            eTime += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator WaitEventCoroutine_VinePillar()
    {
        // 애니메이션이 끝날때까지 기다려야 한다
        yield return new WaitForSeconds(_vinePillarAnimDuration);

        // Debug.Log("Vine Pillar Animation이 종료되었습니다. 이제 Attack Evaluator의 쿨타임이 끝났습니다");
    }
    private IEnumerator LightingHintCoroutine()
    {
        // N초간 Capturable 상태로 만들며 대기하는 코루틴

        Debug.Log("LightHintCoroutine 실행");

        IsCapturable = true;

        /*
         * TODO: 빛의 힌트 연출
         * 꼬리 내리는 애니메이션
         * 이마 다이아 문양이 흰색으로 반짝 빛나는 이펙트
         * 효과음을 추가’하기로 하였습니다.
         */

        var count = 4;
        var interval = 1.2f;

        PlayMultipleSound("Twinkle", count, interval);

        for (int i = 0; i < count; i++)
        {
            _twinkleEffect.Play();
            yield return new WaitForSeconds(_twinkleEffect.GetTwinkleLifeTime());
            _twinkleEffect.Stop();
        }

        IsCapturable = false;

        Debug.Log("LightHintCoroutine 종료");
    }

    // effects
    public override void ExecutePostDeathActions()
    {
        base.ExecutePostDeathActions();

        // 흑표범 사망 후 연출
        StartCoroutine(AfterBlackPantherCoroutine());
    }
    public IEnumerator AfterBlackPantherCoroutine()
    {
        yield return new WaitUntil(() => isEndMoveProcess);

        yield return new WaitForSeconds(2f);

        // 최종 컷씬 재생
        cutscenePlayerList.PlayCutscene("Final CutScene");
    }

    // etc
    public void ShineEyes()
    {
        _shinningEffect.Play();
    }
    public void RoarProcess()
    {
        if (IsActiveLuminescence == false)
        {
            SetActiveLuminescence(true);
            currentHitCount = 0;
            IsGodMode = true;
        }
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        // Rushable Range
        Gizmos.color = Color.green;
        joonyle99.Util.GizmosDrawVerticalLine(new Vector3(_rushableRange.Start, transform.position.y, transform.position.z));
        joonyle99.Util.GizmosDrawVerticalLine(new Vector3(_rushableRange.End, transform.position.y, transform.position.z));

        var current = SceneContext.Current;

        if (current == null)
            return;

        var player = current.Player;

        if (player == null)
            return;

        // 넝쿨 기둥이 생성되는 땅의 높이
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(player.transform.position.x - 50f, _floorHeight, player.transform.position.z),
            new Vector3(player.transform.position.x + 50f, _floorHeight, player.transform.position.z));

        // 넝쿨 기둥이 생성되는 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(player.transform.position.x - _pillarFarDist, _floorHeight, player.transform.position.z),
            new Vector3(player.transform.position.x - _pillarFarDist, _floorHeight + 5f, player.transform.position.z));
        Gizmos.DrawLine(new Vector3(player.transform.position.x + _pillarFarDist, _floorHeight, player.transform.position.z),
            new Vector3(player.transform.position.x + _pillarFarDist, _floorHeight + 5f, player.transform.position.z));

        // 흙먼지의 넝쿨 기둥으로부터의 최소 거리
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(player.transform.position.x - _dustDistFromPillar / 2f, _floorHeight, player.transform.position.z),
            new Vector3(player.transform.position.x - _dustDistFromPillar / 2f, _floorHeight + 1f, player.transform.position.z));
        Gizmos.DrawLine(new Vector3(player.transform.position.x + _dustDistFromPillar / 2f, _floorHeight, player.transform.position.z),
            new Vector3(player.transform.position.x + _dustDistFromPillar / 2f, _floorHeight + 1f, player.transform.position.z));
    }
}
