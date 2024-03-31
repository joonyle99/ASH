using UnityEngine;

public class FloatingMoveState : Monster_StateBase, IAttackableState, IMovableState, IHurtableState
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if(Monster.FloatingChaseEvaluator)
        {
            if (!Monster.FloatingChaseEvaluator.IsUsable) return;

            // Target Within Range
            if (Monster.FloatingChaseEvaluator.IsTargetWithinRange())
            {
                // Move to Target for Chase
                Monster.NavMeshMoveModule.MoveToDestination(Monster.FloatingChaseEvaluator.TargetPosition);

                return;
            }
        }

        if (Monster.FloatingPatrolModule)
        {
            // Patrol Point Update
            Monster.FloatingPatrolModule.UpdatePatrolPoint();

            // Move to Target for Patrol
            Monster.NavMeshMoveModule.MoveToDestination(Monster.FloatingPatrolModule.TargetPosition);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
