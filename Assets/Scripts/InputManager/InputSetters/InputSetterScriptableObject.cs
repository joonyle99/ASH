using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputSetterScriptableObject : ScriptableObject, IInputSetter
{
    public abstract event IInputSetter.InputEventHandler JumpPressedEvent;
    public abstract InputState GetState();
}
