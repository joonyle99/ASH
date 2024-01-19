using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructOnHit : MonoBehaviour, IAttackListener
{
    public IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        Destruction.Destruct(gameObject);
        return IAttackListener.AttackResult.Success;
    }
}
