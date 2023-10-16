using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] protected float _threatVelocityThreshold = 3;
    [SerializeField] protected float _damage = 1;

    [SerializeField] Rigidbody2D _rigidbody;

    protected virtual bool CanDealDamage(PlayerBehaviour player)
    {
        return true;
    }

    private void Awake()
    {
        if(_rigidbody == null )
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerBehaviour player = collision.transform.GetComponent<PlayerBehaviour>();
        if (player != null)
        {
            if (_rigidbody.velocity.sqrMagnitude >= Mathf.Pow(_threatVelocityThreshold, 2) && CanDealDamage(player))
                player.OnHitByPhysicalObject(_damage, collision);
        }
    }

}
