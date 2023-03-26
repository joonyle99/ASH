using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMahineDemo
{

    public class JumpState : PlayerState
    {
        private void HandleJumping()
        {
            if (Player.IsGrounded || (Time.time < Player._timeLeftGrounded + Player._coyoteTime) || (Player._enableDoubleJump && !Player._hasDoubleJumped))
            {
                if (!Player._hasJumped || (Player._hasJumped && !Player._hasDoubleJumped))
                {
                    // 1단 or 2단 점프
                    ExecuteJump(new Vector2(Player.Rigidbody2D.velocity.x, Player._jumpForce), Player._hasJumped); // Ground jump (x에 _rb.velocity.x를 줌으로써 더 멀리 점프 가능)

                    // _hasJumped가 false일 때 들어왔다? -> 1단 점프가 실행된다는 뜻
                    // _hasJumped가 true일 때 들어왔다? -> 2단 점프가 실행된다는 뜻
                }
            }

            // execute jump function
            void ExecuteJump(Vector2 dir, bool doubleJump = false)
            {
                Player.Rigidbody2D.velocity = dir;
                Player._hasDoubleJumped = doubleJump;
                Player._enableDoubleJump = !Player._hasDoubleJumped;
                Player._hasJumped = true;
            }
        }

        protected override void OnEnter()
        {
            Debug.Log("Start Jump");
            Player.Animator.SetTrigger("Jump");

            HandleJumping();
        }

        protected override void OnUpdate()
        {
            Debug.Log("Update Jump");

            if (Input.GetKeyDown(KeyCode.Space) && Player._enableDoubleJump)
            {
                Debug.Log("Double Jump");
                ChangeState<JumpState>(true);
            }
            if(Player.Rigidbody2D.velocity.y < 0)
            {
                ChangeState<FallState>();
            }
        }
        protected override void OnExit()
        {
            Debug.Log("Exit Jump");
        }
    }
}