using UnityEngine;

public class JumpState : PlayerState
{
    PlayerJumpController _jumpController;

    protected override void OnEnter()
    {
        _jumpController = Player.GetComponent<PlayerJumpController>();

        Player.Animator.SetBool("IsJump", true);

        // Wall Jump
        if (Player.PreviousState is WallState)
            _jumpController.ExecuteWallJumpAnimEvent();
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