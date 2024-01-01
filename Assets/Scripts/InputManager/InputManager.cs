using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InputState
{
    public bool IsPressingJump;
    public Vector2 Movement;
    public float Vertical;
}

public class InputManager : HappyTools.SingletonBehaviourFixed<InputManager>
{
    [SerializeField] InputSetterScriptableObject _defaultInputSetter;
    [SerializeField] KeyCode _interactionKeyCode = KeyCode.E;

    public event IInputSetter.InputEventHandler JumpPressedEvent;
    public event IInputSetter.InputEventHandler DashPressedEvent;
    public event IInputSetter.InputEventHandler BasicAttackPressedEvent;
    public event IInputSetter.InputEventHandler ShootingAttackPressedEvent;

    public delegate void InputEventHandler();
    IInputSetter _currentSetter;

    InputState _cachedState;

    ActionKey _interactionKey;
    public ActionKey InteractionKey => _interactionKey;

    protected override void Awake()
    {
        base.Awake();
        _interactionKey = new ActionKey(_interactionKeyCode);
    }
    public void ChangeToDefaultSetter()
    {
        ChangeInputSetter(_defaultInputSetter);
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
        if (_currentSetter is InputSetterScriptableObject)
            (_currentSetter as InputSetterScriptableObject).Update();
        _cachedState = _currentSetter.GetState();
    }
    public InputState GetState()
    {
        return _cachedState;
    }
}
