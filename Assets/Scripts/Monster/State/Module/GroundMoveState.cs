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
            var collider = Monster.GroundChaseEvaluator.IsTargetWithinRangePlus();
            if (collider)
            {
                // 추가로 상대와의 거리가 x보다 가까워지면 추격을 중단
                var dist = Vector2.Distance(Monster.transform.position, collider.transform.position);
                if (dist < 3f)
                {
                    Monster.GroundChaseEvaluator.IsTooClose = true;
                }
                else
                {
                    Monster.GroundChaseEvaluator.IsTooClose = false;
                    Monster.StartSetRecentDirAfterGrounded(Monster.GroundChaseEvaluator.ChaseDir);
                }
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
                if (Monster.GroundPatrolEvaluator.IsTargetWithinRangePlus())
                    Monster.StartSetRecentDirAfterGrounded(-Monster.RecentDir);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
