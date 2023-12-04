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
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) == 0)
        {
            ChangeState<IdleState>();
            return;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Player.CanDash && Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0)
            {
                ChangeState<DashState>();
                return;
            }
        }
        if (Player.IsTouchedWall && Player.IsDirSync && Mathf.RoundToInt(Player.RawInputs.Movement.y) > 0)
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