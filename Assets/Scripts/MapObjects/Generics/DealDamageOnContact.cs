using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class DealDamageOnContact : MonoBehaviour, ICollisionWithPlayerListener
{
    [SerializeField] protected float _damage = 1;
    [SerializeField] protected float _knockbackPower = 7;

    Rigidbody2D _rigidbody;

    protected virtual bool CanDealDamage(PlayerBehaviour player)
    {
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
        if (CanDealDamage(player))
        {
            Vector2 knockbackDir = player.transform.position.x > transform.position.x ? Vector2.right : Vector2.left;
            AttackInfo attackInfo = new AttackInfo(_damage, _knockbackPower * knockbackDir, AttackType.GimmickAttack);
            var result = player.OnHit(attackInfo);
        }
    }

}
