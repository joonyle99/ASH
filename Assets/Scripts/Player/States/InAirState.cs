using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InAirState : PlayerState
{
    [SerializeField] float _moveSpeed = 7f;
    [SerializeField] float _jumpVelocityFalloff = 7f;
    [SerializeField] float _coyoteTime = 0.2f;
    protected override void OnEnter()
    {
    }

    protected override void OnUpdate()
    {
        if (Player.Rigidbody.velocity.y < _jumpVelocityFalloff || !Player.RawInputs.IsPressingJump)
            Player.Rigidbody.gravityScale = 5;
        else
            Player.Rigidbody.gravityScale = 1;

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