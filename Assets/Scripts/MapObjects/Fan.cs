using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : ToggleObject
{
    [SerializeField] GameObject _windZone;
    protected override void OnToggleChanged()
    {
        _windZone.SetActive(IsOn);
    }
}
