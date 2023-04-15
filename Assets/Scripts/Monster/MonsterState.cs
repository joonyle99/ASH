using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterState : StateBase
{
    public MonsterBehaviour Monster { get { return StateMachine as MonsterBehaviour; } }
}
