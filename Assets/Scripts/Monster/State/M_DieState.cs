using System.Collections;
using UnityEngine;

public class M_DieState : MonsterState
{
    protected override void OnEnter()
    {
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