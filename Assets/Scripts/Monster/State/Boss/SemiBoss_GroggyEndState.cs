using UnityEngine;

public class SemiBoss_GroggyEndState : SemiBoss_StateBase
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

        SemiBoss.IsGroggy = false;
        SemiBoss.IsGodMode = true;

        // ������ MonsterBodyHit Attack ����� �Ҵ�
        SemiBoss.SetIsAttackableHitBox(true);
    }
}
