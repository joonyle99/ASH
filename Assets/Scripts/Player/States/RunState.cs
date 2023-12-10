using UnityEngine;
using UnityEngine.EventSystems;

public class RunState : PlayerState
{
    protected override void OnEnter()
    {
        Player.MovementController.enabled = true;
        Player.Animator.SetBool("IsRun", true);
    }

    protected override void OnUpdate()
    {
        if (!Player.IsMoveXKey)
        {
            ChangeState<IdleState>();
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Player.CanDash && Player.IsMoveXKey)
            {
                ChangeState<DashState>();
                return;
            }
        }

        if (Player.IsTouchedWall && Player.IsDirSync && Player.IsMoveUpKey)
        {
            ChangeState<WallGrabState>();
            return;
        }
    }
    protected override void OnExit()
    {
        Player.MovementController.enabled = false;
        Player.Animator.SetBool("IsRun", false);
    }

}