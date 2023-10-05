using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrapper : HappyTools.GameBootstrapper
{
    [SerializeField] InputManager _inputManager;

    public override void InitGame()
    {
        _inputManager.ChangeToDefaultSetter();
        
    }

}
