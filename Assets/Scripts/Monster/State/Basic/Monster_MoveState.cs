using UnityEngine;

/// <summary>
/// 몬스터의 공통 MoveState
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

        // 도착
        if (Monster.FloatingPatrol.IsArrived())
        {
            Monster.RigidBody.velocity = Vector2.zero;

            // 다음 목표 지점으로
            Monster.FloatingPatrol.ChangeWayPoint();
            Monster.FloatingPatrol.StartWaitingTimer();

            animator.SetTrigger("Idle");
        }
        // 이동
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
