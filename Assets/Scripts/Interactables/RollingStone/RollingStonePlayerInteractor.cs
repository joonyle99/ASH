using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingStonePlayerInteractor : DealDamageOnContact
{
    PolygonCollider2D _collider;
    public PolygonCollider2D Collider { get { return _collider; } }
    public float Damage { get { return _damage; } set { _damage = value; } }
    public float ThreatVelocityThreshold { get { return _threatVelocityThreshold; } set { _threatVelocityThreshold = value; } }

    private void Awake()
    {
        _collider = GetComponent<PolygonCollider2D>();
    }
    private void FixedUpdate()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    protected override bool CanDealDamage(PlayerBehaviour player)
    {
        //TODO : 플레이어가 밀고있을 땐 false
        return true;
    }




}
