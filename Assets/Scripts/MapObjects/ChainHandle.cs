using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainHandle : MonoBehaviour
{
    [SerializeField] Joint2D _jointWithPlayer;
    Rigidbody2D _rigidbody;
    SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void ConnectTo(Rigidbody2D target)
    {
        _spriteRenderer.enabled = false;
        _rigidbody.freezeRotation = true;
        _jointWithPlayer.connectedBody = target;
        _jointWithPlayer.enabled = true;
    }
    public void Disconnect()
    {
        _spriteRenderer.enabled = true;
        _rigidbody.freezeRotation = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = 0;
        _jointWithPlayer.enabled = false;
    }
}
