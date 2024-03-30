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

        if (Monster.GroundChaseEvaluator && Monster.GroundChaseEvaluator.IsUsable && Monster.GroundChaseEvaluator.CanWorking)
        {
            var collider = Monster.GroundChaseEvaluator.IsTargetWithinRange();
            if (collider)
            {
                // 추가로 상대와의 거리가 x보다 가까워지면 추격을 중단
                var dist = Vector2.Distance(Monster.transform.position, collider.transform.position);
                if (dist < Monster.GroundChaseEvaluator.MaxChaseDistance)
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

        if (Monster.GroundPatrolEvaluator && Monster.GroundPatrolEvaluator.IsUsable && Monster.GroundPatrolEvaluator.CanWorking)
        {
            // 범위 바깥에 있는 경우
            if (Monster.GroundPatrolEvaluator.IsOutOfPatrolRange())
            {
                // 오른쪽으로 간다
                if (Monster.GroundPatrolEvaluator.IsLeftOfLeftPoint())
                    Monster.StartSetRecentDirAfterGrounded(1);
                // 왼쪽으로 간다
                else if (Monster.GroundPatrolEvaluator.IsRightOfRightPoint())
                    Monster.StartSetRecentDirAfterGrounded(-1);
            }
            else
            {
                // 범위 안에서 가상의 벽에 닿으면 반대 방향으로 간다
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
