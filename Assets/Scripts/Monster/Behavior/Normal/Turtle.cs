public sealed class Turtle : MonsterBehavior
{
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
        // ** �ź��̴� Dead ���¿����� Hit�� �����ϴ� **

        // Turtle Hit Process
        if (IsHiding || IsDead)
            HitProcess(attackInfo, false, true, false);
        else
            HitProcess(attackInfo, false, true, true);

        // Change to Hurt State
        if (CurrentState is IHurtableState)
            SetAnimatorTrigger("Hurt");

        return IAttackListener.AttackResult.Success;
    }
    public override void Die(bool isHitBoxDisable = true, bool isDeathEffect = true)
    {
        base.Die(false, false);

        // Trigger -> Collision
        SetHitBoxStepable(true);
    }
}
