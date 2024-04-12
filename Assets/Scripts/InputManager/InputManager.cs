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
}

public class InputManager : HappyTools.SingletonBehaviourFixed<InputManager>
{
    [SerializeField] InputSetterScriptableObject _defaultInputSetter;

    IInputSetter _currentSetter;

    InputState _cachedState;
    public InputState State => _cachedState;

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
