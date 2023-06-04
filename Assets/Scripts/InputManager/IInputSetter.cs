using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputSetter
{
    public delegate void InputEventHandler();
    public event InputEventHandler JumpPressedEvent;
    public event InputEventHandler DashPressedEvent;
    public event InputEventHandler BasicAttackPressedEvent;
    public event InputEventHandler HealingPressedEvent;
    public event InputEventHandler ShootingAttackPressedEvent;

    public InputState GetState();
}
