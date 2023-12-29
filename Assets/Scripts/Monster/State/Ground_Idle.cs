using UnityEngine;

public class Ground_Idle : M_IdleState
{
    protected override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Ground_Idle Enter");
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    protected override void OnExit()
    {
        base.OnExit();
        Debug.Log("Ground_Idle Exit");
    }
}
