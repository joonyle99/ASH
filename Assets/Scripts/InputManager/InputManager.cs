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
    public event IInputSetter.InputEventHandler DashPressedEvent;

    public delegate void InputEventHandler();
    IInputSetter _currentSetter;
    IInputSetter _defaultSetter;

    InputState _cachedState;
    protected override void Awake()
    {
        base.Awake();
        _defaultSetter = GetComponent<PCInputSetter>();
    }
    public void ChangeToDefaultSetter()
    {
        ChangeInputSetter(_defaultSetter);
    }
    
    public void ChangeInputSetter(IInputSetter setter)
    {
        if (_currentSetter != null)
        {
            _currentSetter.JumpPressedEvent -= () => JumpPressedEvent?.Invoke();
            _currentSetter.DashPressedEvent -= () => DashPressedEvent?.Invoke();
        }
        _currentSetter = setter;

        _currentSetter.JumpPressedEvent += () => JumpPressedEvent?.Invoke();
        _currentSetter.DashPressedEvent += () => DashPressedEvent?.Invoke();
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
