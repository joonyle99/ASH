using UnityEngine;
using UnityEngine.EventSystems;

public class WallClimbState : WallState
{
    [Header("Wall Climb Setting")]
    [SerializeField] private float _wallClimbSpeed = 4.0f;

    protected override void OnEnter()
    {
        base.OnEnter();

        Debug.Log("Enter Climb");

        Player.Rigidbody.gravityScale = 0f;
        Animator.SetBool("WallClimb", true);
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
            // TODO : ���� ���� �����ϱ� => ���������� Wall State ���� ���� �����ϱ�
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
        //Debug.Log("Exit Wall Climb");

        Player.Rigidbody.gravityScale = 5f;
        Animator.SetBool("WallClimb", false);
    }
}
