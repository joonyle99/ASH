using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputSetterScriptableObject : ScriptableObject, IInputSetter
{
    public abstract InputState GetState();
    public virtual void Update() { }
}
