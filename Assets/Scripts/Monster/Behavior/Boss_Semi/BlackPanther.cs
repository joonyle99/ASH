using System.Collections;
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

    [SerializeField] private float _pillarInterval;
    // VineMissile
    [Header("VineMissile")]
    [Space]

    [SerializeField] private BlackPanther_VineMissile _missile;
    [SerializeField] private Transform _missileSpawnPoint;
    [SerializeField] private float _missileSpeed;

    [Space]

    [SerializeField] private ParticleSystem _sparkEffect;
    [SerializeField] private ParticleSystem _smokeEffect;

    private Vector2 _targetPos;
    private BlackPanther_VineMissile _currentMissile;

    // VinePillar
    [Header("VinePillar")]
    [Space]

    // [SerializeField] private BlackPanther_VinePillar;
    [SerializeField] private int _pillarCount;

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
        _targetPos = SceneContext.Current.Player.HeartCollider.bounds.center;
        _currentMissile = Instantiate(_missile, _missileSpawnPoint.position, Quaternion.identity);

        if (_currentMissile)
        {
            _currentMissile.Shake();
        }
    }

    public void VineMissile01_AnimEvent()
    {
        if (_currentMissile)
        {
            _currentMissile.Shoot(_targetPos, _missileSpeed);

            var spark = Instantiate(_sparkEffect, _missileSpawnPoint.position, _missileSpawnPoint.rotation);
            spark.Play();
        }
    }

    // vine pillar

    public void VinePillarPre_AnimEvent()
    {
        // �÷��̾� ��ġ Ȯ��
    }

    public void VinePillar01_AnimEvent()
    {
        // ��� ����
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

    }
}
