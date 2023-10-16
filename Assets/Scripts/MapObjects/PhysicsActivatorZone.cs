using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsActivatorZone : ITriggerZone
{
    [SerializeField] Rigidbody2D _targetRigidbody;
    [SerializeField] bool _freezeOnStart = true;
    private void Awake()
    {
        if (_freezeOnStart)
            _targetRigidbody.simulated = false;
    }
    public override void OnActivatorEnter(TriggerActivator activator) 
    {
        if(activator.IsPlayer)
        {
            _targetRigidbody.simulated = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + _targetRigidbody.transform.position);
    }
}
