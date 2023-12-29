using UnityEngine;

public abstract class MonsterState : StateBase
{
    // MonsterBehavior을 가져오는 프로퍼티
    public MonsterBehavior Monster
    {
        get { return StateMachine as MonsterBehavior; }
    }
}

/*
public abstract class MonsterState<T> : StateBase where T : MonsterBehavior
{
    // MonsterBehavior을 가져오는 프로퍼티
    public T Monster
    {
        get { return StateMachine as T; }
    }
}
*/