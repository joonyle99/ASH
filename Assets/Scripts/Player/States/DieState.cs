using System.Collections;
using UnityEngine;

public class DieState : PlayerState
{
    protected override void OnEnter()
    {
        Animator.SetTrigger("Die");

        Player.IsDead = true;
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {
        Player.IsDead = false;
    }
}