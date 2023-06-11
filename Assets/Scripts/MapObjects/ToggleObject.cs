using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    public bool IsOn { get; private set; }

    public void SetToggle(bool value)
    {
        if(IsOn != value)
        {
            IsOn = value;
            OnToggleChanged();
        }
    }

    protected virtual void OnToggleChanged()
    {

    }
}
