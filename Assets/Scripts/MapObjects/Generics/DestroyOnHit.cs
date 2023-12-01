using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnHit : MonoBehaviour, IAttackListener
{
    public void OnHitted(bool isBasicAttack)
    {
        Destroy(gameObject);
    }
}
