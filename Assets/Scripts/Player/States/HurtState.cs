using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtState : PlayerState
{
    private float time;

    protected override void OnEnter()
    {
        Debug.Log("Enter Hurt");

        time = 0f;

        Animator.SetTrigger("Hurt");
        Animator.SetBool("IsHurt", true);
    }

    protected override void OnUpdate()
    {
        if (Player.CurHP < 0)
        {
            Player.CurHP = 0;
            ChangeState<DieState>();
        }
        else
        {
            time += Time.deltaTime;

            // Hurt State Á¾·á
            if (time > 0.2f)
            {
                time = 0f;
                ChangeState<IdleState>();
            }
        }
    }

    protected override void OnExit()
    {
        Debug.Log("Exit Hurt");

        Animator.SetBool("IsHurt", false);
    }
}