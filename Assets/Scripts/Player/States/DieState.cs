using System.Collections;
using UnityEngine;

public class DieState : PlayerState
{
    protected override void OnEnter()
    {
        Player.IsDead = true;

        Animator.SetTrigger("Die");
        Animator.SetBool("IsDead", Player.IsDead);
    }

    protected override void OnUpdate()
    {
        if(Input.GetKeyDown(KeyCode.R))
            Player.InstantRespawn();
    }

    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {
        Player.IsDead = false;
        Animator.SetBool("IsDead", Player.IsDead);
    }
}