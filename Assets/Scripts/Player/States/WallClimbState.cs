using UnityEngine;

public class WallClimbState : WallState
{
    [Header("Wall Climb Setting")]
    [SerializeField] float _wallClimbSpeed = 4.0f;

    protected override void OnEnter()
    {
        base.OnEnter();

        Player.Rigidbody.gravityScale = 0f;

        Animator.SetBool("IsClimb", true);
    }
    protected override void OnUpdate()
    {
        // ���� �ö󰡱�
        if (Player.RawInputs.Movement.y > 0)
        {
            Player.Rigidbody.velocity = moveDirection * _wallClimbSpeed;
        }
        // �Ʒ��� ��������
        else if (Player.RawInputs.Movement.y < 0)
        {
            Player.Rigidbody.velocity = - moveDirection * _wallClimbSpeed;
        }
        // ������ ������ Wall Grab State�� ���� ����
        else
        {
            ChangeState<WallGrabState>();
            return;
        }

        // TODO : ���� Ÿ�� �ö󰡴ٰ� ���� �������..? -> ��� ��������
        // 1. ���̻� �� �ö󰡰� �Ѵ� (Ray���̴� ���� ������ ������ ���� ����)
        // 2. InAirState�� �����Ѵ� (�׷� �� �ٽ� ������,, ������)
        if (!Player.IsTouchedWall)
        {
            ChangeState<InAirState>();
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
