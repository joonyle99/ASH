using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerActivator : MonoBehaviour
{
    public bool IsPlayer { get; private set; }
    public PlayerBehaviour AsPlayer { get { return _playerComponent; } }

    PlayerBehaviour _playerComponent;
    private void Awake()
    {
        _playerComponent = GetComponent<PlayerBehaviour>();
        if (_playerComponent != null)
            IsPlayer = true;
        else
            IsPlayer = false;
    }
}
