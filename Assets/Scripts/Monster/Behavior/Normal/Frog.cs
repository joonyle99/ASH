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

        // ���� ���ϸ� ���� ��� ����
        if (!AttackEvaluator.IsUsable)
            AttackEvaluator.IsUsable = true;

        // ���� ���ϸ� �߰� ��� ����
        if (!GroundChaseEvaluator.IsUsable)
            GroundChaseEvaluator.IsUsable = true;

        return IAttackListener.AttackResult.Success;
    }
    private bool GroundMoveToOtherCondition(string targetTransitionParam, Monster_StateBase currentState)
    {
        if (currentState is GroundMoveState)
        {
            // Idle, Attack ���·� ������ ���� ���� ������ �� ����
            if (targetTransitionParam is "Idle" or "Attack")
            {
                return IsGround;
            }
        }

        return true;
    }
}
