public sealed class Turtle : MonsterBehavior
{
    public void FixedUpdate()
    {
        if (IsDead)
            return;

        if (CurrentStateIs<GroundMoveState>())
        {
            if (MonsterMovementModule)
                MonsterMovementModule.GroundWalking();
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
            Animator.SetTrigger("Hurt");

        return IAttackListener.AttackResult.Success;
    }
    public override void Die(bool isDeathEffect = true)
    {
        base.Die(false);

        // Trigger -> Collision
        SetHitBoxStepable(true);
    }
}
