using UnityEngine;

public class Floating_Idle : M_IdleState
{
    protected override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Floating_Idle Enter");
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    protected override void OnExit()
    {
        base.OnExit();
        Debug.Log("Floating_Idle Exit");
    }
}
