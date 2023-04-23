using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesolateDiveState : PlayerState
{
    [SerializeField] float _diveSpeed =20.0f;
    [SerializeField] float _diveTime = 0.5f;
    [SerializeField] float _explosionRadius = 2.0f;
    [SerializeField] int _explosionDamage = 10;
    [SerializeField] float _minHeight = 5.0f;

    bool _isDiving = false;
    bool _isInvincible = false;

    protected override void OnEnter()
    {
        Debug.Log("Enter Desolate Dive");
    }

    protected override void OnUpdate()
    {
        Player.Rigidbody.velocity = new Vector2(0, -_diveSpeed);

        if(Player.IsGrounded)
                ChangeState<IdleState>();
    }
    protected override void OnExit()
    {
        Debug.Log("Exit Desolate Dive");
    }
}
