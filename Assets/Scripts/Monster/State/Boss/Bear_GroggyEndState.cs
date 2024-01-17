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

        var bear = Monster as Bear;
        if (bear != null)
        {
            bear.isGroggy = false;
            bear.IsGodMode = true;

            // 몬스터의 MonsterBodyHit Attack 기능을 켠다
            bear.SetIsAttackableHitBox(true);
        }
    }
}
