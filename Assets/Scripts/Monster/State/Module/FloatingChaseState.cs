using System.Linq;
using UnityEngine;

public class FloatingChaseState : Monster_MoveState
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        // Move to Target
        Monster.NavMeshMove.SetDestination(Monster.FloatingChaseEvaluator.TargetTrans);
        Monster.NavMeshMove.MoveToDestination();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
