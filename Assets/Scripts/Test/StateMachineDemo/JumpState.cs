using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMahineDemo
{

    public class JumpState : PlayerState
    {
        PlayerCollision playerCollision;

        #region Jumping

        // 4. Jumping
        [Header("Jumping")]
        public float _jumpForce = 9f;
        public float _fallMultiplier = 4f;
        public float _jumpVelocityFalloff = 7f;
        public float _coyoteTime = 0.2f;
        public bool _hasJumped;
        public bool _enableDoubleJump = true;
        public bool _hasDoubleJumped;

        public float _timeLeftGrounded = -10;

        private void HandleJumping()
        {
            if (playerCollision.IsGrounded || (Time.time < _timeLeftGrounded + _coyoteTime) || (_enableDoubleJump && !_hasDoubleJumped))
            {
                if (!_hasJumped || (_hasJumped && !_hasDoubleJumped))
                {
                    // 1�� or 2�� ����
                    ExecuteJump(new Vector2(Player.Rigidbody2D.velocity.x, _jumpForce), _hasJumped); // Ground jump (x�� _rb.velocity.x�� �����ν� �� �ָ� ���� ����)

                    // _hasJumped�� false�� �� ���Դ�? -> 1�� ������ ����ȴٴ� ��
                    // _hasJumped�� true�� �� ���Դ�? -> 2�� ������ ����ȴٴ� ��
                }
            }

            // execute jump function
            void ExecuteJump(Vector2 dir, bool doubleJump = false)
            {
                Player.Rigidbody2D.velocity = dir;
                _hasDoubleJumped = doubleJump;
                _enableDoubleJump = !_hasDoubleJumped;
                _hasJumped = true;
            }
        }

        #endregion

        protected override void OnEnter()
        {
            playerCollision = GetComponent<PlayerCollision>();

            Debug.Log("Start Jump");

            Player.Animator.SetTrigger("Jump");

            HandleJumping();
        }

        protected override void OnUpdate()
        {
            Debug.Log("Update Jump");

            if (Input.GetKeyDown(KeyCode.Space) && _enableDoubleJump)
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