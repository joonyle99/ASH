using UnityEngine;

public class JumpState : PlayerState
{
    PlayerJumpController _jumpController;

    protected override bool OnEnter()
    {
        _jumpController = Player.GetComponent<PlayerJumpController>();

        Player.Animator.SetBool("IsJump", true);

        // Wall Jump
        if (Player.PreviousState is WallState && InputManager.Instance.State.JumpKey.Pressing) _jumpController.WallJump();
        // End Wall Jump
        else if (Player.PreviousState is WallState && !InputManager.Instance.State.JumpKey.Pressing) _jumpController.EndWallJump();
        // Jump
        else _jumpController.BasicJump();

        return true;
    }

    protected override bool OnUpdate()
    {
        return true;
    }
    protected override bool OnFixedUpdate()
    {
        return true;
    }

    protected override bool OnExit()
    {
        return true;
    }
}