using UnityEngine;

public class SemiBoss_GroggyRageState : SemiBoss_StateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        // ���� ����. �ǰݵ� �� ����.
        SemiBoss.IsGodMode = true;

        // �ݳ� ���� ����
        SemiBoss.IsRage = true;

        // TODO : �ܰ��� �� �ٲ� ���� �� ����.
        // like ���͸��� ����
        SemiBoss.StartFlashTimer(3f);
        SemiBoss.StartSuperArmorFlash();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        SemiBoss.GroggyPostProcess();

        // ������ �ݳ��ϸ� �̵��ӵ� / ���ݼӵ� ����
        SemiBoss.MoveSpeed *= 2f;
        SemiBoss.AttackEvaluator.TargetCheckCoolTime /= 2f;
    }
}
