using UnityEngine;

public sealed class Frog : MonsterBehaviour
{
    protected override void Awake()
    {
        base.Awake();

        AnimTransitionEvent -= HandleGroundedTransition;  // Prevent multiple subscriptions (Handler)
        AnimTransitionEvent += HandleGroundedTransition;
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
    private bool HandleGroundedTransition(string targetTransitionParam, Monster_StateBase currentState)
    {
        // Hurt 애니메이션은 어떠한 상황에서도 즉시 전환
        if (targetTransitionParam is "Hurt") return true;

        // 땅에 닿을 때까지 애니메이션 전환을 미룸
        return IsGround;
    }

    private void OnDestroy()
    {
        AnimTransitionEvent -= HandleGroundedTransition;
    }

    public override void KnockBack(Vector2 forceVector)
    {
        RigidBody2D.velocity = Vector2.zero;

        base.KnockBack(forceVector);
    }
}
