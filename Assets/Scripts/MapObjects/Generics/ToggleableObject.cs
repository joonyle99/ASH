using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToggleableObject : MonoBehaviour
{
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
