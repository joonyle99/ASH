using UnityEngine;

// TODO : Bear AttackState가 아닌 Boss AttackState로 일반화 하기
public class Bear_AttackState : Monster_AttackState
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        var bear = Monster as Bear;
        if (bear != null)
            bear.AttackPreProcess();
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
            bear.AttackPostProcess();
    }
}
