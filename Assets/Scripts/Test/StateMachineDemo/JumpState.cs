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
                    // 1�� or 2�� ����
                    ExecuteJump(new Vector2(Player.Rigidbody2D.velocity.x, Player._jumpForce), Player._hasJumped); // Ground jump (x�� _rb.velocity.x�� �����ν� �� �ָ� ���� ����)

                    // _hasJumped�� false�� �� ���Դ�? -> 1�� ������ ����ȴٴ� ��
                    // _hasJumped�� true�� �� ���Դ�? -> 2�� ������ ����ȴٴ� ��
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