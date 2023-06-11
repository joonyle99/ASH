using UnityEngine;

public class WallGrabState : WallState
{
    // wall grab state�� ����
    // wall slide / wall climb state�� �� �� �ִ�
    // �� wall state �� ���� �⺻�� ����

    protected override void OnEnter()
    {
        base.OnEnter();

        Debug.Log("enter grab");

        // Player Stop
        Player.Rigidbody.gravityScale = 0f;
        Player.Rigidbody.velocity = Vector2.zero;

        Animator.SetBool("WallGrab", true);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        // Wall Climb State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.y) != 0 )
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
        //Debug.Log("Exit Wall Grab");

        Player.Rigidbody.gravityScale = 5f;
        Animator.SetBool("WallGrab", false);
    }
}
