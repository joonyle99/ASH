using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtState : PlayerState
{
    protected override void OnEnter()
    {
        Animator.SetBool("IsHurt", true);
    }

    protected override void OnUpdate()
    {
        if (Player.IsGrounded)
        {
            ChangeState<IdleState>();
            return;
        }

        if (Player.CurHP <= 0)
        {
            Player.CurHP = 0;
            ChangeState<DieState>();
            return;
        }
    }

    protected override void OnExit()
    {
        Animator.SetBool("IsHurt", false);
    }
}