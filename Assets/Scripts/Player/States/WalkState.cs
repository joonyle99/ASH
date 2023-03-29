using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WalkState : PlayerState
{
    [SerializeField] float _walkSpeed = 7;
    [SerializeField] float _acceleration = 3f;
    [SerializeField] float _movementLerpSpeed = 100f;

    protected override void OnEnter()
    {
        Player.Animator.SetInteger("AnimState", 1);
    }
    protected override void OnUpdate()
    {
        float xInput = Player.SmoothedInputs.Movement.x;
        Vector3 targetVelocity = new Vector3(xInput * _walkSpeed, Player.Rigidbody.velocity.y);
        Player.Rigidbody.velocity = targetVelocity;
        
        if (Player.RawInputs.Movement.x == 0)
        {
            ChangeState<IdleState>();
            return;
        }
    }

    protected override void OnExit()
    {
    }

}
