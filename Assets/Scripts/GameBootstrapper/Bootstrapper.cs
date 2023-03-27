using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrapper : HappyTools.GameBootstrapper
{
    [SerializeField] InputManager _inputManager;
    [SerializeField] SceneTransitionManager _sceneTransitionManager;

    public override void InitGame()
    {
        Debug.Log("Game Initialization");
        _inputManager.ChangeInputSetter(_inputManager.GetComponent<PCInputSetter>());
        
    }

}
