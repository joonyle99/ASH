using UnityEngine;

public class WallGrabState : WallState
{
    // wall grab state�� ����
    // wall slide / wall climb state�� �� �� �ִ�
    // �� wall state �� ���� �⺻�� ����

    protected override void OnEnter()
    {
        base.OnEnter();

        Animator.SetBool("Wall Grab", true);

        //Debug.Log("Enter Grab");

        // Player Stop
        Player.Rigidbody.gravityScale = 0f;
        Player.Rigidbody.velocity = Vector2.zero;
    }

    protected override void OnUpdate()
    {
        // Wall Climb State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.y) != 0)
        {
            ChangeState<WallClimbState>();
            return;
        }

        // Wall Slide State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) == 0 && Mathf.RoundToInt(Player.RawInputs.Movement.y) == 0)
        {
            ChangeState<WallSlideState>();
            return;
        }
    }
    protected override void OnExit()
    {
        base.OnExit();

        //Debug.Log("Exit Wall Grab");

        Animator.SetBool("Wall Grab", false);

        Player.Rigidbody.gravityScale = 5f;
    }
}
