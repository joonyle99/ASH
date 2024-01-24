using UnityEngine;

public class GroundMoveState : Monster_StateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (Monster.GroundPatrolEvaluator)
        {
            // reDirection for patrol

            // out patrol range
            if (Monster.GroundPatrolEvaluator.IsOutOfPatrolRange())
            {
                if (Monster.GroundPatrolEvaluator.IsLeftOfLeftPoint())
                    Monster.StartSetRecentDirAfterGrounded(1);
                else if (Monster.GroundPatrolEvaluator.IsRightOfRightPoint())
                    Monster.StartSetRecentDirAfterGrounded(-1);
            }
            // in patrol range
            else
            {
                if (Monster.GroundPatrolEvaluator.IsTargetWithinRange())
                    Monster.StartSetRecentDirAfterGrounded(-Monster.RecentDir);
            }
        }

        if (Monster.GroundChaseEvaluator)
        {
            // change state to chase
            if (Monster.GroundChaseEvaluator.IsTargetWithinRange())
                Monster.StartSetRecentDirAfterGrounded(Monster.GroundChaseEvaluator.ChaseDir);
        }

        // ground walking
        if (Monster.MoveType == MonsterDefine.MoveType.GroundWalking)
            Monster.GroundWalking();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
