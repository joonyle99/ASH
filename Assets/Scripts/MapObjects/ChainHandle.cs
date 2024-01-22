using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainHandle : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    SpriteRenderer _spriteRenderer;
    Rigidbody2D _playerHand;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void FixedUpdate()
    {
        if (_playerHand != null)
        {
            _rigidbody.MovePosition(_playerHand.position);
            _rigidbody.angularVelocity = 0;
        }
    }
    public void ConnectTo(Rigidbody2D target)
    {
        _spriteRenderer.enabled = false;
        _playerHand = target;
    }
    public void Disconnect()
    {
        _spriteRenderer.enabled = true;
        _playerHand = null;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = 0;

    }
}
