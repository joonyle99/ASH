using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputSetterScriptableObject : ScriptableObject, IInputSetter
{
#pragma warning disable CS0067 // Disable event never used
    public event IInputSetter.InputEventHandler JumpPressedEvent;
    public event IInputSetter.InputEventHandler DashPressedEvent;
    public event IInputSetter.InputEventHandler BasicAttackPressedEvent;
    public event IInputSetter.InputEventHandler ShootingAttackPressedEvent;
#pragma warning restore CS0067 // Disable event never used
    public abstract InputState GetState();
}
