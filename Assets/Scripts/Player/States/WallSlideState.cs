using UnityEngine;
using UnityEngine.EventSystems;

public class WallSlideState : WallState
{
    [Header("Wall Slide Setting")]
    [SerializeField] private float _wallSlideSpeed = 0.5f;

    protected override void OnEnter()
    {
        base.OnEnter();

        Debug.Log("Enter Slide");

        Animator.SetBool("WallSlide", true);
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();

        // 서서히 땅에 떨어지는 기능
        Player.Rigidbody.velocity = - moveDirection * _wallSlideSpeed;

        // Wall Grab State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0 && Mathf.RoundToInt(Player.RawInputs.Movement.y) == 0)
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


        // InAirState
        if (!Player.IsTouchedWall)
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
        //Debug.Log("Exit WallSlide");

        Animator.SetBool("WallSlide", false);
    }
}
