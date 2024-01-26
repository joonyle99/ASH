using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class DealDamageOnFastContact : MonoBehaviour, ICollisionWithPlayerListener
{
    [SerializeField] protected float _threatVelocityThreshold = 3;
    [SerializeField] protected float _damageToPlayer = 1;
    [SerializeField] protected float _knockbackPowerToPlayer = 1;
    [SerializeField] protected float _damageToMonster = 1;
    [SerializeField] protected float _knockbackPowerToMonster = 1;
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
            return;
        }
        MonsterBehavior monster = collision.transform.GetComponent<MonsterBehavior>();
        if (monster != null)
        {
            if (_rigidbody.velocity.sqrMagnitude > Mathf.Pow(_threatVelocityThreshold, 2))
            {
                Vector2 knockbackDir = monster.transform.position.x > transform.position.x ? Vector2.right : Vector2.left;
                AttackInfo attackInfo = new AttackInfo(_damageToMonster, _knockbackPowerToMonster * knockbackDir, AttackType.GimmickAttack);
                var result = monster.OnHit(attackInfo);
            }
        }

    }
    public void OnPlayerEnter(PlayerBehaviour player)
    {
        if (_rigidbody.velocity.sqrMagnitude > Mathf.Pow(_threatVelocityThreshold, 2) && CanDealDamage(player))
        {
            Vector2 knockbackDir = player.transform.position.x > transform.position.x ? Vector2.right : Vector2.left;
            AttackInfo attackInfo = new AttackInfo(_damageToPlayer, _knockbackPowerToPlayer * knockbackDir, AttackType.GimmickAttack);
            var result = player.OnHit(attackInfo);
        }
    }

}
