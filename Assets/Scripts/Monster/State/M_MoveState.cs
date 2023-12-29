using UnityEngine;

public class M_MoveState : MonsterState
{
    protected override void OnEnter()
    {

    }

    protected override void OnUpdate()
    {
        // 도착
        if (Monster.WayPointPatrol.IsArrived())
        {
            Monster.Rigidbody.velocity = Vector2.zero;

            // 다음 목표 지점으로
            Monster.WayPointPatrol.ChangeWayPoint();
            StartCoroutine(Monster.WayPointPatrol.WaitingTimer());

            ChangeState<M_IdleState>();
        }
        // 이동
        else
        {
            Monster.WayPointPatrol.SetMoveDir();
            Monster.Rigidbody.velocity = Monster.WayPointPatrol.MoveDir * Monster.MoveSpeed;
        }
    }

    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {

    }
}
