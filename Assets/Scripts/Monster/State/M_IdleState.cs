using UnityEngine;

public class M_IdleState : MonsterState
{
    protected override void OnEnter()
    {

    }

    protected override void OnUpdate()
    {
        if (!Monster.WayPointPatrol.IsWaiting)
            ChangeState<M_MoveState>();
    }

    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {

    }
}