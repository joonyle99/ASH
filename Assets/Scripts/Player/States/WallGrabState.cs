using UnityEngine;

/// <summary>
/// wall grab state�� ����
/// wall slide / wall climb state�� �� �� �ִ�
/// �� wall state �� ���� �⺻�� ����
/// </summary>
public class WallGrabState : WallState
{
    protected override void OnEnter()
    {
        base.OnEnter();

        Animator.SetBool("IsGrab", true);

        Player.Rigidbody.gravityScale = 0f;
        Player.Rigidbody.velocity = Vector2.zero;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        // Debug.Log("Grab");

        // Wall Climb State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.y) != 0)
        {
            ChangeState<WallClimbState>();
            return;
        }

        /*
        // Wall Slide State
        // ����Ű �Է� ������ ������
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) == 0 && Mathf.RoundToInt(Player.RawInputs.Movement.y) == 0)
        {
            ChangeState<WallSlideState>();
            return;
        }
        */
    }
    protected override void OnExit()
    {
        Animator.SetBool("IsGrab", false);

        Player.Rigidbody.gravityScale = 5f;

        base.OnExit();
    }
}
