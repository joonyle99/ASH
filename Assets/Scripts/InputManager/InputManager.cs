using System.Collections;
using UnityEngine;

public struct InputState
{
    public Vector2 Movement;
    public float Vertical;

    public ActionKey InteractionKey;
    public ActionKey JumpKey;
    public ActionKey DashKey;
    public ActionKey AttackKey;
    public ActionKey ShootingAttackKey;
    public ActionKey LightKey;
    public ActionKey EscapeKey;
}

public class InputManager : HappyTools.SingletonBehaviourFixed<InputManager>
{
    [SerializeField] private InputSetterScriptableObject _defaultInputSetter;

    [Space]

    [SerializeField] private InputSetterScriptableObject _moveRightInputSetter;
    [SerializeField] private InputSetterScriptableObject _moveLeftInputSetter;
    [SerializeField] private InputSetterScriptableObject _stayStillInputSetter;

    private IInputSetter _currentSetter;

    private InputState _cachedState;
    public InputState State => _cachedState;

    private Coroutine _inputUpdateCoroutine;

    private void Update()
    {
        if(_currentSetter == null)
        {
            Debug.Log($"_currentSetter is null");
            return;
        }

        _currentSetter.Update();
        _cachedState = _currentSetter.GetState();
    }

    public void ChangeToDefaultSetter()
    {
        if (_defaultInputSetter == null)
        {
            Debug.LogError("Default Input Setter is not set");
            return;
        }

        ChangeInputSetter(_defaultInputSetter);
    }
    public void ChangeToMoveRightSetter()
    {
        if (_moveRightInputSetter == null)
        {
            Debug.LogError("Move Right Input Setter is not set");
            return;
        }

        ChangeInputSetter(_moveRightInputSetter);
    }
    public void ChangeToMoveLeftSetter()
    {
        if (_moveLeftInputSetter == null)
        {
            Debug.LogError("Move Left Input Setter is not set");
            return;
        }
        ChangeInputSetter(_moveLeftInputSetter);
    }
    public void ChangeToStayStillSetter()
    {
        if (_stayStillInputSetter == null)
        {
            Debug.LogError("Stay Still Input Setter is not set");
            return;
        }
        ChangeInputSetter(_stayStillInputSetter);
    }
    public void ChangeInputSetter(IInputSetter setter)
    {
        if (setter == _currentSetter)
            return;

        _currentSetter = setter;
    }

    public IInputSetter GetCurrentInputSetter() => _currentSetter;
}
