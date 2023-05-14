using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterState : StateBase
{
    public MonsterBehaviour Monster { get { return StateMachine as MonsterBehaviour; } } // Monster State에서 Monster Behaviour 객체를 생성
}
