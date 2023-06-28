using UnityEngine;
using UnityEngine.EventSystems;

public class WallSlideState : WallState
{
    [Header("Wall Slide Setting")]
    [SerializeField] private float _wallSlideSpeed = 0.65f;

    protected override void OnEnter()
    {
        base.OnEnter();

        //Debug.Log("Enter Slide");

        Player.Rigidbody.gravityScale = 0f;
        Animator.SetBool("WallSlide", true);
    }
    protected override void OnUpdate()
    {
        // 憮憮�� 階縑 雲橫雖朝 晦棟
        Debug.Log("天天天天天天天天天天天天天天天天天天天" + moveDirection);
        Player.Rigidbody.velocity = (-1) * moveDirection * _wallSlideSpeed;

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


        // In Air State
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

        Player.Rigidbody.gravityScale = 5f;
        Animator.SetBool("WallSlide", false);
    }
}
