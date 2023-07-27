using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtState : PlayerState
{
    private float time;

    protected override void OnEnter()
    {
        Animator.SetTrigger("Hurt");
        Animator.SetBool("IsHurt", true);
    }

    protected override void OnUpdate()
    {
        if (Player.CurHP < 0)
        {
            Player.CurHP = 0;
            ChangeState<DieState>();
            return;
        }
        else
        {
            time += Time.deltaTime;

            // Hurt State Á¾·á
            if (time > 0.7f)
            {
                time = 0f;
                ChangeState<IdleState>();
                return;
            }
        }
    }

    protected override void OnExit()
    {
        Animator.SetBool("IsHurt", false);
    }
}