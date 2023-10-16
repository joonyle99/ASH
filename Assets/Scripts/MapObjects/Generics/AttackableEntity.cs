using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackableEntity : MonoBehaviour
{
    [SerializeField] bool _allowsBasicAttack;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var basicAttackHitbox = collision.GetComponent<PlayerBasicAttackHitbox>();
        if (_allowsBasicAttack && basicAttackHitbox)
        {
            OnHittedByBasicAttack(basicAttackHitbox.Player);
        }
    }

    virtual protected void OnHittedByBasicAttack(PlayerBehaviour player)
    {

    }

}
