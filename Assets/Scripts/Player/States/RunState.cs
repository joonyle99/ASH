using UnityEngine;

public class RunState : PlayerState, IAttackableState, IJumpableState, IInteractableState
{
    protected override bool OnEnter()
    {
        if (GameUIManager.Instance.OptionView.IsPause) return true;


        Player.PlayerMovementController.enabled = true;
        Player.Animator.SetBool("IsRun", true);

        return true;
    }

    protected override bool OnUpdate()
    {
        if (GameUIManager.Instance.OptionView.IsPause) return true;


        if (!Player.IsMoveXKey)
        {
            ChangeState<IdleState>();
            return true;
        }

        if (InputManager.Instance.State.DashKey.KeyDown)
        {
            if (Player.CanDash && Player.IsMoveXKey)
            {
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
        if (GameUIManager.Instance.OptionView.IsPause) return true;

        Player.PlayerMovementController.enabled = false;
        Player.Animator.SetBool("IsRun", false);

        return true;
    }

}