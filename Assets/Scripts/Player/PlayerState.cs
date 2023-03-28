using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PlayerState : StateBase
{
    public PlayerBehaviour Player { get { return StateMachine as PlayerBehaviour; } }
}