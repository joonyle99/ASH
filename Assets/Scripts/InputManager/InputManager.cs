using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InputState
{
    public bool IsPressingJump;
    public Vector2 Movement;
}
public class InputManager : HappyTools.SingletonBehaviourFixed<InputManager>, IInputSetter
{
    public event IInputSetter.InputEventHandler JumpPressedEvent;

    public delegate void InputEventHandler();
    IInputSetter _currentSetter;

    InputState _cachedState;
    
    public void ChangeInputSetter(IInputSetter setter)
    {
        if (_currentSetter != null)
        {
            _currentSetter.JumpPressedEvent -= () => JumpPressedEvent?.Invoke();
        }
        _currentSetter = setter;

        _currentSetter.JumpPressedEvent += () => JumpPressedEvent?.Invoke();
    }
    void Update()
    {
        _cachedState = _currentSetter.GetState();
    }
    public InputState GetState()
    {
        return _cachedState;
    }
}
