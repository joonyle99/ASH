using UnityEngine;

/// <summary>
/// 몬스터의 공통 MoveState
/// </summary>
public class ChaseState : Monster_StateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        // Not Found
        if (!Monster.ChaseEvaluator.IsTargetWithinChaseRange())
        {
            animator.SetTrigger("Patrol");
            return;
        }

        // Move to Target
        Monster.NavMeshMove.SetDestination(Monster.ChaseEvaluator.TargetTrans);
        Monster.NavMeshMove.MoveToTarget();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
