using UnityEngine;

public class NewMonster_IdleState : MonsterState
{
    protected override void OnEnter()
    {
        Debug.Log("Monster Idle OnEnter");
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {
        Debug.Log("Monster Idle OnExit");
    }
}
