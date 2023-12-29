using UnityEngine;

/// <summary>
/// 박쥐 몬스터 클래스
/// </summary>
public class Bat : NormalMonster
{
    #region Function

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (IsDead)
            return;

        // 공격 범위 안에 타겟이 들어오면
        if (AttackEvaluators.IsTargetWithinAttackRange())
            ChangeState<M_AttackState>();
    }

    public override void SetUp()
    {
        // 기본 초기화
        base.SetUp();
    }

    public override void KnockBack(Vector2 forceVector)
    {
        base.KnockBack(forceVector);
    }

    public override void OnHit(int damage, Vector2 forceVector)
    {
        base.OnHit(damage, forceVector);
    }

    public override void Die()
    {
        base.Die();
    }

    #endregion
}
