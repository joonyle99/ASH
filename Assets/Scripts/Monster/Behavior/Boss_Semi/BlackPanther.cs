using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public sealed class BlackPanther : BossBehavior, ILightCaptureListener
{
    public enum AttackType
    {
        Null = 0,

        VineMissile,
        VinePillar
    }

    #region Variable

    [Header("BlackPanther")]
    [Space]

    [Tooltip("1 : VineMissile\r\n2 : VinePillar\r\n")]
    [SerializeField] private Range _attackTypeRange;
    [SerializeField] private AttackType _currentAttack;
    [SerializeField] private AttackType _nextAttack;

    [Header("VineMissile")]
    [Space]

    [SerializeField] private BlackPanther_VineMissile _missile;

    [Space]

    [SerializeField] private Transform _missileSpawnPoint;
    [SerializeField] private float _missileSpeed;

    [Space]

    [SerializeField] private ParticleSystem _smokeEffect;
    [SerializeField] private ParticleSystem _sparkEffect;

    private Vector2 _targetPos;
    private BlackPanther_VineMissile _currentMissile;

    [Header("VinePillar")]
    [Space]

    [SerializeField] private BlackPanther_VinePillar _pillar;

    [Space]

    [SerializeField] private int _pillarCount;
    [SerializeField] private float _floorHeight;
    [SerializeField] private float _pillarSpawnDistance;
    [SerializeField] private float _minDistanceEachPillar;
    [SerializeField] private float _minDistanceFromCaster;
    [SerializeField] private Range _createTimeRange;

    private List<float> _usedPosX;

    [Space]

    [SerializeField] private ParticleSystem _dustEffect;
    [SerializeField] private float _distanceFromPillar;

    #endregion

    #region Function

    protected override void Start()
    {
        base.Start();

        // overwrite
        monsterData.MaxHp = finalTargetHurtCount * MonsterDefine.BossHealthUnit;
        CurHp = monsterData.MaxHp;

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
        // TotalHitCount++;
        currentHitCount++;
        CurHp -= MonsterDefine.BossHealthUnit;

        CheckHurtState();

        return IAttackListener.AttackResult.Success;
    }
    public override void Die(bool isHitBoxDisable = true, bool isDeathProcess = true)
    {
        // ������ ��� ����Ʈ�� ������� �ʴ´�
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

    // boss base
    public override void AttackPreProcess()
    {
        // ���� ���� ���� ����
        _currentAttack = _nextAttack;

        if (_currentAttack is AttackType.Null || _nextAttack is AttackType.Null)
        {
            Debug.LogError("AttackType is Null");
            return;
        }

        currentAttackCount++;
    }
    public override void AttackPostProcess()
    {
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

        currentHitCount = 0;
    }

    // basic
    private void SetToRandomAttack()
    {
        int nextAttackNumber = (int)_attackTypeRange.Random();
        _nextAttack = (AttackType)nextAttackNumber;
        Animator.SetInteger("NextAttackNumber", nextAttackNumber);
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

    // vine missile
    public void VineMissilePre01_AnimEvent()
    {
        var smoke = Instantiate(_smokeEffect, _missileSpawnPoint.position, Quaternion.Euler(-90, 0, 0));
        smoke.Play();
    }
    public void VineMissilePre02_AnimEvent()
    {
        _currentMissile = Instantiate(_missile, _missileSpawnPoint.position, Quaternion.identity);

        if (_currentMissile)
        {
            _currentMissile.Shake();
        }
    }
    public void VineMissile01_AnimEvent()
    {
        _targetPos = SceneContext.Current.Player.HeartCollider.bounds.center;

        if (_currentMissile)
        {
            _currentMissile.Shoot(_targetPos, _missileSpeed);

            var spark = Instantiate(_sparkEffect, _missileSpawnPoint.position, _missileSpawnPoint.rotation);
            spark.Play();
        }
    }

    // vine pillar
    public void VinePillar01_AnimEvent()
    {
        _usedPosX = new List<float>();

        // ���� ��� ���� ��ġ ���� ����
        for (int i = 0; i < _pillarCount; ++i)
        {
            // reallocation count limit
            var posReallocationCount = 0;

            // calculate pillar spawn position
            float newPosXInRange;
            do
            {
                newPosXInRange = Random.Range(MainBodyCollider2D.bounds.min.x - _pillarSpawnDistance,
                    MainBodyCollider2D.bounds.max.x + _pillarSpawnDistance);

                posReallocationCount++;

            } while ((_usedPosX.Any(usedPosX => Mathf.Abs(usedPosX - newPosXInRange) <= _minDistanceEachPillar) ||
                      Mathf.Abs(MainBodyCollider2D.bounds.center.x - newPosXInRange) <= _minDistanceFromCaster) &&
                     posReallocationCount <= 20);

            // store posX
            _usedPosX.Add(newPosXInRange);
        }

        // ���� ��� ���� ��, ������ �˸��� �� ����Ʈ
        foreach (var posX in _usedPosX)
        {
            var leftPos = new Vector2(posX - _distanceFromPillar, _floorHeight);
            var rightPos = new Vector2(posX + _distanceFromPillar, _floorHeight);

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
        pillar.Opacity(0.5f);
    }

    // effects
    public IEnumerator SlowMotionCoroutine(float duration)
    {
        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        // ���� ����� �����Ǵ� ���� ��ġ
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(transform.position.x - 25f, _floorHeight, transform.position.z),
            new Vector3(transform.position.x + 25f, _floorHeight, transform.position.z));

        // ���� ����� �����Ǵ� ����
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(transform.position.x - _pillarSpawnDistance, _floorHeight, transform.position.z),
            new Vector3(transform.position.x - _pillarSpawnDistance, _floorHeight + 5f, transform.position.z));
        Gizmos.DrawLine(new Vector3(transform.position.x + _pillarSpawnDistance, _floorHeight, transform.position.z),
            new Vector3(transform.position.x + _pillarSpawnDistance, _floorHeight + 5f, transform.position.z));

        // �����ڷκ��� ������ �Ÿ�
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(new Vector3(transform.position.x - _minDistanceFromCaster, _floorHeight, transform.position.z),
            new Vector3(transform.position.x - _minDistanceFromCaster, _floorHeight + 3f, transform.position.z));
        Gizmos.DrawLine(new Vector3(transform.position.x + _minDistanceFromCaster, _floorHeight, transform.position.z),
            new Vector3(transform.position.x + _minDistanceFromCaster, _floorHeight + 3f, transform.position.z));
    }
}
