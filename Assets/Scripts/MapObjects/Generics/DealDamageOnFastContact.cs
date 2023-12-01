using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class DealDamageOnFastContact : MonoBehaviour, ICollisionWithPlayerListener
{
    [SerializeField] protected float _threatVelocityThreshold = 3;
    [SerializeField] protected float _damage = 1;
    [SerializeField] InteractableObject _interactableComponent;

    Rigidbody2D _rigidbody;

    protected virtual bool CanDealDamage(PlayerBehaviour player)
    {
        if (_interactableComponent != null && _interactableComponent.IsInteracting)
            return false;
        return true;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerBehaviour player = collision.transform.GetComponent<PlayerBehaviour>();
        if (player != null)
        {
            OnPlayerEnter(player);
        }
    }
    public void OnPlayerEnter(PlayerBehaviour player)
    {
        if (_rigidbody.velocity.sqrMagnitude > Mathf.Pow(_threatVelocityThreshold, 2) && CanDealDamage(player))
            player.OnHitByPhysicalObject(_damage, _rigidbody);
    }

}
