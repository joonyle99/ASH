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
        //Player.Rigidbody2D.velocity = Vector3.MoveTowards(Player.Rigidbody2D.velocity, targetVelocity, _movementLerpSpeed * Time.deltaTime);
        // _currentMovementLerpSpeed should be set to something crazy high to be effectively instant. But slowed down after a wall jump and slowly released

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
