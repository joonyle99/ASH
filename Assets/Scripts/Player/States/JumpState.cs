using UnityEngine;

public class JumpState : PlayerState
{
    PlayerJumpController _jumpController;

    protected override bool OnEnter()
    {
        _jumpController = Player.GetComponent<PlayerJumpController>();

        Player.Animator.SetTrigger("Jump");
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
        if (Player.IsUpWardGrounded)
        {
            Player.Animator.SetBool("IsJump", false);
            StateBase state = Player.PreviousState;

            if (state is IdleState)
            {
                Player.Animator.SetTrigger("ToIdle");
                ChangeState<IdleState>();
                return true;
            }
            else if (state is RunState)
            {
                Player.Animator.SetTrigger("ToRun");
                ChangeState<RunState>();
                return true;
            }

            return true;
        }

        return true;
    }
    protected override bool OnFixedUpdate()
    {
        return true;
    }

    protected override bool OnExit()
    {
        Player.Animator.ResetTrigger("Jump");
        Player.Animator.ResetTrigger("ToIdle");
        Player.Animator.ResetTrigger("ToRun");

        return true;
    }
}