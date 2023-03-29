using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InAirState : PlayerState
{
    [SerializeField] float _moveSpeed = 7f;

    PlayerJumpController _jumpController;
    protected override void OnEnter()
    {
        _jumpController = GetComponent<PlayerJumpController>();
    }

    protected override void OnUpdate()
    {
        float xInput = Player.SmoothedInputs.Movement.x;
        Vector3 targetVelocity = new Vector3(xInput * _moveSpeed, Player.Rigidbody.velocity.y);
        Player.Rigidbody.velocity = targetVelocity;

        if (Player.IsGrounded)
        {
            ChangeState<IdleState>();
            return;
        }

    }

    protected override void OnExit()
    {
    }
}