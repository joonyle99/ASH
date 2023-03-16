using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private struct Inputs
    {
        public float X, Y;
        public int RawX, RawY;
    }

    private Rigidbody2D _rb;
    private Inputs _inputs;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _groundMask = LayerMask.GetMask("Ground");
    }

    private void Update()
    {
        GatherInput();
        HandleGrounding();
        HandleWalking();
        HandleJumping();
        //Debug.Log("_rb.velocity.y : " + _rb.velocity.y + "_jumpVelocityFalloff : " + _jumpVelocityFalloff);
    }

    #region Inputs

    private void GatherInput()
    {
        _inputs.RawX = (int)Input.GetAxisRaw("Horizontal");
        _inputs.RawY = (int)Input.GetAxisRaw("Vertical");
        _inputs.X = Input.GetAxis("Horizontal");
        _inputs.Y = Input.GetAxis("Vertical");
    }

    #endregion

    #region Detection

    [Header("Detection")]
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _grounderOffset = -1, _grounderRadius = 0.2f;
    [SerializeField] private Collider2D[] _ground = new Collider2D[1];
    public bool IsGrounded;

    private void HandleGrounding()
    {
        // Grounder
        IsGrounded = (Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(0, _grounderOffset),
            _grounderRadius, _ground, _groundMask) > 0) ? true : false;

        // OnGrounded
        if (IsGrounded) // land state
        {
            _hasJumped = false;
            _hasDoubleJumped = false;
            _enableDoubleJump = true;
        }
        // OffGrounded
        else if (!IsGrounded) // Above land state
        {
            _timeLeftGrounded = Time.time;
        }
    }

    private void DrawGrounderGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, _grounderOffset), _grounderRadius);
    }

    private void OnDrawGizmos()
    {
        DrawGrounderGizmos();
    }

    #endregion

    #region Walking

    [Header("Walking")]
    [SerializeField] private float _walkSpeed = 10;
    [SerializeField] private float _acceleration = 2;
    [SerializeField] private float _currentMovementLerpSpeed = 100;

    private void HandleWalking()
    {
        // 공중에 떠 있을 때는 천천히 멈추고, 땅에 있으면 더 빨리 멈춘다 (마찰력 > 공기저항 고려)
        var acceleration = IsGrounded ? _acceleration : _acceleration * 0.5f;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            // 빠른 방향전환
            if (_rb.velocity.x > 0)
            {
                _inputs.X = 0;
            }

            // smoothing
            _inputs.X = Mathf.MoveTowards(_inputs.X, -1, acceleration * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            // 빠른 방향전환
            if (_rb.velocity.x < 0)
            {
                _inputs.X = 0;
            }

            // Smoothing
            _inputs.X = Mathf.MoveTowards(_inputs.X, 1, acceleration * Time.deltaTime);
        }
        else
        {
            _inputs.X = Mathf.MoveTowards(_inputs.X, 0, acceleration * 2 * Time.deltaTime);
        }

        var idealVel = new Vector2(_inputs.X * _walkSpeed, _rb.velocity.y);
        _rb.velocity = Vector2.MoveTowards(_rb.velocity, idealVel, _currentMovementLerpSpeed * Time.deltaTime);
    }

    #endregion

    #region Jumping

    [Header("Jumping")]
    [SerializeField] private float _jumpForce = 20;
    [SerializeField] private float _fallMultiplier = 50;
    [SerializeField] private float _jumpVelocityFalloff = 15;
    [SerializeField] private float _coyoteTime = 0.2f;
    [SerializeField] private bool _hasJumped;
    [SerializeField] private bool _enableDoubleJump;
    [SerializeField] private bool _hasDoubleJumped;

    private float _timeLeftGrounded;

    private void HandleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 3 situation jump
            if (IsGrounded || (Time.time <= _timeLeftGrounded + _coyoteTime) || (_enableDoubleJump && !_hasDoubleJumped))
            {
                // normal jump or double jump
                if (!_hasJumped || (_hasJumped && !_hasDoubleJumped))
                {
                    ExecuteJump(new Vector2(_rb.velocity.x, _jumpForce), _hasJumped);
                }
            }

            // Light Jump & Full Jump
            // Fall faster and allow small jumps. _jumpVelocityFalloff is the point at which we start adding extra gravity. Using 0 causes floating
            //if ((_rb.velocity.y < _jumpVelocityFalloff) || ((_rb.velocity.y > 0) && !Input.GetKey(KeyCode.Space)))
            //{
            //    _rb.velocity += _fallMultiplier * Physics2D.gravity.y * Vector2.up * Time.deltaTime;
            //}
        }

        // execute jump function
        void ExecuteJump(Vector2 dir, bool hasJump = false)
        {
            _rb.velocity = dir;
            _hasDoubleJumped = hasJump;
            _enableDoubleJump = !_hasDoubleJumped;
            _hasJumped = true;
        }

        if (_rb.velocity.y < _jumpVelocityFalloff)
        {
            Debug.Log("들어오나?");
            _rb.velocity += _fallMultiplier * Physics2D.gravity.y * Vector2.up * Time.deltaTime;
        }
    }



    #endregion

}
