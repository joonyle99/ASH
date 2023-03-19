using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMahineDemo
{

    public class IdleState : PlayerState
    {
        protected override void OnEnter()
        {

        }
        protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.A))
                ChangeState<AttackState>();
            if (Input.GetKey(KeyCode.W))
                ChangeState<WalkState>();
        }


        protected override void OnExit()
        {

        }
    }
}