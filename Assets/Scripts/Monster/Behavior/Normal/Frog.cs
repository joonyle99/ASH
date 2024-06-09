public sealed class Frog : MonsterBehavior
{
    protected override void Awake()
    {
        base.Awake();

        AnimTransitionEvent -= GroundMoveToOtherCondition;  // Prevent multiple subscriptions (Handler)
        AnimTransitionEvent += GroundMoveToOtherCondition;
    }
    public void FixedUpdate()
    {
        if (IsDead)
            return;

        if (CurrentStateIs<Monster_IdleState>())
        {
            if (GroundMovementModule)
                GroundMovementModule.AffectGravity();
        }
    }

    public override IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        base.OnHit(attackInfo);

        // 선공 당하면 공격 모드 진입
        if (!AttackEvaluator.IsUsable)
            AttackEvaluator.IsUsable = true;

        // 선공 당하면 추격 모드 진입
        if (!GroundChaseEvaluator.IsUsable)
            GroundChaseEvaluator.IsUsable = true;

        return IAttackListener.AttackResult.Success;
    }
    private bool GroundMoveToOtherCondition(string targetTransitionParam, Monster_StateBase currentState)
    {
        if (currentState is GroundMoveState)
        {
            // Idle, Attack 상태로 전이할 때는 땅에 착지한 후 전이
            if (targetTransitionParam is "Idle" or "Attack")
            {
                return IsGround;
            }
        }

        return true;
    }
}
