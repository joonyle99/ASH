using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructOnHit : MonoBehaviour, IAttackListener
{
    public void OnHitted(bool isBasicAttack)
    {
        Destruction.Destruct(gameObject);
    }
}
