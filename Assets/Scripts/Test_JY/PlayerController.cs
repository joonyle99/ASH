using System;
using Unity.VisualScripting.Antlr3.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Inputs _inputs;
    private bool _useGravity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _groundMask = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        GatherInput();
        HandleGrounding();
        HandleWalking();
        HandleJumping();
        HandleDashing();
    }

    #region Inputs

    private bool _facingLeft;

    private void GatherInput()
    {
        _inputs.RawX = (int)Input.GetAxisRaw("Horizontal");
        _inputs.RawY = (int)Input.GetAxisRaw("Vertical");
        _inputs.X = Input.GetAxis("Horizontal");
        _inputs.Y = Input.GetAxis("Vertical");

        _facingLeft = _inputs.RawX != 1 && (_inputs.RawX == -1 || _facingLeft);
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
        bool grounded = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(0, _grounderOffset),
            _grounderRadius, _ground, _groundMask) > 0;

        // OnGrounded
        if (!IsGrounded && grounded) // land state
        {
            IsGrounded = true;
            _hasJumped = false;
            _enableDoubleJump = true;
            _hasDoubleJumped = false;
            _currentMovementLerpSpeed = 100;
        }
        // OffGrounded
        else if (IsGrounded && !grounded) // jump timing
        {
            IsGrounded = false;
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

    [Header("Walking")] [SerializeField] private float _walkSpeed = 10;
    [SerializeField] private float _acceleration = 3;
    [SerializeField] private float _currentMovementLerpSpeed = 100;

    private void HandleWalking()
    {
        if (_dashing) return;

        var acceleration = IsGrounded ? _acceleration : _acceleration * 0.5f;

        // left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (_rb.velocity.x > 0)
            {
                _inputs.X = 0;
            }

            // Smooth
            _inputs.X = Mathf.MoveTowards(_inputs.X, -1, acceleration * Time.deltaTime);
        }
        // right
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (_rb.velocity.x < 0)
            {
                _inputs.X = 0;
            }

            // Smooth
            _inputs.X = Mathf.MoveTowards(_inputs.X, 1, acceleration * Time.deltaTime);
        }
        // none
        else
        {
            _inputs.X = Mathf.MoveTowards(_inputs.X, 0, acceleration * 2 * Time.deltaTime);
        }

        var idealVel = new Vector3(_inputs.X * _walkSpeed, _rb.velocity.y);

        // _currentMovementLerpSpeed should be set to something crazy high to be effectively instant. But slowed down after a wall jump and slowly released
        _rb.velocity = Vector3.MoveTowards(_rb.velocity, idealVel, _currentMovementLerpSpeed * Time.deltaTime);
    }
    
    #endregion

    #region Jumping

    [Header("Jumping")] [SerializeField] private float _jumpForce = 15;
    [SerializeField] private float _fallMultiplier = 10;
    [SerializeField] private float _jumpVelocityFalloff = 14;
    [SerializeField] private float _coyoteTime = 0.2f;
    [SerializeField] private bool _hasJumped;
    [SerializeField] private bool _enableDoubleJump = true;
    [SerializeField] private bool _hasDoubleJumped;

    private float _timeLeftGrounded = -10;

    private void HandleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_dashing) return;

            if (IsGrounded || (Time.time < _timeLeftGrounded + _coyoteTime) || (_enableDoubleJump && !_hasDoubleJumped))
            {
                if (!_hasJumped || (_hasJumped && !_hasDoubleJumped)){
                    ExecuteJump(new Vector2(_rb.velocity.x, _jumpForce), _hasJumped); // Ground jump (x에 _rb.velocity.x를 줌으로써 더 멀리 점프 가능)
                    
                    // _hasJumped가 false일 때 들어왔다? -> 1단 점프가 실행된다는 뜻
                    // _hasJumped가 true일 때 들어왔다? -> 2단 점프가 실행된다는 뜻
                }
            }
        }

        // execute jump function
        void ExecuteJump(Vector2 dir, bool doubleJump = false)
        {
            _rb.velocity = dir;
            _hasDoubleJumped = doubleJump;
            _enableDoubleJump = !_hasDoubleJumped;
            _hasJumped = true;

        }

        // Fall faster and allow small jumps. _jumpVelocityFalloff is the point at which we start adding extra gravity. Using 0 causes floating
        // Light Jump & Full Jump
        if ((_rb.velocity.y < _jumpVelocityFalloff) || ((_rb.velocity.y > 0) && !Input.GetKey(KeyCode.Space)))
        {
            _rb.velocity += _fallMultiplier * Physics2D.gravity.y * Vector2.up * Time.deltaTime;
        }
    }

    #endregion

    #region Dashing

    [Header("Dash")]
    [SerializeField] private float _dashSpeed = 35.0f;
    [SerializeField] private float _dashLength = 0.22f;
    [SerializeField] private bool _hasDashed;
    [SerializeField] private bool _dashing;

    private float _timeStartedDash;
    private Vector2 _dashDir;

    private void HandleDashing()
    {
        // Dash
        if(Input.GetKeyDown(KeyCode.X) && !_hasDashed)
        {
            _dashDir = new Vector2(_inputs.RawX, _inputs.RawY).normalized; // 대쉬 방향
            if (_dashDir == Vector2.zero)
                _dashDir = _facingLeft ? Vector2.left : Vector2.right;
            _dashing = true;
            _hasDashed = true;
            _timeStartedDash = Time.time;
            _useGravity = false;
            _rb.gravityScale = _useGravity ? 1 : 0;
        }

        // Already Dash
        if(_dashing)
        {
            _rb.velocity = _dashDir * _dashSpeed;

            if(Time.time >= _timeStartedDash + _dashLength)
            {
                _dashing = false;
                // Clamp the velocity so they don't keep shooting off
                _rb.velocity = new Vector2(_rb.velocity.x, (_rb.velocity.y > 3) ? 3 : _rb.velocity.y);
                _useGravity = true;
                _rb.gravityScale = _useGravity ? 1 : 0;
                _hasDashed = false;
            }
        }
    }

    #endregion

    /*
    private void OnCollisionStay2D(Collision2D collision)
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (collision.gameObject.layer == groundLayer)
        {
            Debug.Log("Ground Stay");
            if(IsGrounded && _hasJumped)
            {
                Debug.Log("_hasJumped gonna false");
                _hasJumped = false;
            }
        }
    }
    */

    private struct Inputs
    {
        public float X, Y;
        public int RawX, RawY;
    }
}