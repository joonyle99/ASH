using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitVelocity : MonoBehaviour
{
    [SerializeField] float _maxVelocity;

    Rigidbody2D _rigidbody;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        if (_rigidbody.velocity.sqrMagnitude > _maxVelocity * _maxVelocity)
        {
            _rigidbody.velocity = new Vector2( _rigidbody.velocity.normalized.x * _maxVelocity, _rigidbody.velocity.y);
        }
    }
}
