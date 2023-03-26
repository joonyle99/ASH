using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMahineDemo
{

    public class IdleState : PlayerState
    {
        protected override void OnEnter()
        {
            Debug.Log("Start Idle");
        }

        protected override void OnUpdate()
        {
            Debug.Log("Update Idle");

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                ChangeState<WalkState>();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ChangeState<JumpState>();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                ChangeState<AttackState>();
            }
        }

        protected override void OnExit()
        {
            Debug.Log("Exit Idle");
        }
    }
}