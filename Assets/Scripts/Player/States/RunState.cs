using UnityEngine;
using UnityEngine.EventSystems;

public class RunState : PlayerState
{
    protected override void OnEnter()
    {
        Player.PlayerMovementController.enabled = true;
        Player.Animator.SetBool("IsRun", true);
    }

    protected override void OnUpdate()
    {
        if (!Player.IsMoveXKey)
        {
            ChangeState<IdleState>();
            return;
        }

        if (InputManager.Instance.State.DashKey.KeyDown)
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
            if (Player.IsClimbable)
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
        Player.PlayerMovementController.enabled = false;
        Player.Animator.SetBool("IsRun", false);
    }

}