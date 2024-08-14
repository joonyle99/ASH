using UnityEngine;

public abstract class InputSetterScriptableObject : ScriptableObject, IInputSetter
{
    [SerializeField] protected KeyCode escapeKeyCode = KeyCode.Escape;

    public virtual InputState GetState()
    {
        InputState state = new InputState();
        state.EscapeKey.Update(escapeKeyCode);
        return state;
    }
    public virtual void Update() { }
}
