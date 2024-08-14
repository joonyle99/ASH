using UnityEngine;

public abstract class ToggleableObject : MonoBehaviour
{
    [Header("式式式式式式式式式 Toggle Object 式式式式式式式式式")]
    [Space]

    [SerializeField] protected bool _isOn = false;

    public bool IsOn { get => _isOn;}

    protected abstract void OnTurnedOn();
    protected abstract void OnTurnedOff();

    public void TurnOn()
    {
        if (IsOn)
            return;
        _isOn = true;
        OnTurnedOn();
    }
    public void TurnOff()
    {
        if (!IsOn)
            return;
        _isOn = false;
        OnTurnedOff();
    }

}
