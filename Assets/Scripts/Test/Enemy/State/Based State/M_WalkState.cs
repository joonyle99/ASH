using UnityEngine;

public class M_WalkState : MonsterState
{
    protected override void OnEnter()
    {
        Debug.Log("Enter M_Idle");
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnExit()
    {
        Debug.Log("Enter M_Idle");
    }
}
