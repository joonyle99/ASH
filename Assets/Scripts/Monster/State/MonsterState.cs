using UnityEngine;

public abstract class MonsterState : StateBase
{
    // MonsterBehavior�� �������� ������Ƽ
    public MonsterBehavior Monster
    {
        get { return StateMachine as MonsterBehavior; }
    }
}

/*
public abstract class MonsterState<T> : StateBase where T : MonsterBehavior
{
    // MonsterBehavior�� �������� ������Ƽ
    public T Monster
    {
        get { return StateMachine as T; }
    }
}
*/