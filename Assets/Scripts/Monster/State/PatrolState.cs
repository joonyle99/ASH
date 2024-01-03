using UnityEngine;

/// <summary>
/// 몬스터의 공통 IdleState
/// </summary>
public class PatrolState : Monster_StateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        Monster.PatrolEvaluator.SetTargetPos();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        // change to Chase
        if (Monster.ChaseEvaluator.IsTargetWithinChaseRange())
        {
            animator.SetTrigger("Chase");
            return;
        }

        // Patrol Point Update
        Monster.PatrolEvaluator.UpdatePatrolPoint();

        // Move to Target
        Monster.NavMeshMove.SetDestination(Monster.PatrolEvaluator.TargetPosition);
        Monster.NavMeshMove.MoveToTarget();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
