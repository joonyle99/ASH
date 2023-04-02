using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InAirState : PlayerState
{
    [SerializeField] float _moveSpeed = 7f;
    [SerializeField] float _fastDropThreshhold = 7f;
    [SerializeField] float _fastDropPower = 7f;

    PlayerJumpController _jumpController;
    protected override void OnEnter()
    {
        //Debug.Log("InAir Enter");
        _jumpController = GetComponent<PlayerJumpController>();
    }

    protected override void OnUpdate()
    {
        //좌우 입력
        float xInput = Player.SmoothedInputs.Movement.x;
        Vector3 targetVelocity = new Vector3(xInput * _moveSpeed, Player.Rigidbody.velocity.y);
        Player.Rigidbody.velocity = targetVelocity;

        if (Player.IsGrounded)
        {
            ChangeState<IdleState>();
            return;
        }

        //떨어지는 속도 증가
        if (Player.Rigidbody.velocity.y < _fastDropThreshhold)
        {
            Player.Rigidbody.velocity += _fastDropPower * Physics2D.gravity * Time.deltaTime;
        }
    }

    protected override void OnExit()
    {
        //Debug.Log("InAir Exit");
    }
}