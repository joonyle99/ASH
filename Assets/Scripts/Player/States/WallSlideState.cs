using UnityEngine;

public class WallSlideState : WallState
{
    [Header("Wall Slide Setting")]
    [SerializeField] float _wallSlideSpeed = 0.65f;

    protected override void OnEnter()
    {
        base.OnEnter();

        Player.Rigidbody.gravityScale = 0f;

        Animator.SetBool("IsSlide", true);
    }

    protected override void OnUpdate()
    {
        // 벽면을 따라 서서히 땅에 떨어지는 기능
        Player.Rigidbody.velocity = (-1) * moveDirection * _wallSlideSpeed;

        // Wall Grab State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0)
        {
            ChangeState<WallGrabState>();
            return;
        }

        // Wall Climb State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.y) != 0)
        {
            ChangeState<WallClimbState>();
            return;
        }

        // In Air State로 가는 조건
        if (!Player.IsTouchedWall)
        {
            ChangeState<InAirState>();
            return;
        }

        // Idle State
        if (Player.IsGrounded)
        {
            ChangeState<IdleState>();
            return;
        }
    }

    protected override void OnExit()
    {
        Player.Rigidbody.gravityScale = 5f;

        Animator.SetBool("IsSlide", false);

        base.OnExit();
    }
}
