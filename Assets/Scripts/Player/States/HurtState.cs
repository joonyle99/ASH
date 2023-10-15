using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtState : PlayerState
{
    private float time;

    protected override void OnEnter()
    {
        time = 0f;
        Player.PlaySound_SE_Hurt_02();
        Animator.SetTrigger("Hurt");
        Animator.SetBool("IsHurt", true);

        StopAllCoroutines();
    }

    protected override void OnUpdate()
    {
        if (Player.CurHP <= 0)
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
    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {
        Animator.SetBool("IsHurt", false);
    }
}