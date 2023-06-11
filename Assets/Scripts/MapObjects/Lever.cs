using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : AttackableEntity
{
    [SerializeField] Animator _animator;
    [SerializeField] ToggleObject _targetObject;

    bool _isOn = false;

    protected override void OnHittedByBasicAttack(PlayerBehaviour player)
    {
        _isOn = !_isOn;
        _animator.SetBool("IsOn", _isOn);
        _targetObject.SetToggle(_isOn);
    }
}
