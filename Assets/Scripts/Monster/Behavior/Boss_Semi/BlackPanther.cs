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

    [Header("�������������� BlackPanther Behaviour ��������������")]
    [Space]

    [Tooltip("1: Rush1\n2 : VineMissile\n3 : Rush2\n2 : VinePillar")]
    [SerializeField] private AttackType _firstAttack;
    [SerializeField] private AttackType _currentAttack;
    [SerializeField] private AttackType _nextAttack;

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

    [Header("VineMissile - VFX")]
    [Space]

    [SerializeField] private ParticleSystem _smokeEffect;
    [SerializeField] private ParticleSystem _sparkEffect;

    private Vector2 _targetPos;
    private BlackPanther_VineMissile _currentMissile;

    private float _vineMissileAnimDuration;

    [Header("____ VinePillar ____")]
    [Space]

    [SerializeField] private BlackPanther_VinePillar _pillar;
    [SerializeField] private int _pillarCount;
    [SerializeField] private float _floorHeight;
    [SerializeField] private float _pillarFarDist;
    [SerializeField] private float _minDistEachPillar;
    [SerializeField] private Range _createTimeRange;

    private List<float> _usedPosX;
    private int _allocationLimit = 40;

    [Header("VinePillar - VFX")]
    [Space]

    [SerializeField] private ParticleSystem _dustEffect;
    [SerializeField] private float _dustDistFromPillar;

    private float _vinePillarAnimDuration;

    #endregion

    #region Function

    protected override void Awake()
    {
        base.Awake();

        foreach (var clip in Animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "ani_panther_vineMissile")
                _vineMissileAnimDuration = clip.length;
            else if (clip.name == "ani_panther_vinePillar")
                _vinePillarAnimDuration = clip.length;
        }

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

        // �׷α� ���·� ����
        SetAnimatorTrigger("Groggy");
    }

    // boss base
    public override void AttackPreProcess()
    {
        // ���� ���� ���� ����
        _currentAttack = _nextAttack;

        currentAttackCount++;

        if (_currentAttack == AttackType.VineMissile)
            _totalMissileCount++;

        if (IsRage)
        {
            // TODO: 3��° �̻��� ���� IsCapturable = true�� ����
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
            // TODO: 3��° �̻��� ���� IsCapturable = false�� ����
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

        // AttackType�� ������ ���� �Ѿ�� ù ��° ������ ���ư��ϴ�.
        if (nextAttackNumber > (int)AttackType.VinePillar)
            nextAttackNumber = (int)AttackType.Rush1;

        // ���� ���� Ÿ���� �����մϴ�.
        _nextAttack = (AttackType)nextAttackNumber;
        Animator.SetInteger("NextAttackNumber", nextAttackNumber);
    }

    // vine missile
    public void VineMissilePre_AnimEvent()
    {
        var smoke = Instantiate(_smokeEffect, _missileSpawnPoint.position, Quaternion.identity);
        smoke.Play();
    }
    public void VineMissile01_AnimEvent()
    {
        _targetPos = SceneContext.Current.Player.HeartCollider.bounds.center;

        var dir = (_targetPos - (Vector2)_missileSpawnPoint.position).normalized;

        // TODO: dir�� x ������ ���Ͱ� �ٶ󺸴� ����� �ٸ��ٸ�..?

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

        // ���� ��� ���� ��ġ ���� ����
        for (int i = 0; i < _pillarCount; ++i)
        {
            /*
            // check allocation count each pillar spawn
            var allocationCount = 0;

            float newPosXInRange;
            // calculate random pillar spawn position
            do
            {
                // set random range
                var min = player.transform.position.x - _pillarFarDist;
                var max = player.transform.position.x + _pillarFarDist;
                newPosXInRange = UnityEngine.Random.Range(min, max);

                // increase allocation count while under the limit
                allocationCount++;

            } while ((_usedPosX.Any(usedPosX => Mathf.Abs(usedPosX - newPosXInRange) <= _minDistEachPillar) ||
                      (newPosXInRange >= player.BodyCollider.bounds.min.x && newPosXInRange <= player.BodyCollider.bounds.max.x))
                     && allocationCount <= _allocationLimit);
            */

            var min = player.transform.position.x - _pillarFarDist;
            var max = player.transform.position.x + _pillarFarDist;
            var dist = max - min;
            var unitDist = dist / _pillarCount;
            var spawnPos = min + unitDist * i;

            // store posX
            // _usedPosX.Add(newPosXInRange);
            _usedPosX.Add(spawnPos);
        }

        // ���� ��� ���� ��, ������ �˸��� �� ����Ʈ
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

        // ���� ��� ����
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

    // wait event
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
        // TODO: �÷��̾�� ������ ���� ������ ��ȯ�� �� �ֵ���
        // 1. ��� ������ ���°� �ȴ�
        // 2. �÷��̾� �������� �߰� ������ �����Ѵ�
        // 3. �÷��̾� �������� �̵��� ������ ����� �����Ѵ� (��Ÿ���� �Ǵ�)
        // 4. �ٽ� 1�� ���ư���

        // �߰� ��ġ ����
        var player = SceneContext.Current.Player;
        var dir = Mathf.Sign(player.transform.position.x - transform.position.x);
        var targetPosX = player.transform.position.x + dir * 30f;

        // ����� �ڵ�
        Vector3 startPoint = new Vector3(targetPosX, transform.position.y, transform.position.z);
        Vector3 endPoint = new Vector3(targetPosX, transform.position.y + 5f, transform.position.z);
        Debug.DrawLine(startPoint, endPoint, Color.cyan, 1f);

        // ��� �������� ������ ������ ��ٷ��� �Ѵ�
        yield return new WaitUntil(() => Mathf.Abs(transform.position.x - targetPosX) < 0.5f);

        Animator.SetTrigger("Stop");

        // ���� ��, ����� ���� �ٶ󺻴�
        var targetCollider2 = GroundChaseEvaluator.IsTargetWithinRange();
        if (targetCollider2) StartSetRecentDirAfterGrounded(GroundChaseEvaluator.ChaseDir);
    }
    private IEnumerator WaitEventCoroutine_VineMissile()
    {
        // �ִϸ��̼��� ���������� ��ٷ��� �Ѵ�
        yield return new WaitForSeconds(_vineMissileAnimDuration);
    }
    private IEnumerator WaitEventCoroutine_VinePillar()
    {
        // �ִϸ��̼��� ���������� ��ٷ��� �Ѵ�
        yield return new WaitForSeconds(_vinePillarAnimDuration);
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

        // ���� ����� �����Ǵ� ���� ����
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(player.transform.position.x - 50f, _floorHeight, player.transform.position.z),
            new Vector3(player.transform.position.x + 50f, _floorHeight, player.transform.position.z));

        // ���� ����� �����Ǵ� ����
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(player.transform.position.x - _pillarFarDist, _floorHeight, player.transform.position.z),
            new Vector3(player.transform.position.x - _pillarFarDist, _floorHeight + 5f, player.transform.position.z));
        Gizmos.DrawLine(new Vector3(player.transform.position.x + _pillarFarDist, _floorHeight, player.transform.position.z),
            new Vector3(player.transform.position.x + _pillarFarDist, _floorHeight + 5f, player.transform.position.z));

        // ���� ��� ���� �ּ� �Ÿ�
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(new Vector3(player.transform.position.x - _minDistEachPillar / 2f, _floorHeight, player.transform.position.z),
            new Vector3(player.transform.position.x - _minDistEachPillar / 2f, _floorHeight + 3f, player.transform.position.z));
        Gizmos.DrawLine(new Vector3(player.transform.position.x + _minDistEachPillar / 2f, _floorHeight, player.transform.position.z),
            new Vector3(player.transform.position.x + _minDistEachPillar / 2f, _floorHeight + 3f, player.transform.position.z));

        // ������� ���� ������κ����� �ּ� �Ÿ�
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(player.transform.position.x - _dustDistFromPillar / 2f, _floorHeight, player.transform.position.z),
            new Vector3(player.transform.position.x - _dustDistFromPillar / 2f, _floorHeight + 1f, player.transform.position.z));
        Gizmos.DrawLine(new Vector3(player.transform.position.x + _dustDistFromPillar / 2f, _floorHeight, player.transform.position.z),
            new Vector3(player.transform.position.x + _dustDistFromPillar / 2f, _floorHeight + 1f, player.transform.position.z));
    }
}
