public sealed class Frog : MonsterBehaviour
{
    protected override void Awake()
    {
        base.Awake();

        AnimTransitionEvent -= HandleTransitionCondition;  // Prevent multiple subscriptions (Handler)
        AnimTransitionEvent += HandleTransitionCondition;
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
    private bool HandleTransitionCondition(string targetTransitionParam, Monster_StateBase currentState)
    {
        // Hurt �ִϸ��̼��� ��� ��Ȳ������ ��� ��ȯ
        if (targetTransitionParam is "Hurt") return true;

        // ���� ���� ������ �ִϸ��̼� ��ȯ�� �̷�
        return IsGround;
    }

    private void OnDestroy()
    {
        AnimTransitionEvent -= HandleTransitionCondition;
    }
}
