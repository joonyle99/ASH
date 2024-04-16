using System.Diagnostics;
using Debug = UnityEngine.Debug;

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
        // ** 거북이는 Dead 상태에서도 Hit가 가능하다 **

        // 숨거나 사망 상태에서 피격 처리
        if (IsHiding || IsDead)
            HitProcess(attackInfo, false, true, false);
        // 일반적인 상태의 피격 처리
        else
            HitProcess(attackInfo, false, true, true);

        // Change to Hurt State
        if (CurrentState is IHurtableState)
            SetAnimatorTrigger("Hurt");

        return IAttackListener.AttackResult.Success;
    }
    public override void Die(bool isHitBoxDisable = true, bool isDeathProcess = true)
    {
        // 거북이는 히트박스 비활성화를 하지 않고, 사망 이펙트를 재생하지 않는다
        base.Die(false, false);

        // Trigger -> Collision
        SetHitBoxStepable(true);
    }
}
