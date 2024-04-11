using UnityEngine;

public abstract class InputSetterScriptableObject : ScriptableObject, IInputSetter
{
    public abstract InputState GetState();
    public virtual void Update() { }
}
