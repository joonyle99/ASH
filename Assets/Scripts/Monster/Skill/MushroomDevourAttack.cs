using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomDevourAttack : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _forceXPower;
    [SerializeField] private float _forceYPower;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerBehaviour player = collision.transform.GetComponent<PlayerBehaviour>();

        if (player != null)
        {
            if (player.IsGodMode || player.IsDead)
                return;

            float dir = Mathf.Sign(player.transform.position.x - transform.root.position.x);
            Vector2 forceVector = new Vector2(_forceXPower * dir, _forceYPower);

            player.OnHit(_damage, forceVector);
        }
    }
}
