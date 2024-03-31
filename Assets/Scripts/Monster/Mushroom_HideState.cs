using UnityEngine;

public class Mushroom_HideState : Monster_HideState
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if (Monster.AttackEvaluator.IsUsable)
            Monster.AttackEvaluator.IsUsable = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        if (!Monster.AttackEvaluator.IsUsable)
            Monster.AttackEvaluator.IsUsable = true;
    }
}
