using UnityEngine;

public class SemiBoss_RageState : SemiBoss_StateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

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

        // ������ �ݳ��ϸ� �̵��ӵ� / ���ݼӵ� ����
        SemiBoss.MoveSpeed *= 1.5f;
        SemiBoss.AttackEvaluator.TargetCheckCoolTime /= 1.5f;
    }
}
