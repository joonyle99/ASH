using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InputState
{
    public bool IsPressingJump;
    public Vector2 Movement;
}

public class InputManager : HappyTools.SingletonBehaviourFixed<InputManager>
{
    [SerializeField] KeyCode _interactionKey = KeyCode.E;

    public event IInputSetter.InputEventHandler JumpPressedEvent;
    public event IInputSetter.InputEventHandler DashPressedEvent;
    public event IInputSetter.InputEventHandler BasicAttackPressedEvent;
    public event IInputSetter.InputEventHandler ShootingAttackPressedEvent;

    public delegate void InputEventHandler();
    IInputSetter _currentSetter;
    IInputSetter _defaultSetter;

    InputState _cachedState;
    public bool IsInteractionDown { get { return Input.GetKeyDown(_interactionKey); } }
    public bool IsPressingInteraction { get { return Input.GetKey(_interactionKey); } }
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
        if (setter == _currentSetter)
            return;

        if (_currentSetter != null)
        {
            _currentSetter.JumpPressedEvent -= () => JumpPressedEvent?.Invoke();
            _currentSetter.DashPressedEvent -= () => DashPressedEvent?.Invoke();
            _currentSetter.BasicAttackPressedEvent -= () => BasicAttackPressedEvent?.Invoke();
            _currentSetter.ShootingAttackPressedEvent -= () => ShootingAttackPressedEvent?.Invoke();
        }
        _currentSetter = setter;

        _currentSetter.JumpPressedEvent += () => JumpPressedEvent?.Invoke();
        _currentSetter.DashPressedEvent += () => DashPressedEvent?.Invoke();
        _currentSetter.BasicAttackPressedEvent += () => BasicAttackPressedEvent?.Invoke();
        _currentSetter.ShootingAttackPressedEvent += () => ShootingAttackPressedEvent?.Invoke();
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
