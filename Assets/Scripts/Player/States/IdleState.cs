using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerState
{
    protected override void OnEnter()
    {
        //Debug.Log("Idle Enter");
    }

    protected override void OnUpdate()
    {
        if (Player.RawInputs.Movement.x != 0)
        {
            ChangeState<WalkState>();
            return;
        }
    }

    protected override void OnExit()
    {
        //Debug.Log("Idle Exit");
    }
}