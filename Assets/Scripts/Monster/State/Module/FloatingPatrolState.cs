using System.Linq;
using UnityEngine;

public class FloatingPatrolState : Monster_MoveState
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        Monster.FloatingPatrolModule.SetTargetPos();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        // Patrol Point Update
        Monster.FloatingPatrolModule.UpdatePatrolPoint();

        // Move to Target
        Monster.NavMeshMove.SetDestination(Monster.FloatingPatrolModule.TargetPosition);
        Monster.NavMeshMove.MoveToDestination();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
