using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class JumpState : PlayerState
{
    PlayerJumpController _jumpController;

    protected override void OnEnter()
    {
        _jumpController = Player.GetComponent<PlayerJumpController>();

        Player.Animator.SetBool("IsJump", true);

        // Wall Jump
        if (Player.PreviousState is WallState && Player.RawInputs.IsPressingJump)
            _jumpController.ExecuteWallJumpAnimEvent();
        // End Wall Jump
        else if(Player.PreviousState is WallState && !Player.RawInputs.IsPressingJump)
            _jumpController.ExcuteEndWallJumpAnimEvent();
        // Jump
        else
            _jumpController.ExecuteJumpAnimEvent();
    }

    protected override void OnUpdate()
    {

    }
    protected override void OnExit()
    {

    }
}