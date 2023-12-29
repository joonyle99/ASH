using System.Collections;
using UnityEngine;

public class M_DieState : MonsterState
{
    protected override void OnEnter()
    {
        Animator.SetTrigger("Die");

        Monster.Die();
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {

    }
}