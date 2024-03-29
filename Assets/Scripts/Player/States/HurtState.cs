using System.Collections;
using UnityEngine;

public class HurtState : PlayerState
{

    protected override bool OnEnter()
    {
        Animator.SetTrigger("Hurt");

        Player.IsHurt = true;

        Player.StartGodModeTimer();
        Player.StartWhiteFlash();
        // Player.StartWhiteFlash();

        return true;
    }

    protected override bool OnUpdate()
    {

        return true;
    }

    protected override bool OnFixedUpdate()
    {

        return true;
    }

    protected override bool OnExit()
    {
        Player.IsHurt = false;

        return true;
    }
}