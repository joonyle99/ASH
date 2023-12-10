using UnityEngine;

public class WallClimbState : WallState
{
    [Header("Wall Climb State")]
    [Space]

    [Range(0f, 20f)] [SerializeField] float _wallClimbSpeed = 4.0f;

    private float _prevGravity;

    bool IsAboveWall { get { return Player.transform.position.y > wallHitPos.y; } }

    protected override void OnEnter()
    {
        _prevGravity = Player.Rigidbody.gravityScale;
        Player.Rigidbody.gravityScale = 0f;
        Player.Rigidbody.velocity = Vector2.zero;

        Animator.SetBool("IsClimb", true);
    }

    protected override void OnUpdate()
    {
        if (Player.IsMoveUpKey)
        {
            // Wall End Jump
            if (!Player.IsTouchedWall && IsAboveWall)
            {
                ChangeState<JumpState>();
                return;
            }

            if (Player.UpwardGroundHit)
                return;

            transform.position += Vector3.up * _wallClimbSpeed * Time.deltaTime;
        }
        else if (Player.IsMoveDownKey)
        {
            if (!Player.IsTouchedWall)
            {
                ChangeState<InAirState>();
                return;
            }

            transform.position -= Vector3.up * _wallClimbSpeed * Time.deltaTime;
        }
        else
        {
            ChangeState<WallGrabState>();
            return;
        }

        // Idle State
        if (Player.IsGrounded && Player.IsMoveDownKey)
        {
            ChangeState<IdleState>();
            return;
        }
    }

    protected override void OnExit()
    {
        Player.Rigidbody.gravityScale = _prevGravity;

        Animator.SetBool("IsClimb", false);
    }
}
