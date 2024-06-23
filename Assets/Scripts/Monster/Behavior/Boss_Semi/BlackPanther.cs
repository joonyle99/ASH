using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Tooltip("1: Rush1\n2 : VineMissile\n3 : Rush2\n2 : VinePillar")]
    [SerializeField] private AttackType _currentAttack;
    [SerializeField] private AttackType _nextAttack;

    [Header("____ VineMissile ____")]
    [Space]

    [SerializeField] private bool _isMissiling;
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

    [Header("VineMissile - VFX")]
    [Space]

    [SerializeField] private ParticleSystem _smokeEffect;
    [SerializeField] private ParticleSystem _sparkEffect;

    private Vector2 _targetPos;
    private BlackPanther_VineMissile _currentMissile;

    [Header("____ VinePillar ____")]
    [Space]

    [SerializeField] private bool _isPillaring;
    [SerializeField] private BlackPanther_VinePillar _pillar;
    [SerializeField] private int _pillarCount;
    [SerializeField] private float _floorHeight;
    [SerializeField] private float _pillarFarDistFromCaster;
    [SerializeField] private float _minDistEachPillar;
    [SerializeField] private Range _createTimeRange;

    private List<float> _usedPosX;

    [Header("VinePillar - VFX")]
    [Space]

    [SerializeField] private ParticleSystem _dustEffect;
    [SerializeField] private float _dustDistFromPillar;

    #endregion

    #region Function

    protected override void Awake()
    {
        base.Awake();

        AttackEvaluator.WaitEvent -= HandleFunc;
        AttackEvaluator.WaitEvent += HandleFunc;
    }
    protected override void Start()
    {
        base.Start();

        SetToFirstAttack();
    }
    public void FixedUpdate()
    {
        if (IsDead)
            return;

        if (CurrentStateIs<BossAttackState>() &&
            (_currentAttack == AttackType.Rush1 || _currentAttack == AttackType.Rush2))
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

        if (_currentAttack == AttackType.VineMissile)
            _totalMissileCount++;

        if (IsRage)
        {
            // TODO: 3번째 미사일 때만 IsCapturable = true로 변경
            if (_totalMissileCount % 3 == 0)
            {
                if (!IsCapturable)
                    IsCapturable = true;
            }
        }
        else
        {
            if (!IsGodMode)
                IsGodMode = true;
        }
    }
    public override void AttackPostProcess()
    {
        SetToNextAttack();

        if (IsRage)
        {
            // TODO: 3번째 미사일 때만 IsCapturable = false로 변경
            if (_totalMissileCount % 3 == 0)
            {
                if (IsCapturable)
                {
                    IsCapturable = false;
                }
            }
        }
        else
        {
            if (IsGodMode)
                IsGodMode = false;
        }
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
        int nextAttackNumber = (int)AttackType.Rush1;
        _nextAttack = (AttackType)nextAttackNumber;
        Animator.SetInteger("NextAttackNumber", nextAttackNumber);
    }
    private void SetToNextAttack()
    {
        int nextAttackNumber = (int)_currentAttack + 1;

        // AttackType의 마지막 값을 넘어가면 첫 번째 값으로 돌아갑니다.
        if (nextAttackNumber > (int)AttackType.VinePillar)
            nextAttackNumber = (int)AttackType.Rush1;

        // 다음 공격 타입을 설정합니다.
        _nextAttack = (AttackType)nextAttackNumber;
        Animator.SetInteger("NextAttackNumber", nextAttackNumber);
    }

    // vine missile
    public void StartMissile()
    {
        Debug.Log("start missile");
        _isMissiling = true;
    }
    public void EndMissile()
    {
        _isMissiling = false;
    }
    public void VineMissilePre_AnimEvent()
    {
        var smoke = Instantiate(_smokeEffect, _missileSpawnPoint.position, Quaternion.identity);
        smoke.Play();
    }
    public void VineMissile01_AnimEvent()
    {
        _targetPos = SceneContext.Current.Player.HeartCollider.bounds.center;

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
    public void StartPillar()
    {
        _isPillaring = true;
    }
    public void EndPillar()
    {
        _isPillaring = false;
    }
    public void VinePillar01_AnimEvent()
    {
        _usedPosX = new List<float>();

        // 넝쿨 기둥 생성 위치 설정 로직
        for (int i = 0; i < _pillarCount; ++i)
        {
            // reallocation count limit
            var posReallocationCount = 0;

            // calculate pillar spawn position
            float newPosXInRange;
            do
            {
                newPosXInRange = UnityEngine.Random.Range(transform.position.x - _pillarFarDistFromCaster,
                    transform.position.x + _pillarFarDistFromCaster);

                posReallocationCount++;

            } while ((_usedPosX.Any(usedPosX => Mathf.Abs(usedPosX - newPosXInRange) <= _minDistEachPillar) ||
                      (newPosXInRange >= MainBodyCollider2D.bounds.min.x &&
                       newPosXInRange <= MainBodyCollider2D.bounds.max.x)) &&
                     posReallocationCount <= 20);

            // store posX
            _usedPosX.Add(newPosXInRange);
        }

        // 넝쿨 기둥 생성 전, 위험을 알리는 흙 이펙트
        foreach (var posX in _usedPosX)
        {
            var leftPos = new Vector2(posX - _dustDistFromPillar, _floorHeight);
            var rightPos = new Vector2(posX + _dustDistFromPillar, _floorHeight);

            // for debug
            joonyle99.Util.DrawX(new Vector2(posX, _floorHeight));

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
        pillar.Opacity();
    }

    private IEnumerator HandleFunc()
    {
        switch (_nextAttack)
        {
            case AttackType.Rush1:
            case AttackType.Rush2:
                yield return StartCoroutine(WaitEventCoroutine_Rush());
                break;
            case AttackType.VineMissile:
                yield return StartCoroutine(WaitEventCoroutine_VineMissile());
                break;
            case AttackType.VinePillar:
                yield return StartCoroutine(WaitEventCoroutine_VinePillar());
                break;
        }
    }

    private IEnumerator WaitEventCoroutine_Rush()
    {
        // TODO: 플레이어에게 돌진할 때만 방향을 전환할 수 있도록
        // 1. 사용 가능한 상태가 된다
        // 2. 플레이어 방향으로 추격 방향을 설정한다
        // 3. 플레이어 뒷편으로 이동할 때까지 사용을 제한한다 (쿨타임을 건다)
        // 4. 다시 1로 돌아간다

        // 플레이어 뒷편으로 지나갈 때까지 기다려야 한다

        var player = SceneContext.Current.Player;
        var dir = Mathf.Sign(player.transform.position.x - transform.position.x);
        var targetPosX = player.transform.position.x + dir * 30f;

        Vector3 startPoint = new Vector3(targetPosX, transform.position.y, transform.position.z);
        Vector3 endPoint = new Vector3(targetPosX, transform.position.y + 5f, transform.position.z);
        Debug.DrawLine(startPoint, endPoint, Color.cyan, 1f);

        yield return new WaitUntil(() => Mathf.Abs(transform.position.x - targetPosX) < 0.5f);

        Animator.SetTrigger("Stop");
    }
    private IEnumerator WaitEventCoroutine_VineMissile()
    {
        // 애니메이션이 끝날때까지 기다려야 한다
        Debug.Log("wait missile ending");

        yield return new WaitUntil(() => !_isMissiling);
    }
    private IEnumerator WaitEventCoroutine_VinePillar()
    {
        // 애니메이션이 끝날때까지 기다려야 한다

        yield return new WaitUntil(() => !_isPillaring);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        // 넝쿨 기둥이 생성되는 땅의 위치
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(transform.position.x - 25f, _floorHeight, transform.position.z),
            new Vector3(transform.position.x + 25f, _floorHeight, transform.position.z));

        // 넝쿨 기둥이 생성되는 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(transform.position.x - _pillarFarDistFromCaster, _floorHeight, transform.position.z),
            new Vector3(transform.position.x - _pillarFarDistFromCaster, _floorHeight + 5f, transform.position.z));
        Gizmos.DrawLine(new Vector3(transform.position.x + _pillarFarDistFromCaster, _floorHeight, transform.position.z),
            new Vector3(transform.position.x + _pillarFarDistFromCaster, _floorHeight + 5f, transform.position.z));

        // 넝쿨 기둥 사이 최소 거리
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(new Vector3(transform.position.x - _minDistEachPillar / 2f, _floorHeight, transform.position.z),
            new Vector3(transform.position.x - _minDistEachPillar / 2f, _floorHeight + 3f, transform.position.z));
        Gizmos.DrawLine(new Vector3(transform.position.x + _minDistEachPillar / 2f, _floorHeight, transform.position.z),
            new Vector3(transform.position.x + _minDistEachPillar / 2f, _floorHeight + 3f, transform.position.z));

        // 흙먼지의 넝쿨 기둥으로부터의 최소 거리
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(transform.position.x - _dustDistFromPillar / 2f, _floorHeight, transform.position.z),
            new Vector3(transform.position.x - _dustDistFromPillar / 2f, _floorHeight + 1f, transform.position.z));
        Gizmos.DrawLine(new Vector3(transform.position.x + _dustDistFromPillar / 2f, _floorHeight, transform.position.z),
            new Vector3(transform.position.x + _dustDistFromPillar / 2f, _floorHeight + 1f, transform.position.z));
    }
}
