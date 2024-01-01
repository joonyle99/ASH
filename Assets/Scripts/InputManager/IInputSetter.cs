using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputSetter
{
    public InputState GetState();
    public void Update();
}
