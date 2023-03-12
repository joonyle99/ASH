using System;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    private Inputs _inputs;
    //private bool _userGravity;
    
    void Update()
    {
        HandleGrounding();
        HandleWalking();
        HandleJumping();
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

    [Header("Detection")] [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _grounderOffset = -1, _grounderRadius = 0.2f;
    public bool IsGrounded;
    public static event Action OnTouchedGround;

    private readonly Collider2D[] _ground = new Collider2D[1];

    private void HandleGrounding()
    {
        // Grounder
        var grounded = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(0, _grounderOffset),
            _grounderRadius, _ground, _groundMask) > 0;

        // OnGrounded
        if (!IsGrounded && grounded)
        {
            IsGrounded = true;
            _hasJumped = false;
            _currentMovementLerpSpeed = 100;
            OnTouchedGround?.Invoke();
            transform.SetParent(_ground[0].transform);

        }
        // OffGrounded
        else if (IsGrounded && !grounded)
        {
            IsGrounded = false;
            _timeLeftGrounded = Time.time;
            transform.SetParent(null);
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
        var acceleration = IsGrounded ? _acceleration : _acceleration * 0.5f;

        // left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (_rb.velocity.x > 0)
            {
                _inputs.X = 0;
            }

            _inputs.X = Mathf.MoveTowards(_inputs.X, -1, acceleration * Time.deltaTime);
        }
        // right
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (_rb.velocity.x < 0)
            {
                _inputs.X = 0;
            }

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

    [Header("Jumping")] [SerializeField] private float _jumpForce = 20;
    [SerializeField] private float _fallMultiplier = 10;
    [SerializeField] private float _jumpVelocityFalloff = 10;
    //[SerializeField] private Transform _jumpLaunchPoof;
    [SerializeField] private float _coyoteTime = 0.2f;
    [SerializeField] private bool _enableDoubleJump = true;

    private float _timeLeftGrounded = -10;
    private bool _hasJumped;
    private bool _hasDoubleJumped;

    private void HandleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsGrounded || (Time.time < _timeLeftGrounded + _coyoteTime) || (_enableDoubleJump && !_hasDoubleJumped))
            {
                if (!_hasJumped || (_hasJumped && !_hasDoubleJumped)){
                    ExecuteJump(new Vector2(_rb.velocity.x, _jumpForce), _hasJumped); // Ground jump
                }
            }
        }

        void ExecuteJump(Vector3 dir, bool doubleJump = false)
        {
            _rb.velocity = dir;
            _hasDoubleJumped = doubleJump;
            _hasJumped = true;

        }

        // Fall faster and allow small jumps. _jumpVelocityFalloff is the point at which we start adding extra gravity. Using 0 causes floating
            if ((_rb.velocity.y < _jumpVelocityFalloff) || ((_rb.velocity.y > 0) && !Input.GetKey(KeyCode.C)))
            _rb.velocity += _fallMultiplier * Physics2D.gravity.y * Vector2.up * Time.deltaTime;
    }

    #endregion

    private struct Inputs
    {
        public float X, Y;
        public int RawX, RawY;
    }
}
