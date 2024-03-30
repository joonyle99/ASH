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
                // �߰��� ������ �Ÿ��� x���� ��������� �߰��� �ߴ�
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
            // ���� �ٱ��� �ִ� ���
            if (Monster.GroundPatrolEvaluator.IsOutOfPatrolRange())
            {
                // ���������� ����
                if (Monster.GroundPatrolEvaluator.IsLeftOfLeftPoint())
                    Monster.StartSetRecentDirAfterGrounded(1);
                // �������� ����
                else if (Monster.GroundPatrolEvaluator.IsRightOfRightPoint())
                    Monster.StartSetRecentDirAfterGrounded(-1);
            }
            else
            {
                // ���� �ȿ��� ������ ���� ������ �ݴ� �������� ����
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
