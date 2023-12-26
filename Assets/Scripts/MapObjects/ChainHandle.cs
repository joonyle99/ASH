using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainHandle : MonoBehaviour
{
    [SerializeField] Joint2D _jointWithPlayer;
    public void ConnectTo(Rigidbody2D target)
    {
        _jointWithPlayer.connectedBody = target;
        _jointWithPlayer.enabled = true;
    }
    public void Disconnect()
    {
        _jointWithPlayer.enabled = false;
    }
}
