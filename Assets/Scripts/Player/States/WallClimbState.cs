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
        base.OnUpdate();

        // Move Up
        if (Player.RawInputs.Movement.y > 0)
        {
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

        // InAirState
        if(!Player.IsTouchedWall)
        {
            ChangeState<InAirState>();
            return;
        }

        // IdleState
        if (Player.IsGrounded)
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
