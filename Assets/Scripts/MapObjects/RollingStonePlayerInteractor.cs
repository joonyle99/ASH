using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingStonePlayerInteractor : DealDamageOnContact
{
    Rigidbody2D _rigidbody;
    Collider2D _collider;
    public Collider2D Collider { get { return _collider; } }
    public float Damage { get { return _damage; } set { _damage = value; } }
    public float ThreatVelocityThreshold { get { return _threatVelocityThreshold; } set { _threatVelocityThreshold = value; } }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }
    private void FixedUpdate()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    protected override bool CanDealDamage(PlayerBehaviour player)
    {
        return true;
    }




}
