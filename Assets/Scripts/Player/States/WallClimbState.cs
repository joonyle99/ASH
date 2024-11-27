using UnityEngine;

public class WallClimbState : WallState
{
    [Header("Wall Climb State")]
    [Space]

    [Range(0f, 20f)] [SerializeField] float _wallClimbSpeed = 4.0f;

    private float _prevGravity;

    bool IsAboveWall => Player.transform.position.y > wallHitPos.y;

    protected override bool OnEnter()
    {
        base.OnEnter();

        _prevGravity = Player.Rigidbody.gravityScale;
        Player.Rigidbody.gravityScale = 0f;
        Player.Rigidbody.velocity = Vector2.zero;

        Animator.SetBool("IsClimb", true);

        return true;
    }

    protected override bool OnUpdate()
    {
        if (Player.IsMoveUpKey)
        {
            // Wall End Jump
            if (!Player.IsTouchedWall && IsAboveWall)
            {
                Player.GetComponent<PlayerJumpController>().CastJump();
                return true;
            }

            if (Player.UpwardGroundHitForClimb)
                return false;

            transform.position += transform.up * _wallClimbSpeed * Time.deltaTime;
            // Player.Rigidbody.MovePosition(transform.position + transform.up * _wallClimbSpeed * Time.deltaTime);
        }
        else if (Player.IsMoveDownKey)
        {
            if (!Player.IsTouchedWall)
            {
                ChangeState<InAirState>();
                return true;
            }

            transform.position -= transform.up * _wallClimbSpeed * Time.deltaTime;
            // Player.Rigidbody.MovePosition(transform.position - transform.up * _wallClimbSpeed * Time.deltaTime);
        }
        else
        {
            ChangeState<WallGrabState>();
            return true;
        }

        // Idle State
        if (Player.IsGrounded && Player.IsMoveDownKey)
        {
            ChangeState<IdleState>();
            return true;
        }

        return true;
    }

    protected override bool OnExit()
    {
        Player.Rigidbody.gravityScale = _prevGravity;

        Animator.SetBool("IsClimb", false);

        base.OnExit();

        return true;
    }
}
