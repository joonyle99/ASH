using System.Collections;
using UnityEngine;

public class HurtState : PlayerState
{

    protected override void OnEnter()
    {
        Animator.SetTrigger("Hurt");

        Player.IsHurt = true;

        Player.StartGodMode();
        Player.StartWhiteFlash();
        // Player.StartWhiteFlash();
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {
        Player.IsHurt = false;
    }
}