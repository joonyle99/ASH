using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [Header("VineMissile - VFX")]
    [Space]

    [SerializeField] private ParticleSystem _smokeEffect;
    [SerializeField] private ParticleSystem _sparkEffect;

    private Vector2 _targetPos;
    private BlackPanther_VineMissile _currentMissile;

    [SerializeField] private float _vineMissileAnimDuration;

    [Header("____ VinePillar ____")]
    [Space]

    [SerializeField] private BlackPanther_VinePillar _pillar;
    [SerializeField] private int _pillarCount;
    [SerializeField] private float _floorHeight;
    [SerializeField] private float _pillarFarDist;
    [SerializeField] private float _minDistEachPillar;
    [SerializeField] private Range _createTimeRange;

    private List<float> _usedPosX;
    private int _allocationLimit = 30;

    [Header("VinePillar - VFX")]
    [Space]

    [SerializeField] private ParticleSystem _dustEffect;
    [SerializeField] private float _dustDistFromPillar;

    [SerializeField] private float _vinePillarAnimDuration;

    [Header("Cutscene")]
    [Space]

    [SerializeField] private ParticleSystem _shiningEyes;

    public bool IsLightingHintInRage => IsRage && TotalMissileCount % 3 == 0;

    #endregion

    #region Function

    protected override void Awake()
    {
        base.Awake();

        // ���� ��ų �ִϸ��̼� Ŭ���� ���� ����
        foreach (var clip in Animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "ani_panther_vineMissile")
                _vineMissileAnimDuration = clip.length;
            else if (clip.name == "ani_panther_vinePillar")
                _vinePillarAnimDuration = clip.length;
        }

        // ���� �ǵ����� ��� �̺�Ʈ ���
        AttackEvaluator.WaitEvent -= OnAttackWaitEvent;
        AttackEvaluator.WaitEvent += OnAttackWaitEvent;
    }
    protected override void Start()
    {
        base.Start();

        SetToFirstAttack();

        if (AttackEvaluator)
            AttackEvaluator.IsUsable = false;
    }
    public void FixedUpdate()
    {
        if (IsDead)
            return;

        if (CurrentStateIs<Monster_WalkState>())
        {
            if (GroundMovementModule)
                GroundMovementModule.WalkGround(0.7f);
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
        {
            TotalMissileCount++;
        }

        if (!IsRage)
        {
            // Rush�� ��쿡�� GodMode�� �����Ѵ�
            if (_currentAttack == AttackType.Rush1 || _currentAttack == AttackType.Rush2)
            {
                if (IsGodMode)
                    IsGodMode = false;
            }
        }
    }
    public override void AttackPostProcess()
    {
        SetToNextAttack();

        if (!IsRage)
        {
            if (_currentAttack == AttackType.Rush1 || _currentAttack == AttackType.Rush2)
            {
                if (!IsGodMode)
                    IsGodMode = true;

                currentHitCount = 0;
            }
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
            joonyle99.Util.DebugDrawX(new Vector2(posX, _floorHeight));

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
        // pillar.Opacity();
    }

    // wait event (Attack Evaluator�� ���� ��� �̺�Ʈ)
    private IEnumerator OnAttackWaitEvent()
    {
        // �ش� WaitEvent() Handler�� ���� State�� �ٲ�� ���� ȣ��Ǵ� �̺�Ʈ�̹Ƿ�,
        // nextAttack�� �������� ó���ؾ� �Ѵ�. (��ǻ� nextAttack�� currentAttack)
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
        // �߰� ��ġ ����
        var player = SceneContext.Current.Player;
        var dir = Mathf.Sign(player.transform.position.x - transform.position.x);
        float targetPosX = Mathf.Clamp(player.transform.position.x + dir * _rushBackDist, _rushableRange.Start, _rushableRange.End);

        // ����� �ڵ�
        Vector3 startPoint = new Vector3(targetPosX, transform.position.y, transform.position.z);
        Vector3 endPoint = new Vector3(targetPosX, transform.position.y + 5f, transform.position.z);
        Debug.DrawLine(startPoint, endPoint, Color.cyan, 1f);

        // ��� �������� ������ ������ ��ٸ���
        yield return new WaitUntil(() => Mathf.Abs(transform.position.x - targetPosX) < 0.5f);

        Animator.SetTrigger("Stop");

        // ���� ��, ����� ���� �ٶ󺻴�
        var targetCollider2 = GroundChaseEvaluator.IsTargetWithinRange();
        if (targetCollider2) StartSetRecentDirAfterGrounded(GroundChaseEvaluator.ChaseDir);
    }
    private IEnumerator LightingHintCoroutine()
    {
        // N�ʰ� Capturable ���·� ����� ����ϴ� �ڷ�ƾ

        Debug.Log("LightHintCoroutine ����");

        IsCapturable = true;

        yield return new WaitForSeconds(3f);

        IsCapturable = false;

        Debug.Log("LightHintCoroutine ����");
    }
    private IEnumerator WaitEventCoroutine_VineMissile()
    {
        // �ִϸ��̼��� ���������� ��ٷ��� �Ѵ�
        yield return new WaitForSeconds(_vineMissileAnimDuration);

        // Debug.Log("Vine Missile Animation�� ����Ǿ����ϴ�. ���� Attack Evaluator�� ��Ÿ���� �������ϴ�");

        // Light Hint�� �����ϴ� ���
        if (IsLightingHintInRage)
        {
            yield return LightingHintCoroutine();
        }
    }
    private IEnumerator WaitEventCoroutine_VinePillar()
    {
        // �ִϸ��̼��� ���������� ��ٷ��� �Ѵ�
        yield return new WaitForSeconds(_vinePillarAnimDuration);

        // Debug.Log("Vine Pillar Animation�� ����Ǿ����ϴ�. ���� Attack Evaluator�� ��Ÿ���� �������ϴ�");
    }

    // effects
    public override void ExecutePostDeathActions()
    {
        base.ExecutePostDeathActions();

        // ��ǥ�� ��� �� ����
        StartCoroutine(AfterBlackPantherCoroutine());
    }
    public IEnumerator AfterBlackPantherCoroutine()
    {
        yield return new WaitUntil(() => isEndMoveProcess);

        // ���� �ƾ� ���
        cutscenePlayerList.PlayCutscene("Final CutScene");
    }

    // etc
    public void ShineEyes()
    {
        _shiningEyes.Play();
    }
    public void ActivateAttackEvaluator()
    {
        if (AttackEvaluator)
            AttackEvaluator.IsUsable = true;
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
