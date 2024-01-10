using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructOnHit : MonoBehaviour, IAttackListener
{
    public void OnHit(AttackInfo attackInfo)
    {
        Destruction.Destruct(gameObject);
    }
}
