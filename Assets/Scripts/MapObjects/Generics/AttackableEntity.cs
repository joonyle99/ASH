using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(IAttackListener), typeof(Rigidbody2D))]
public class AttackableEntity : MonoBehaviour
{
    [SerializeField] bool _allowsBasicAttack = true;

    IAttackListener _attackListener;
    private void Awake()
    {
        _attackListener = GetComponent<IAttackListener>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var basicAttackHitbox = collision.GetComponent<PlayerBasicAttackHitbox>();
        if (_allowsBasicAttack && basicAttackHitbox)
        {
            OnHittedByBasicAttack();
        }
    }

    void OnHittedByBasicAttack()
    {
        _attackListener.OnHitted(true);
    }

}
