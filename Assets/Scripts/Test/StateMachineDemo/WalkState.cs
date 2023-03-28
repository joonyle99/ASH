using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMahineDemo
{

    public class WalkState : PlayerState
    {
        PlayerInput playerInput;
        PlayerCollision playerCollision;

        #region Walking

        // 3. Walking
        [Header("Walking")]
        public float _walkSpeed = 7;
        public float _acceleration = 3f;
        public float _currentMovementLerpSpeed = 100f;

        private void HandleWalking()
        {
            // 마찰력 > 공기저항
            var acceleration = playerCollision.IsGrounded ? _acceleration : _acceleration * 0.5f;

            // left
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                // 빠른 방향전환
                if (Player.Rigidbody2D.velocity.x > 0)
                {
                    playerInput._inputs.X = 0;
                }

                // Smooth
                playerInput._inputs.X = Mathf.MoveTowards(playerInput._inputs.X, -1, acceleration * Time.deltaTime);
            }
            // right
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                // 빠른 방향전환
                if (Player.Rigidbody2D.velocity.x < 0)
                {
                    playerInput._inputs.X = 0;

                }
                // Smooth
                playerInput._inputs.X = Mathf.MoveTowards(playerInput._inputs.X, 1, acceleration * Time.deltaTime);
            }
            // none
            else
            {
                playerInput._inputs.X = Mathf.MoveTowards(playerInput._inputs.X, 0, acceleration * 2 * Time.deltaTime);
            }

            var idealVel = new Vector3(playerInput._inputs.X * _walkSpeed, Player.Rigidbody2D.velocity.y);

            // _currentMovementLerpSpeed should be set to something crazy high to be effectively instant. But slowed down after a wall jump and slowly released
            Player.Rigidbody2D.velocity = Vector3.MoveTowards(Player.Rigidbody2D.velocity, idealVel, _currentMovementLerpSpeed * Time.deltaTime);
        }

        #endregion

        protected override void OnEnter()
        {

            playerInput = GetComponent<PlayerInput>();
            playerCollision = GetComponent<PlayerCollision>();

            Debug.Log("Start Walk");

            Player.Animator.SetInteger("AnimState", 1);
        }
        protected override void OnUpdate()
        {
            //Debug.Log("Update Walk");

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