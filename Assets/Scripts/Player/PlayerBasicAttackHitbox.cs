using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicAttackHitbox : MonoBehaviour
{
    public PlayerBehaviour Player { get; private set; }
    private void Awake()
    {
        Player = GetComponentInParent<PlayerBehaviour>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OncologySlime slime = collision.GetComponent<OncologySlime>();

        if (slime != null)
        {
            float dir = Mathf.Sign(collision.transform.position.x - this.gameObject.transform.parent.position.x);
            Vector2 vec = new Vector2(10 * dir, 10 / 2f);

            slime.KnockBack(vec);
            slime.OnDamage(20);
        }
    }
}