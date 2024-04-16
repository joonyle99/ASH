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

        var chaseEvaluator = Monster.GroundChaseEvaluator;
        var patrolEvaluator = Monster.GroundPatrolEvaluator;

        if (chaseEvaluator?.IsUsable == true)
        {
            // �߰� ����� ���� �ȿ� �ִ��� Ȯ��
            var targetCollider = chaseEvaluator.IsTargetWithinRange();
            if (targetCollider)
            {
                // �߰��� ������ �Ÿ��� x���� ��������� �߰��� �ߴ�
                var distSquared = Vector3.SqrMagnitude(targetCollider.transform.position - Monster.transform.position);
                var maxDistSquared = chaseEvaluator.MaxChaseDistance * chaseEvaluator.MaxChaseDistance;
                var isTooClose = distSquared < maxDistSquared;

                // ���� ������ ���� ������Ʈ
                if (chaseEvaluator.IsTooClose != isTooClose)
                    chaseEvaluator.IsTooClose = isTooClose;

                // ������ ���� ��쿡�� ���� ��ȯ
                if (!isTooClose)
                {
                    // Monster.SetRecentDir(chaseEvaluator.ChaseDir);
                    Monster.StartSetRecentDirAfterGrounded(chaseEvaluator.ChaseDir);
                }

                // �߰� ������ �۵� �� Patrol ������� �۵����� �ʵ���
                return;
            }
        }
        
        if (patrolEvaluator?.IsUsable == true)
        {
            // ���� �ٱ��� �ִ� ���
            if (patrolEvaluator.IsOutOfPatrolRange())
            {
                // ���������� ����
                if (patrolEvaluator.IsLeftOfLeftPoint())
                {
                    // Monster.SetRecentDir(1);
                    Monster.StartSetRecentDirAfterGrounded(1);
                }
                // �������� ����
                else if (patrolEvaluator.IsRightOfRightPoint())
                {
                    // Monster.SetRecentDir(-1);
                    Monster.StartSetRecentDirAfterGrounded(-1);
                }
            }
            // ���� �ȿ� �ִ� ���
            else
            {
                // ���� �ȿ��� Patrol ���� ������ �ݴ� �������� ����
                if (patrolEvaluator.IsTargetWithinRange())
                {
                    // Monster.SetRecentDir(-Monster.RecentDir);
                    Monster.StartSetRecentDirAfterGrounded(-Monster.RecentDir);
                }
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
