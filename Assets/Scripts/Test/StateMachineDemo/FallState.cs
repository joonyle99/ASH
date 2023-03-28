using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMahineDemo
{

    public class FallState : PlayerState
    {
        PlayerCollision playerCollision;

        protected override void OnEnter()
        {
            playerCollision = GetComponent<PlayerCollision>();

            Debug.Log("Start Fall");
        }

        protected override void OnUpdate()
        {
            //Debug.Log("Update Fall");
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