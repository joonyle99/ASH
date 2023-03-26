using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMahineDemo
{

    public class FallState : PlayerState
    {
        protected override void OnEnter()
        {
            Debug.Log("Start Fall");
        }

        protected override void OnUpdate()
        {
            Debug.Log("Update Fall");

            if (Input.GetKeyDown(KeyCode.Space) && Player._enableDoubleJump)
            {
                Debug.Log("Double Jump");
                ChangeState<JumpState>();
            }

            if (Player.IsGrounded)
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