using UnityEngine;

public class WallClimbState : WallState
{
    [Header("Wall Climb Setting")]
    [SerializeField] private float _wallClimbSpeed = 4.0f;

    protected override void OnEnter()
    {
        base.OnEnter();

        //Debug.Log("Enter Climb");

        Player.Rigidbody.gravityScale = 0f;
        Animator.SetBool("Wall Climb", true);
    }
    protected override void OnUpdate()
    {
        // Move Up
        if (Player.RawInputs.Movement.y > 0)
        {
            // Debug.Log("=======================================" + moveDirection);
            Player.Rigidbody.velocity = moveDirection * _wallClimbSpeed;
        }
        // Move Down
        else if (Player.RawInputs.Movement.y < 0)
        {
            Player.Rigidbody.velocity = - moveDirection * _wallClimbSpeed;
        }
        // Move Stop => Wall Grab
        else
        {
            ChangeState<WallGrabState>();
            return;
        }

        if (!Player.IsTouchedWall)
        {
            // TODO : 여기 버그 개선하기 => 전반적으로 Wall State 관련 버그 개선하기
            ChangeState<InAirState>();
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
        base.OnExit();

        //Debug.Log("Exit Wall Climb");

        Player.Rigidbody.gravityScale = 5f;
        Animator.SetBool("Wall Climb", false);
    }
}
