using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterState : StateBase
{
    public MonsterBehavior Monster { get { return StateMachine as MonsterBehavior; } } // Monster State���� Monster Behaviour ��ü�� ����
}
