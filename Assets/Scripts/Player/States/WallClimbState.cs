using UnityEngine;

public class WallClimbState : WallState
{
    [Header("Wall Climb Setting")]

    [Space]

    [Range(0f, 20f)] [SerializeField] float _wallClimbSpeed = 4.0f;

    protected override void OnEnter()
    {
        base.OnEnter();

        Player.Rigidbody.gravityScale = 0f;

        Animator.SetBool("IsClimb", true);
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();

        // ���� �ö󰡱�
        if (Player.RawInputs.Movement.y > 0)
        {
            if (!Player.IsTouchedWall)
            {
                ChangeState<JumpState>();
                return;
            }

            Player.Rigidbody.velocity = moveDirection * _wallClimbSpeed;
        }
        // �Ʒ��� ��������
        else if (Player.RawInputs.Movement.y < 0)
        {
            if (!Player.IsTouchedWall)
            {
                ChangeState<InAirState>();
                return;
            }

            Player.Rigidbody.velocity = - moveDirection * _wallClimbSpeed;
        }
        // ������ ������ Wall Grab State�� ���� ����
        else
        {
            ChangeState<WallGrabState>();
            return;
        }

        // Idle State
        if (Player.IsGrounded && Player.RawInputs.Movement.y < 0)
        {
            ChangeState<IdleState>();
            return;
        }
    }

    protected override void OnExit()
    {
        Player.Rigidbody.gravityScale = 5f;

        Animator.SetBool("IsClimb", false);

        base.OnExit();
    }
}
