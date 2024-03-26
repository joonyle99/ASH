using UnityEngine;

public class WallSlideState : WallState
{
    [Header("Wall Slide Setting")]

    [Space]

    [Range(0f, 1f)] [SerializeField] float _wallSlideSpeed = 0.65f;

    protected override bool OnEnter()
    {
        base.OnEnter();

        Player.Rigidbody.gravityScale = 0f;

        Animator.SetBool("IsSlide", true);

        return true;
    }

    protected override bool OnUpdate()
    {
        // 벽면을 따라 서서히 땅에 떨어지는 기능
        Player.Rigidbody.velocity = (-1) * wallPerPendVec * _wallSlideSpeed;

        // Wall Grab State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0)
        {
            ChangeState<WallGrabState>();
            return true;
        }

        // Wall Climb State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.y) != 0)
        {
            ChangeState<WallClimbState>();
            return true;
        }

        // In Air State로 가는 조건
        if (!Player.IsTouchedWall)
        {
            ChangeState<InAirState>();
            return true;
        }

        // Idle State
        if (Player.IsGrounded)
        {
            ChangeState<IdleState>();
            return true;
        }

        return true;
    }

    protected override bool OnExit()
    {
        Player.Rigidbody.gravityScale = 5f;

        Animator.SetBool("IsSlide", false);

        base.OnExit();

        return true;
    }
}
