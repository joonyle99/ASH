using UnityEngine;

public class BossGroggyRageState : Boss_StateBase
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        // ���� ����. �ǰݵ� �� ����.
        Boss.IsGodMode = true;

        // �ݳ� ���� ����
        Boss.IsRage = true;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        Boss.GroggyPostProcess();

        // ������ �ݳ��ϸ� �̵��ӵ� / ���ݼӵ� ����
        Boss.MoveSpeed *= 1.5f;
        Boss.AttackEvaluator.TargetCheckCoolTime /= 1.5f;

        // �ִϸ��̼� �ӵ� ����
        animator.SetFloat("Mul_IdleSpeed", 1.3f);
        animator.SetFloat("Mul_MoveSpeed", 1.3f);
        animator.SetFloat("Mul_SlashSpeed", 1.3f);
        animator.SetFloat("Mul_EarthQuakeSpeed", 1.3f);
    }
}
