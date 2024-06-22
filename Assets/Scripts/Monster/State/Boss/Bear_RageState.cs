using UnityEngine;

public class Bear_RageState : Boss_StateBase
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        Boss.IsRage = true;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        // 보스가 격노하면 이동속도 / 공격속도 증가
        Boss.monsterData.MoveSpeed *= 1.5f;
        Boss.AttackEvaluator.TargetCheckCoolTime /= 1.5f;

        // 애니메이션 속도 증가
        animator.SetFloat("Mul_IdleSpeed", 1.3f);
        animator.SetFloat("Mul_MoveSpeed", 1.3f);
        animator.SetFloat("Mul_SlashSpeed", 1.3f);
        animator.SetFloat("Mul_EarthQuakeSpeed", 1.3f);
    }
}
