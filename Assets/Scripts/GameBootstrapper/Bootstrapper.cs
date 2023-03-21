using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrapper : HappyTools.GameBootstrapper
{
    [SerializeField] InputManager _inputManagerPrefab;

    InputManager _inputManager;
    public override void InitGame()
    {
        Debug.Log("Game Initialization");
        _inputManager = Instantiate(_inputManagerPrefab, transform);
        _inputManager.SetInputSetter(_inputManager.GetComponent<PCInputSetter>());
    }
}
