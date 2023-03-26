using System;
using Unity.VisualScripting.Antlr3.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using StateMahineDemo;
using Unity.Mathematics;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _anim;
    private Inputs _inputs;
    private PlayerState _playerState;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _groundMask = LayerMask.GetMask("Ground");
        _wallMask = LayerMask.GetMask("Wall");
    }

    void Update()
    {
        GatherInput();
        HandleGrounding();
        HandleWalking();
        HandleJumping();
        HandleDashing();
        HandleAnim();
    }

    #region Inputs

    [SerializeField] private bool _facingLeft;

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
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private LayerMask _wallMask;

    [SerializeField] private readonly float _collisionRadius = 0.2f;
    [SerializeField] private readonly Vector2 _bottomOffset = new Vector2(0f, 0.1f);
    [SerializeField] private readonly Vector2 _rightOffset = new Vector2(0.4f, 0.7f);
    [SerializeField] private readonly Vector2 _leftOffset = new Vector2(-0.4f, 0.7f);

    [SerializeField] private readonly Collider2D[] _ground = new Collider2D[1];
    [SerializeField] private readonly Collider2D[] _wall = new Collider2D[1];
    [SerializeField] private readonly Collider2D[] _leftWall = new Collider2D[1];
    [SerializeField] private readonly Collider2D[] _rightWall = new Collider2D[1];


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
                    || Physics2D.OverlapCircleNonAlloc((Vector2)transform.position + _rightOffset,_collisionRadius, _rightWall, _wallMask) > 0;
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
            _anim.SetBool("Grounded", true);
        }
        // OffGrounded
        else if (IsGrounded && !grounded) // jump timing
        {
            IsGrounded = false;
            _timeLeftGrounded = Time.time;
            _anim.SetBool("Grounded", false);
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

    #region Walking

    [Header("Walking")]
    [SerializeField] private float _walkSpeed = 7;
    [SerializeField] private float _acceleration = 3f;
    [SerializeField] private float _currentMovementLerpSpeed = 100f;

    private void HandleWalking()
    {
        if (_dashing) return;

        // 마찰력 > 공기저항
        var acceleration = IsGrounded ? _acceleration : _acceleration * 0.5f;

        // left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // 빠른 방향전환
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
            // 빠른 방향전환
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

    [Header("Jumping")]
    [SerializeField] private float _jumpForce = 7f;
    [SerializeField] private float _fallMultiplier = 3f;
    [SerializeField] private float _jumpVelocityFalloff = 6f;
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
                if (!_hasJumped || (_hasJumped && !_hasDoubleJumped)){ // 1단 or 2단 점프
                    ExecuteJump(new Vector2(_rb.velocity.x, _jumpForce), _hasJumped); // Ground jump (x에 _rb.velocity.x를 줌으로써 더 멀리 점프 가능)
                    _anim.SetTrigger("Jump");

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
    [SerializeField] private float _dashSpeed = 13f;
    [SerializeField] private float _dashLength = 0.2f;
    [SerializeField] private bool _hasDashed;
    [SerializeField] private bool _dashing;
    [SerializeField] private bool _useGravity;

    private float _timeStartedDash;
    private Vector2 _dashDir;

    private void HandleDashing()
    {
        // Dash
        if(Input.GetKeyDown(KeyCode.LeftShift) && !_hasDashed)
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
                _hasDashed = false;
                _useGravity = true;
                _rb.gravityScale = _useGravity ? 1 : 0;
                _rb.velocity = new Vector2(_rb.velocity.x, (_rb.velocity.y > 3) ? 3 : _rb.velocity.y);
            }
        }
    }

    #endregion

    #region Animation

    private void HandleAnim()
    {
        // Fall
        _anim.SetFloat("AirSpeedY", _rb.velocity.y);

        // Walk
        if ((_inputs.RawX != 0 && IsGrounded) && math.abs(_inputs.X) > 0)
        {
            _anim.SetInteger("AnimState", 1);
        }
        else
        {
            _anim.SetInteger("AnimState", 0);
        }
    }

    #endregion

    private struct Inputs
    {
        public float X, Y;
        public int RawX, RawY;
    }
}