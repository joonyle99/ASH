using UnityEngine;

public class GroundMoveState : Monster_StateBase, IAttackableState, IHurtableState, IMovingState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        var chaseEvaluator = Monster.GroundChaseEvaluator;          // 추격 판정기
        var patrolEvaluator = Monster.GroundPatrolEvaluator;        // 순찰 판정기

        // 추격 판정기가 사용 가능한 경우인지 확인한다
        if (chaseEvaluator?.IsUsable == true && !chaseEvaluator.IsDuringCoolTime)
        {
            Debug.Log("chase evaluator");

            // 추격 대상이 범위 안에 있는지 확인
            var targetCollider = chaseEvaluator.IsTargetWithinRange();

            // 추격 대상이 있는 경우
            if (targetCollider)
            {
                /*
                // 상대와의 거리가 x보다 가까워지면 추격을 중단
                var distSquared = Vector3.SqrMagnitude(targetCollider.transform.position - Monster.transform.position);
                var maxDistSquared = chaseEvaluator.MaxChaseDistance * chaseEvaluator.MaxChaseDistance;
                var isTooClose = distSquared < maxDistSquared;

                // 값이 변했을 때만 업데이트
                if (chaseEvaluator.IsTooClose != isTooClose)
                    chaseEvaluator.IsTooClose = isTooClose;
                */

                // 가깝지 않은 경우에만 방향 전환
                // if (!isTooClose)
                if (true)
                {
                    // 방향 전환
                    // Monster.SetRecentDir(chaseEvaluator.ChaseDir);
                    Monster.StartSetRecentDirAfterGrounded(chaseEvaluator.ChaseDir);

                    // TODO: 여기에서 Chase Evaluator의 기다린다
                    if (chaseEvaluator.UseWaitingEvent)
                    {
                        chaseEvaluator.StartWaitingEvent();
                    }
                    else
                    {
                        chaseEvaluator.StartCoolTime();
                    }
                }

                // 추격 판정기가 작동 중이면 순찰 판정기는 작동하지 않도록
                return;
            }
        }

        // 순찰 판정기가 사용 가능한 경우인지 확인한다
        if (patrolEvaluator?.IsUsable == true && !patrolEvaluator.IsDuringCoolTime)
        {
            // 범위 바깥에 있는 경우
            if (patrolEvaluator.IsOutOfPatrolRange())
            {
                // 오른쪽으로 간다
                if (patrolEvaluator.IsLeftOfLeftPoint())
                {
                    // Monster.SetRecentDir(1);
                    Monster.StartSetRecentDirAfterGrounded(1);
                }
                // 왼쪽으로 간다
                else if (patrolEvaluator.IsRightOfRightPoint())
                {
                    // Monster.SetRecentDir(-1);
                    Monster.StartSetRecentDirAfterGrounded(-1);
                }
            }
            // 범위 안에 있는 경우
            else
            {
                // 범위 안에서 Patrol 벽에 닿으면 반대 방향으로 간다
                if (patrolEvaluator.IsTargetWithinRange())
                {
                    // Monster.SetRecentDir(-Monster.RecentDir);
                    Monster.StartSetRecentDirAfterGrounded((-1) * Monster.RecentDir);
                }
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
