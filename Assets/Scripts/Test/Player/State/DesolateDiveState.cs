using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesolateDiveState : PlayerState
{
    [SerializeField] float _diveSpeed = 10.0f;
    [SerializeField] float _diveTime = 0.5f;
    [SerializeField] float _explosionRadius = 2.0f;
    [SerializeField] int _explosionDamage = 10;
    [SerializeField] float _minHeight = 5.0f;

    bool _isDiving = false;
    bool _isInvincible = false;

    protected override void OnEnter()
    {

    }

    protected override void OnExit()
    {

    }

    protected override void OnUpdate()
    {

    }
}
