using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

// [RequireComponent(typeof(IAttackListener), typeof(Rigidbody2D))]
public class AttackableEntity : MonoBehaviour
{
    [SerializeField] bool _allowsBasicAttack = true;

    IAttackListener [] _attackListeners;

    private void Awake()
    {
        _attackListeners = GetComponents<IAttackListener>();
    }

    public void OnHittedByBasicAttack()
    {
        if (!_allowsBasicAttack)
            return;

        foreach(var listener in  _attackListeners)
            listener.OnHitted(true);
    }

}
