using UnityEngine;

public class GroundMoveState : Monster_StateBase, IAttackableState, IMovableState, IHurtableState
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (Monster.GroundChaseEvaluator)
        {
            // first chase
            if (Monster.GroundChaseEvaluator.IsTargetWithinRange())
            {
                Monster.StartSetRecentDirAfterGrounded(Monster.GroundChaseEvaluator.ChaseDir);
                return;
            }
        }

        if (Monster.GroundPatrolEvaluator)
        {
            // then patrol
            if (Monster.GroundPatrolEvaluator.IsOutOfPatrolRange())
            {
                if (Monster.GroundPatrolEvaluator.IsLeftOfLeftPoint())
                    Monster.StartSetRecentDirAfterGrounded(1);
                else if (Monster.GroundPatrolEvaluator.IsRightOfRightPoint())
                    Monster.StartSetRecentDirAfterGrounded(-1);
            }
            else
            {
                if (Monster.GroundPatrolEvaluator.IsTargetWithinRange())
                    Monster.StartSetRecentDirAfterGrounded(-Monster.RecentDir);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
