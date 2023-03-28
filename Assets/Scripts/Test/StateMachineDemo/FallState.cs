using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMahineDemo
{

    public class FallState : PlayerState
    {

        JumpState jump;
        PlayerCollision playerCollision;

        protected override void OnEnter()
        {
            playerCollision = GetComponent<PlayerCollision>();
            jump = GetComponent<JumpState>();

            Debug.Log("Start Fall");
        }

        protected override void OnUpdate()
        {
            //Debug.Log("Update Fall");

            if (Input.GetKeyDown(KeyCode.Space) && jump._enableDoubleJump)
            {
                Debug.Log("Double Jump");
                ChangeState<JumpState>();
            }

            if (playerCollision.IsGrounded)
            {
                ChangeState<IdleState>();
            }
        }

        protected override void OnExit()
        {
            Debug.Log("Exit Fall");
        }
    }
}