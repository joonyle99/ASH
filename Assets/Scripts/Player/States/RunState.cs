using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class RunState : PlayerState, IAttackableState, IJumpableState, IInteractableState
{
    protected override bool OnEnter()
    {
        Player.PlayerMovementController.enabled = true;
        Player.Animator.SetBool("IsRun", true);

        return true;
    }

    protected override bool OnUpdate()
    {
        if (!Player.IsMoveXKey)
        {
            ChangeState<IdleState>();
            return true;
        }

        if (InputManager.Instance.State.DashKey.KeyDown)
        {
            if (Player.CanDash && Player.IsMoveXKey)
            {
                GetComponent<DashState>().SetDashDir(Player.RawInputs.Movement.x);
                ChangeState<DashState>();
                return true;
            }
        }

        if (Player.IsTouchedWall)
        {
            if (Player.IsClimbable)
            {
                if (Player.IsDirSync && Player.IsMoveUpKey)
                {
                    ChangeState<WallGrabState>();
                    return true;
                }
            }
        }

        return true;
    }
    protected override bool OnExit()
    {
        Player.PlayerMovementController.enabled = false;
        Player.Animator.SetBool("IsRun", false);

        return true;
    }

}