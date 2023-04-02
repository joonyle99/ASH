using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrapper : HappyTools.GameBootstrapper
{
    [SerializeField] InputManager _inputManager;
    [SerializeField] SceneManager _sceneTransitionManager;

    public override void InitGame()
    {
        _inputManager.ChangeToDefaultSetter();
        
    }

}
