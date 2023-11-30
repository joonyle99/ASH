using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputSetterScriptableObject : ScriptableObject, IInputSetter
{
#pragma warning disable CS0067 // Disable event never used
    public virtual event IInputSetter.InputEventHandler JumpPressedEvent;
    public virtual event IInputSetter.InputEventHandler DashPressedEvent;
    public virtual event IInputSetter.InputEventHandler BasicAttackPressedEvent;
    public virtual event IInputSetter.InputEventHandler ShootingAttackPressedEvent;
#pragma warning restore CS0067 // Disable event never used
    public abstract InputState GetState();
    public virtual void Update() { }
}
