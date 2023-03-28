using StateMahineDemo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    WalkState walk;
    JumpState jump;
    Animator _anim;

    #region Detection

    [Header("Detection")]
    public LayerMask _groundMask;
    public LayerMask _wallMask;

    private float _collisionRadius = 0.2f;
    private Vector2 _bottomOffset = new Vector2(0f, 0.1f);
    private Vector2 _rightOffset = new Vector2(0.4f, 0.7f);
    private Vector2 _leftOffset = new Vector2(-0.4f, 0.7f);

    public Collider2D[] _ground = new Collider2D[1];
    public Collider2D[] _wall = new Collider2D[1];
    public Collider2D[] _leftWall = new Collider2D[1];
    public Collider2D[] _rightWall = new Collider2D[1];

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
            jump._hasJumped = false;
            jump._enableDoubleJump = true;
            jump._hasDoubleJumped = false;
            walk._currentMovementLerpSpeed = 100;
            _anim.SetBool("Grounded", true);


        }
        // OffGrounded
        else if (IsGrounded && !grounded) // jump timing
        {
            IsGrounded = false;
            jump._timeLeftGrounded = Time.time;
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

    // Start is called before the first frame update
    void Start()
    {
        walk = GetComponent<WalkState>();
        jump = GetComponent<JumpState>();
        _anim = GetComponent<Animator>();
        _groundMask = LayerMask.GetMask("Ground");
        _wallMask = LayerMask.GetMask("Wall");
    }

    // Update is called once per frame
    void Update()
    {
        HandleGrounding();
    }
}
