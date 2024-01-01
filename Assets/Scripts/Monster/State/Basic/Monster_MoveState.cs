using UnityEngine;

/// <summary>
/// ������ ���� MoveState
/// </summary>
public class Monster_MoveState : Monster_StateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        // ����
        if (Monster.FloatingPatrol.IsArrived())
        {
            Monster.RigidBody.velocity = Vector2.zero;

            // ���� ��ǥ ��������
            Monster.FloatingPatrol.ChangeWayPoint();
            Monster.FloatingPatrol.StartWaitingTimer();

            animator.SetTrigger("Idle");
        }
        // �̵�
        else
        {
            Monster.FloatingPatrol.SetMoveDir();
            Monster.RigidBody.velocity = Monster.FloatingPatrol.MoveDir * Monster.MoveSpeed;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
