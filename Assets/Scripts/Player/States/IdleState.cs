using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerState
{
    protected override void OnEnter()
    {
        Player.Animator.SetInteger("AnimState", 0);
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
    }
}