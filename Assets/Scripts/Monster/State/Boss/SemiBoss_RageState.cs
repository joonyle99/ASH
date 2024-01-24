using UnityEngine;

public class SemiBoss_RageState : SemiBoss_StateBase, IPassiveState
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        // TODO : ������ �ݳ��ϸ� �̵��ӵ� / ���ݼӵ� ����
        SemiBoss.MoveSpeed *= 2f;

        // ��ų ���� �ӵ� ����
        if(SemiBoss.AttackEvaluator)
            SemiBoss.AttackEvaluator.TargetCheckCoolTime /= 2f;
    }
}
