public class Frog : MonsterBehavior
{
    protected override void Awake()
    {
        base.Awake();

        customAnimTransitionEvent -= GroundMoveToOtherCondition;
        customAnimTransitionEvent += GroundMoveToOtherCondition;
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
            if (targetTransitionParam == "Idle" || targetTransitionParam == "Attack")
            {
                // ���� ������ �� ����
                return IsGround;
            }
        }

        return true;
    }
}
