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

        if(Monster.FloatingChaseEvaluator)
        {
            if (!Monster.FloatingChaseEvaluator.IsUsable) return;

            // Target Within Range
            if (Monster.FloatingChaseEvaluator.IsTargetWithinRange())
            {
                if (Monster.FloatingMovementModule)
                {
                    // Move to Target for Chase
                    Monster.FloatingMovementModule.MoveToDestination(Monster.FloatingChaseEvaluator.TargetPosition);
                }

                return;
            }
        }

        if (Monster.FloatingPatrolModule)
        {
            // Patrol Point Update
            Monster.FloatingPatrolModule.UpdatePatrolPoint();

            if (Monster.FloatingMovementModule)
            {
                // Move to Target for Patrol
                Monster.FloatingMovementModule.MoveToDestination(Monster.FloatingPatrolModule.TargetPosition);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
