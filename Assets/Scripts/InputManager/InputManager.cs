using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InputState
{
    bool IsPressingJump;
    Vector2 Movement;
}
public class InputManager : HappyTools.SingletonBehaviourFixed<InputManager>
{
    public delegate void InputEventHandler();

    public event InputEventHandler OnJump
    {
        add { _jumpEvent += value; }
        remove { _jumpEvent -= value; }
    }
    event InputEventHandler _jumpEvent;

    IInputSetter _currentSetter;

    public void SetInputSetter(IInputSetter setter)
    {
        _currentSetter = setter;
    }

    public InputState GetInputState()
    {
        return _currentSetter.GetState();
    }
}
