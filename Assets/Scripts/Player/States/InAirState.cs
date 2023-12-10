using UnityEngine;

public class InAirState : PlayerState
{
    [Header("InAir Setting")]

    [Space]

    [SerializeField] float _inAirMoveAcceleration = 30f;          // ���߿��� �¿�� �����̴� ���ǵ�
    [SerializeField] float _maxInAirMoveSpeed = 6f;         // ���߿��� �¿�� �����̴� �ִ� ���ǵ�
    [SerializeField] float _fastDropThreshhold = 4f;    // ���� �������� �����ϴ� ����
    [SerializeField] float _fastDropPower = 1.2f;        // ���� �������� ��
    [SerializeField] float _maxDropSpeed = 60f;        // �������� �ӵ� �ִ밪

    protected override void OnEnter()
    {

    }

    protected override void OnUpdate()
    {
        // Change to Idle State
        if (Player.IsGrounded)
        {
            // ���߿��� �ٴڿ� ������ ���� ����
            Player.PlaySound_SE_Jump_02();

            ChangeState<IdleState>();
            return;
        }

        // Change to Dash State
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Player.CanDash && Player.IsMoveXKey)
            {
                ChangeState<DashState>();
                return;
            }
        }

        // Change to Wall Grab State
        if (Player.IsTouchedWall && Player.IsDirSync)
        {
            ChangeState<WallGrabState>();
            return;
        }

        /*
        // Change to Dive State
        //if (Input.GetKeyDown(KeyCode.D) && Player.RawInputs.Movement.y < 0)
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (Player.GroundDistance > Player.DiveThreshholdHeight)
            {
                ChangeState<DiveState>();
                return;
            }
        }
        */

        // Wall Jump���� In Air State�� �Ѿ�� ���
        if (Player.IsWallJump)
        {
            // �÷��̾��� velocity�� �״�� ������ ����
            Player.Rigidbody.velocity = new Vector2(Player.Rigidbody.velocity.x, Player.Rigidbody.velocity.y);
            Player.IsWallJump = false;
        }
        // Basic Jump���� In Air State�� �Ѿ�� ���
        else
        {
            // ���߿����� �ִ� �̵��ӵ��� �����Ѵ�
            if (Mathf.Abs(Player.Rigidbody.velocity.x) > _maxInAirMoveSpeed)
                Player.Rigidbody.velocity = new Vector2(Mathf.Sign(Player.Rigidbody.velocity.x) * _maxInAirMoveSpeed, Player.Rigidbody.velocity.y);
        }

        // �Ѱ��� ������ �� ���� ������
        if (Player.Rigidbody.velocity.y < _fastDropThreshhold)
        {
            // �������� �ӵ��� �ִ밪 �ο�
            if (Player.Rigidbody.velocity.y < (-1) * _maxDropSpeed)
                Player.Rigidbody.velocity = new Vector2(Player.Rigidbody.velocity.x, (-1) * _maxDropSpeed);
        }
    }
    protected override void OnFixedUpdate()
    {
        // Basic Jump���� In Air State�� �Ѿ�� ���
        if (!Player.IsWallJump)
        {
            // ���߿��� �¿�� ������ �� �ִ�.
            Player.Rigidbody.AddForce(Vector2.right * Player.RawInputs.Movement.x * _inAirMoveAcceleration);
        }

        // �Ѱ��� ������ �� ���� ������
        if (Player.Rigidbody.velocity.y < _fastDropThreshhold)
        {
            // ���� ����
            Player.Rigidbody.AddForce(_fastDropPower * Physics2D.gravity);
        }
    }

    protected override void OnExit()
    {
        Player.Animator.SetBool("IsJump", false);
    }
}