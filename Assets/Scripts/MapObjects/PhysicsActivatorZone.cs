using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsActivatorZone : TriggerZone
{
    [SerializeField] Rigidbody2D _targetRigidbody;
    [SerializeField] bool _freezeOnStart = true;
    private void Awake()
    {
        if (_freezeOnStart)
            _targetRigidbody.simulated = false;
    }
    public override void OnPlayerEnter(PlayerBehaviour player) 
    {
        _targetRigidbody.simulated = true;
    }

    private void OnDrawGizmos()
    {
        if (_targetRigidbody == null)
            return;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, _targetRigidbody.transform.position);
    }
}
