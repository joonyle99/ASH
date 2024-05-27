using UnityEngine;

public class FloatingMoveState : Monster_StateBase, IAttackableState, IHurtableState, IMovingState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        var floatingChaseEvaluator = Monster.FloatingChaseEvaluator;
        var floatingPatrolModule = Monster.FloatingPatrolModule;

        if (floatingChaseEvaluator)
        {
            if (floatingChaseEvaluator.IsUsable)
            {
                // Target Within Range
                if (floatingChaseEvaluator.IsTargetWithinRange())
                {
                    if (Monster.FloatingMovementModule)
                    {
                        // Move to Target for Chase
                        Monster.FloatingMovementModule.MoveToDestination(floatingChaseEvaluator.TargetPosition);

                        // Only Chase Run
                        return;
                    }
                }
            }
        }

        if (floatingPatrolModule)
        {
            if (Monster.FloatingMovementModule)
            {
                var isArrived = Monster.FloatingMovementModule.CheckArrivedToTarget();

                if (isArrived)
                {
                    Debug.Log("NavMesh Agent has arrived, Determine the next destination.");

                    // Set Random Target Position
                    floatingPatrolModule.SetRandomTargetPos();

                    // Move to Target for Patrol
                    Monster.FloatingMovementModule.MoveToDestination(floatingPatrolModule.TargetPosition);
                }
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
