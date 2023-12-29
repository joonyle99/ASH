using UnityEngine;

public class M_MoveState : MonsterState
{
    protected override void OnEnter()
    {

    }

    protected override void OnUpdate()
    {
        // µµÂø
        if (Monster.WayPointPatrol.IsArrived())
        {
            Monster.Rigidbody.velocity = Vector2.zero;

            Monster.WayPointPatrol.ChangeWayPoint();
            StartCoroutine(Monster.WayPointPatrol.WaitingTimer());

            ChangeState<M_IdleState>();
        }
        // ÀÌµ¿
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
