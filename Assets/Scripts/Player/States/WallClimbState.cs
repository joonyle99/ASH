using UnityEngine;

public class WallClimbState : WallState
{
    [Header("Wall Climb State")]
    [Space]

    [Range(0f, 20f)] [SerializeField] float _wallClimbSpeed = 4.0f;

    private float prevGravity;

    protected override void OnEnter()
    {
        base.OnEnter();

        prevGravity = Player.Rigidbody.gravityScale;
        Player.Rigidbody.gravityScale = 0f;

        Animator.SetBool("IsClimb", true);
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();

        // Debug.Log("Climb");

        // 위로 올라가기
        if (Player.IsMoveUpKey)
        {
            // Wall End Jump
            if (!Player.IsTouchedWall)
            {
                Player.Rigidbody.velocity = Vector2.zero;
                ChangeState<JumpState>();
                return;
            }

            Player.Rigidbody.velocity = wallPerPendVec * _wallClimbSpeed;
        }
        // 아래로 내려가기
        else if (Player.IsMoveDownKey)
        {
            if (!Player.IsTouchedWall)
            {
                ChangeState<InAirState>();
                return;
            }

            Player.Rigidbody.velocity = - wallPerPendVec * _wallClimbSpeed;
        }
        // 가만히 있으면 Wall Grab State로 상태 전이
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
        Player.Rigidbody.gravityScale = prevGravity;

        Animator.SetBool("IsClimb", false);

        base.OnExit();
    }
}
