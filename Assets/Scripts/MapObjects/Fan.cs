using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : ToggleObject
{
    protected override void OnToggleChanged()
    {
        gameObject.SetActive(IsOn);
    }
}
