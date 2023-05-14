using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Slime_Idle : M_IdleState
{
    protected override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Slime Idle");
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    protected override void OnExit()
    {
        base.OnExit();
    }
}
