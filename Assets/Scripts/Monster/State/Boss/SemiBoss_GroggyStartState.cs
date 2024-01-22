using UnityEngine;

public class SemiBoss_GroggyStartState : SemiBoss_StateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        // �׷α� ���� ����. ���̻� �������� ������ ���� ����
        SemiBoss.IsGroggy = true;

        // ������ MonsterBodyHit Attack ����� ����.
        SemiBoss.SetIsAttackableHitBox(false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
