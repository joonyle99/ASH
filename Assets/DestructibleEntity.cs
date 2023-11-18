using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleEntity : MonoBehaviour
{
    IAttackListener _attackListener;

    private void Awake()
    {
        _attackListener = GetComponent<IAttackListener>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        RollingStone stone = collision.gameObject.GetComponent<RollingStone>();
        if (stone != null)
        {
            OnHittedByBasicAttack();
        }
    }

    void OnHittedByBasicAttack()
    {
        _attackListener.OnHitted(false);
    }
}
