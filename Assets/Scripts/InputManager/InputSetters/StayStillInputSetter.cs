using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stay Still InputSetter", menuName = "InputSetters/Stay Still")]
public class StayStillInputSetter : InputSetterScriptableObject
{
    public override InputState GetState()
    {
        InputState state = new InputState();
        state.IsPressingJump = false;
        state.Movement = Vector2.zero;
        return state;
    }

}
