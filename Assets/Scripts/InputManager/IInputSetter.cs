using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputSetter
{
    public delegate void InputEventHandler();
    public event InputEventHandler JumpPressedEvent;

    public InputState GetState();
}
