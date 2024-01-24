using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseForce : MonoBehaviour
{
    [SerializeField][Range(0f, 20f)] float _loseAmount;

    Rigidbody2D _rigidbody;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _rigidbody.velocity *= (1 - _loseAmount * Time.fixedDeltaTime);
        _rigidbody.angularVelocity *= (1 - _loseAmount * Time.fixedDeltaTime);
    }
}
