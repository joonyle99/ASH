using UnityEngine;

public class Bear_GroggyEndState : Monster_StateBase
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

        Monster.IsGroggy = false;
        Monster.IsGodMode = true;

        // 몬스터의 MonsterBodyHit Attack 기능을 켠다
        Monster.SetIsAttackableHitBox(true);
    }
}
