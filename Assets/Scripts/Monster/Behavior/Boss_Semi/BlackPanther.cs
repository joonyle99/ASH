using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Unity.VisualScripting;
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

    [Tooltip("Luminescence")]
    [SerializeField] private GameObject _luminescence;

    [Header("VineMissile")]
    [Space]

    [SerializeField] private BlackPanther_VineMissile _missile;
    [SerializeField] private Transform _missileSpawnPoint;
    [SerializeField] private float _missileSpeed;

    [Header("VineMissile - VFX")]
    [Space]

    [SerializeField] private ParticleSystem _smokeEffect;
    [SerializeField] private ParticleSystem _sparkEffect;

    private Vector2 _targetPos;
    private BlackPanther_VineMissile _currentMissile;

    [Header("VinePillar")]
    [Space]

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

    public bool isActiveLuminescence => _luminescence.activeInHierarchy;

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

        if (!isActiveLuminescence)
        {
            IsGodMode = true;
        }

        if (_currentAttack is AttackType.Null || _nextAttack is AttackType.Null)
        {
            Debug.LogError("AttackType is Null");
            return;
        }
    }
    public override void AttackPostProcess()
    {
        SetToRandomAttack();

        if (!isActiveLuminescence)
        {
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

        if (!isActiveLuminescence)
        {
            IsGodMode = true;
            SetAnimatorTrigger("Roar");
            return;
        }

        if (CurrentStateIs<BossGroggyState>())
        {
            SetAnimatorTrigger("Hurt");
        }
    }
    public void SetActiveLuminescence(bool isBool)
    {
        _luminescence.SetActive(isBool);
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
                newPosXInRange = Random.Range(transform.position.x - _pillarFarDistFromCaster,
                    transform.position.x + _pillarFarDistFromCaster);

                posReallocationCount++;

            } while ((_usedPosX.Any(usedPosX => Mathf.Abs(usedPosX - newPosXInRange) <= _minDistEachPillar) ||
                      (newPosXInRange >= MainBodyCollider2D.bounds.min.x &&
                       newPosXInRange <= MainBodyCollider2D.bounds.max.x)) &&
                     posReallocationCount <= 20);

            // store posX
            _usedPosX.Add(newPosXInRange);
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
        Gizmos.DrawLine(new Vector3(transform.position.x - _pillarFarDistFromCaster, _floorHeight, transform.position.z),
            new Vector3(transform.position.x - _pillarFarDistFromCaster, _floorHeight + 5f, transform.position.z));
        Gizmos.DrawLine(new Vector3(transform.position.x + _pillarFarDistFromCaster, _floorHeight, transform.position.z),
            new Vector3(transform.position.x + _pillarFarDistFromCaster, _floorHeight + 5f, transform.position.z));

        // ���� ��� ���� �ּ� �Ÿ�
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(new Vector3(transform.position.x - _minDistEachPillar / 2f, _floorHeight, transform.position.z),
            new Vector3(transform.position.x - _minDistEachPillar / 2f, _floorHeight + 3f, transform.position.z));
        Gizmos.DrawLine(new Vector3(transform.position.x + _minDistEachPillar / 2f, _floorHeight, transform.position.z),
            new Vector3(transform.position.x + _minDistEachPillar / 2f, _floorHeight + 3f, transform.position.z));

        // ������� ���� ������κ����� �ּ� �Ÿ�
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(transform.position.x - _dustDistFromPillar / 2f, _floorHeight, transform.position.z),
            new Vector3(transform.position.x - _dustDistFromPillar / 2f, _floorHeight + 1f, transform.position.z));
        Gizmos.DrawLine(new Vector3(transform.position.x + _dustDistFromPillar / 2f, _floorHeight, transform.position.z),
            new Vector3(transform.position.x + _dustDistFromPillar / 2f, _floorHeight + 1f, transform.position.z));
    }
}
