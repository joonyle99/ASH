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
    public InputSetterScriptableObject DefaultInputSetter => _defaultInputSetter;

    [Space]

    [SerializeField] private InputSetterScriptableObject _moveRightInputSetter;
    public InputSetterScriptableObject MoveRightInputSetter => _moveRightInputSetter;


    [SerializeField] private InputSetterScriptableObject _moveLeftInputSetter;
    public InputSetterScriptableObject MoveLeftInputSetter => _moveLeftInputSetter;


    [SerializeField] private InputSetterScriptableObject _stayStillInputSetter;
    public InputSetterScriptableObject StayStillInputSetter => _stayStillInputSetter;

    private IInputSetter _currentSetter;
    public IInputSetter CurrentSetter => _currentSetter;

    private InputState _cachedState;
    public InputState State => _cachedState;

    private void Update()
    {
        if(_currentSetter == null)
        {
            Debug.Log($"_currentSetter is null");
            return;
        }
        Debug.Log(_currentSetter.ToString());
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

        /*
        var setterObject = (object)setter;

        if (setterObject == (object)_defaultInputSetter)
        {
            Debug.Log("_defaultInputSetter");
        }
        else if (setterObject == (object)_moveRightInputSetter)
        {
            Debug.Log("_moveRightInputSetter");
        }
        else if (setterObject == (object)_moveLeftInputSetter)
        {
            Debug.Log("_moveLeftInputSetter");
        }
        else if (setterObject == (object)_stayStillInputSetter)
        {
            Debug.Log("_stayStillInputSetter");
        }
        */

        _currentSetter = setter;
    }
}
