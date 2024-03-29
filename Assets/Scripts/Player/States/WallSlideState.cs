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
        // ������ ���� ������ ���� �������� ���
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

        // In Air State�� ���� ����
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
