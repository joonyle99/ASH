using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMahineDemo
{

    public class WalkState : PlayerState
    {
        [SerializeField] float moveSpeed;
        [SerializeField] float moveRange;

        protected override void OnEnter()
        {
            Debug.Log("Start Walk");
        }
        protected override void OnUpdate()
        {
            Debug.Log("Update Walk");

            if (Input.GetKeyDown(KeyCode.A))
            {
                ChangeState<AttackState>();
            }
            if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            {
                ChangeState<IdleState>();
            }
        }

        protected override void OnExit()
        {
            Debug.Log("Exit Walk");
        }

    }
}