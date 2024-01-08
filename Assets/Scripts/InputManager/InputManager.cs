using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public struct InputState
{
    public Vector2 Movement;
    public float Vertical;

    public ActionKey InteractionKey;
    public ActionKey JumpKey;
    public ActionKey DashKey;
    public ActionKey BasicAttackKey;
    public ActionKey ShootingAttackKey;
    public ActionKey LightKey;
}

public class InputManager : HappyTools.SingletonBehaviourFixed<InputManager>
{
    [SerializeField] InputSetterScriptableObject _defaultInputSetter;

    IInputSetter _currentSetter;

    InputState _cachedState;
    public InputState State => _cachedState;

    protected override void Awake()
    {
        base.Awake();
    }
    public void ChangeToDefaultSetter()
    {
        ChangeInputSetter(_defaultInputSetter);
    }

    public void ChangeInputSetter(IInputSetter setter)
    {
        if (setter == _currentSetter)
            return;

        _currentSetter = setter;
    }
    void Update()
    {
        if (_currentSetter is InputSetterScriptableObject currentSetter)
            currentSetter.Update();
        _cachedState = _currentSetter.GetState();
    }
}
