using UnityEngine;

public class WallClimbState : WallState
{
    [Header("Wall Climb Setting")]

    [Space]

    [Range(0f, 20f)] [SerializeField] float _wallClimbSpeed = 4.0f;

    protected override void OnEnter()
    {
        base.OnEnter();

        Player.Rigidbody.gravityScale = 0f;

        Animator.SetBool("IsClimb", true);
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();

        // 위로 올라가기
        if (Player.RawInputs.Movement.y > 0)
        {
            if (!Player.IsTouchedWall)
            {
                ChangeState<JumpState>();
                return;
            }

            Player.Rigidbody.velocity = moveDirection * _wallClimbSpeed;
        }
        // 아래로 내려가기
        else if (Player.RawInputs.Movement.y < 0)
        {
            if (!Player.IsTouchedWall)
            {
                ChangeState<InAirState>();
                return;
            }

            Player.Rigidbody.velocity = - moveDirection * _wallClimbSpeed;
        }
        // 가만히 있으면 Wall Grab State로 상태 전이
        else
        {
            ChangeState<WallGrabState>();
            return;
        }

        // Idle State
        if (Player.IsGrounded && Player.RawInputs.Movement.y < 0)
        {
            ChangeState<IdleState>();
            return;
        }
    }

    protected override void OnExit()
    {
        Player.Rigidbody.gravityScale = 5f;

        Animator.SetBool("IsClimb", false);

        base.OnExit();
    }
}
