using UnityEngine;

public class JumpState : PlayerState
{
    PlayerJumpController _jumpController;

    protected override void OnEnter()
    {
        _jumpController = Player.GetComponent<PlayerJumpController>();

        Player.Animator.SetBool("IsJump", true);

        // Wall Jump
        if (Player.PreviousState is WallState && Player.RawInputs.IsPressingJump)
            _jumpController.ExecuteWallJump();
        // End Wall Jump
        else if(Player.PreviousState is WallState && !Player.RawInputs.IsPressingJump)
            _jumpController.ExcuteEndWallJump();
        // Jump
        else
            _jumpController.ExcuteBasicJump();
    }

    protected override void OnUpdate()
    {

    }
    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {

    }
}