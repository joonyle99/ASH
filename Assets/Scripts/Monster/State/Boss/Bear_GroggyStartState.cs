using UnityEngine;

public class Bear_GroggyStartState : Monster_StateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        // 그로기 상태 진입. 더이상 손전등의 영향을 받지 않음
        Monster.IsGroggy = true;

        // 몬스터의 MonsterBodyHit Attack 기능을 끈다.
        Monster.SetIsAttackableHitBox(false);
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
