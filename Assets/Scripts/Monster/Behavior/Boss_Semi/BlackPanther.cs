using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BlackPanther : BossBehavior, ILightCaptureListener
{
    public enum AttackType
    {
        Null = 0,

        VineMissile,
        VineColumn
    }

    #region Variable

    [Header("BlackPanther")]
    [Space]

    [Tooltip("1 : VineMissile\r\n2 : VineColumn\r\n")]
    [SerializeField] private Range _attackTypeRange;
    [SerializeField] private AttackType _currentAttack;
    [SerializeField] private AttackType _nextAttack;

    [SerializeField] private float _pillarInterval;
    // VineMissile
    [Header("VineMissile")]
    [Space]

    [SerializeField] private BlackPanther_VineMissile _missile;
    [SerializeField] private int _missileCount;
    [SerializeField] private float _missileInterval;
    [SerializeField] private float _missileSpeed;
    [SerializeField] private float _missileAngle;
    private Vector2 _playerPos;

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

        // 체력 감소
        // TotalHitCount++;
        currentHitCount++;
        CurHp -= MonsterDefine.BossHealthUnit;

        CheckHurtState();

        return IAttackListener.AttackResult.Success;
    }
    public override void Die(bool isHitBoxDisable = true, bool isDeathProcess = true)
    {
        // 보스는 사망 이펙트를 재생하지 않는다
        base.Die(true, false);

        StartCoroutine(SlowMotionCoroutine(5f));
    }

    public void OnLightEnter(LightCapturer capturer, LightSource lightSource)
    {
        if (IsGroggy)
            return;

        // 그로기 상태로 진입
        SetAnimatorTrigger("Groggy");
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

        currentAttackCount++;
    }
    public override void AttackPostProcess()
    {
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

        // 그로기 상태 해제되며 피격
        if (currentHitCount >= targetHitCount)
        {
            SetAnimatorTrigger("Hurt");
        }
    }

    // vine missile

    public void VineMissilePre_AnimEvent()
    {
        // 플레이어 위치 확인
    }

    public void VineMissile01_AnimEvent()
    {
        // 미사일 발사
    }

    // vine pillar

    public void VinePillarPre_AnimEvent()
    {
        // 플레이어 위치 확인
    }

    public void VinePillar01_AnimEvent()
    {
        // 기둥 생성
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
