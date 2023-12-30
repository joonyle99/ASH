using UnityEngine;

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
        if (Monster.WayPointPatrol.IsArrived())
        {
            Monster.RigidBody.velocity = Vector2.zero;

            // ���� ��ǥ ��������
            Monster.WayPointPatrol.ChangeWayPoint();
            Monster.WayPointPatrol.StartWaitingTimer();

            animator.SetTrigger("Idle");
        }
        // �̵�
        else
        {
            Monster.WayPointPatrol.SetMoveDir();
            Monster.RigidBody.velocity = Monster.WayPointPatrol.MoveDir * Monster.MoveSpeed;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
