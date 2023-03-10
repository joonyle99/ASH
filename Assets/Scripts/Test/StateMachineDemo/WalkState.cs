using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMahineDemo
{

    public class WalkState : PlayerState
    {
        [SerializeField] float moveSpeed;
        [SerializeField] float moveRange;

        float eTime;
        protected override void OnEnter()
        {
            eTime = 0f;
        }
        protected override void OnUpdate()
        {
            eTime += Time.deltaTime;
            transform.position += new Vector3(0, Mathf.Sin(eTime * moveSpeed) * moveRange, 0) * Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.A))
                ChangeState<AttackState>();
            if (!Input.GetKey(KeyCode.W))
                ChangeState<IdleState>();
        }

        protected override void OnExit()
        {
        }

    }
}