using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMahineDemo
{

    public class WalkState : PlayerState
    {
        private void HandleWalking()
        {
            // ������ > ��������
            var acceleration = Player.IsGrounded ? Player._acceleration : Player._acceleration * 0.5f;

            // left
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                // ���� ������ȯ
                if (Player.Rigidbody2D.velocity.x > 0)
                {
                    Player._inputs.X = 0;
                }

                // Smooth
                Player._inputs.X = Mathf.MoveTowards(Player._inputs.X, -1, acceleration * Time.deltaTime);
            }
            // right
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                // ���� ������ȯ
                if (Player.Rigidbody2D.velocity.x < 0)
                {
                    Player._inputs.X = 0;
                }

                // Smooth
                Player._inputs.X = Mathf.MoveTowards(Player._inputs.X, 1, acceleration * Time.deltaTime);
            }
            // none
            else
            {
                Player._inputs.X = Mathf.MoveTowards(Player._inputs.X, 0, acceleration * 2 * Time.deltaTime);
            }

            var idealVel = new Vector3(Player._inputs.X * Player._walkSpeed, Player.Rigidbody2D.velocity.y);

            // _currentMovementLerpSpeed should be set to something crazy high to be effectively instant. But slowed down after a wall jump and slowly released
            Player.Rigidbody2D.velocity = Vector3.MoveTowards(Player.Rigidbody2D.velocity, idealVel, Player._currentMovementLerpSpeed * Time.deltaTime);
        }

        protected override void OnEnter()
        {
            Debug.Log("Start Walk");

            Player.Animator.SetInteger("AnimState", 1);
        }
        protected override void OnUpdate()
        {
            Debug.Log("Update Walk");

            HandleWalking();

            if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            {
                ChangeState<IdleState>();
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
            Debug.Log("Exit Walk");

            Player.Animator.SetInteger("AnimState", 0);
        }

    }
}