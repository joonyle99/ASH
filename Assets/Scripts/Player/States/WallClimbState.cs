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
        // 위로 올라가기
        if (Player.IsMoveUpKey)
        {
            // Wall End Jump
            if (!Player.IsTouchedWall && IsAboveWall)
            {
                ChangeState<JumpState>();
                return;
            }
            
            // 占쌈몌옙 占쏙옙占쏙옙 占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙 占싱듸옙占쏙옙 占쏙옙占쏙옙
            if (Player.UpwardGroundHit)
                return;

            transform.position += Vector3.up * _wallClimbSpeed * Time.deltaTime;
        }
        // 占싣뤄옙占쏙옙 占쏙옙占쏙옙占쏙옙占쏙옙
        else if (Player.IsMoveDownKey)
        {
            if (!Player.IsTouchedWall)
            {
                ChangeState<InAirState>();
                return;
            }

            transform.position -= Vector3.up * _wallClimbSpeed * Time.deltaTime;
        }
        // 占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙 Wall Grab State占쏙옙 占쏙옙占쏙옙 占쏙옙占쏙옙
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
