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
                GetComponent<DashState>().SetDashDir(Player.RawInputs.Movement.x);
                ChangeState<DashState>();
                return;
            }
        }

        if (Player.IsTouchedWall)
        {
            if (Player.IsWallable)
            {
                if (Player.IsDirSync && Player.IsMoveUpKey)
                {
                    ChangeState<WallGrabState>();
                    return;
                }
            }
        }
    }
    protected override void OnExit()
    {
        Player.MovementController.enabled = false;
        Player.Animator.SetBool("IsRun", false);
    }

}