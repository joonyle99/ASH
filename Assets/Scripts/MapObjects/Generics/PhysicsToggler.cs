using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsToggler : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rigidbody;
    [SerializeField] bool _disableOnAwake = true;
    [SerializeField] Vector2 _impulseForceOnEnable;

    private void Awake()
    {
        _rigidbody.simulated = !_disableOnAwake;
    }
    public void EnablePhysics()
    {
        _rigidbody.simulated = true;
        _rigidbody.AddForce(_impulseForceOnEnable, ForceMode2D.Impulse);
    }
    public void DisablePhysics()
    {
        _rigidbody.simulated = false;
    }
}
