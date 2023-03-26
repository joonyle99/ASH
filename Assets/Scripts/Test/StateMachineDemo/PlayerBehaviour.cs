using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace StateMahineDemo
{

    public class PlayerBehaviour : StateMachineBase
    {
        public Inputs _inputs;

        #region Inputs

        [SerializeField] public bool _facingLeft;

        private void GatherInput()
        {
            _inputs.RawX = (int)Input.GetAxisRaw("Horizontal");
            _inputs.RawY = (int)Input.GetAxisRaw("Vertical");
            _inputs.X = Input.GetAxis("Horizontal");
            _inputs.Y = Input.GetAxis("Vertical");

            _facingLeft = _inputs.RawX != 1 && (_inputs.RawX == -1 || _facingLeft);
            SetFacingDirection(_facingLeft);
        }

        private void SetFacingDirection(bool left)
        {
            this.transform.localScale = new Vector3(left ? -1 : 1, transform.localScale.y, transform.localScale.z);
            //_anim.transform.rotation = left ? Quaternion.Euler(0, -90, 0) : Quaternion.Euler(0, 90, 0);
        }

        #endregion

        #region Detection

        [Header("Detection")]
        [SerializeField] public LayerMask _groundMask;
        [SerializeField] public LayerMask _wallMask;

        [SerializeField] public readonly float _collisionRadius = 0.2f;
        [SerializeField] public readonly Vector2 _bottomOffset = new Vector2(0f, 0.1f);
        [SerializeField] public readonly Vector2 _rightOffset = new Vector2(0.4f, 0.7f);
        [SerializeField] public readonly Vector2 _leftOffset = new Vector2(-0.4f, 0.7f);

        [SerializeField] public readonly Collider2D[] _ground = new Collider2D[1];
        [SerializeField] public readonly Collider2D[] _wall = new Collider2D[1];
        [SerializeField] public readonly Collider2D[] _leftWall = new Collider2D[1];
        [SerializeField] public readonly Collider2D[] _rightWall = new Collider2D[1];


        public bool IsGrounded;
        public bool OnWall;
        public bool OnLeftWall;
        public bool OnRightWall;

        private void HandleGrounding()
        {
            // Grounder
            bool grounded = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position + _bottomOffset,
                _collisionRadius, _ground, _groundMask) > 0;

            // Wall
            bool wall = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position + _leftOffset, _collisionRadius, _leftWall, _wallMask) > 0
                        || Physics2D.OverlapCircleNonAlloc((Vector2)transform.position + _rightOffset, _collisionRadius, _rightWall, _wallMask) > 0;
            bool leftWall = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position + _leftOffset,
                _collisionRadius, _leftWall, _wallMask) > 0;
            bool rightWall = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position + _rightOffset,
                _collisionRadius, _rightWall, _wallMask) > 0;

            // OnGrounded
            if (!IsGrounded && grounded) // land state
            {
                IsGrounded = true;
                _hasJumped = false;
                _enableDoubleJump = true;
                _hasDoubleJumped = false;
                _currentMovementLerpSpeed = 100;
                this.Animator.SetBool("Grounded", true);
            }
            // OffGrounded
            else if (IsGrounded && !grounded) // jump timing
            {
                IsGrounded = false;
                _timeLeftGrounded = Time.time;
                this.Animator.SetBool("Grounded", false);
            }

            // OnWall
            if (!OnWall && wall)
            {
                OnWall = true;

                if (!OnLeftWall && leftWall)
                {
                    OnLeftWall = true;
                }

                if (!OnRightWall && rightWall)
                {
                    OnRightWall = true;
                }
            }
            // OffWall
            else if (OnWall && !wall)
            {
                OnWall = false;

                if (OnLeftWall && !leftWall)
                {
                    OnLeftWall = false;
                }

                if (OnRightWall && !rightWall)
                {
                    OnRightWall = false;
                }
            }
        }

        private void DrawGrounderGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere((Vector2)transform.position + _bottomOffset, _collisionRadius);
            Gizmos.DrawWireSphere((Vector2)transform.position + _leftOffset, _collisionRadius);
            Gizmos.DrawWireSphere((Vector2)transform.position + _rightOffset, _collisionRadius);
        }

        private void OnDrawGizmos()
        {
            DrawGrounderGizmos();
        }

        #endregion

        // 3. Walking
        [Header("Walking")]
        [SerializeField] public float _walkSpeed = 7;
        [SerializeField] public float _acceleration = 3f;
        [SerializeField] public float _currentMovementLerpSpeed = 100f;

        // 4. Jumping
        [Header("Jumping")]
        [SerializeField] public float _jumpForce = 9f;
        [SerializeField] public float _fallMultiplier = 4f;
        [SerializeField] public float _jumpVelocityFalloff = 7f;
        [SerializeField] public float _coyoteTime = 0.2f;
        [SerializeField] public bool _hasJumped;
        [SerializeField] public bool _enableDoubleJump = true;
        [SerializeField] public bool _hasDoubleJumped;

        public float _timeLeftGrounded = -10;

        // 5. Dash
        [Header("Dash")]
        [SerializeField] public float _dashSpeed = 13f;
        [SerializeField] public float _dashLength = 0.2f;
        [SerializeField] public bool _hasDashed;
        [SerializeField] public bool _dashing;
        [SerializeField] public bool _useGravity;

        public float _timeStartedDash;
        public Vector2 _dashDir;

        // 6. Animation
        private void HandleAnim()
        {
            // Fall
            Animator.SetFloat("AirSpeedY", Rigidbody2D.velocity.y);
        }


        [SerializeField] int _attackPower;

        public int AttackPower { get { return _attackPower; } }

        protected override void Start()
        {
            base.Start();
            _groundMask = LayerMask.GetMask("Ground");
            _wallMask = LayerMask.GetMask("Wall");
        }

        protected override void Update()
        {
            base.Update();
            GatherInput();
            HandleGrounding();
            HandleAnim();

            // Fall faster and allow small jumps. _jumpVelocityFalloff is the point at which we start adding extra gravity. Using 0 causes floating
            // Light Jump & Full Jump
            if ((Rigidbody2D.velocity.y < _jumpVelocityFalloff) || ((Rigidbody2D.velocity.y > 0) && !Input.GetKey(KeyCode.Space)))
            {
                Rigidbody2D.velocity += _fallMultiplier * Physics2D.gravity.y * Vector2.up * Time.deltaTime;
            }
        }

        public struct Inputs
        {
            public float X, Y;
            public int RawX, RawY;
        }

    }
}
