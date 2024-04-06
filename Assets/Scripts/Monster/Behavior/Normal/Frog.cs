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
            if (targetTransitionParam == "Idle" || targetTransitionParam == "Attack")
            {
                // 땅에 착지한 후 전이
                return IsGround;
            }
        }

        return true;
    }
}
