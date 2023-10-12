using UnityEngine;

public class InAirState : PlayerState
{
    [Header("InAir Setting")]

    [Space]

    [Range(0f, 100f)][SerializeField] float _inAirSpeed = 50f;          // ���߿��� �¿�� �����̴� ���ǵ�
    [Range(0f, 20f)][SerializeField] float _maxInAirSpeed = 5f;         // ���߿��� �¿�� �����̴� �ִ� ���ǵ�
    [Range(0f, 20f)][SerializeField] float _fastDropThreshhold = 4f;    // ���� �������� �����ϴ� ����
    [Range(0f, 5f)][SerializeField] float _fastDropPower = 1.0005f;        // ���� �������� ��
    [Range(0f, 100f)][SerializeField] float _maxDropSpeed = 80f;        // �������� �ӵ� �ִ밪

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

        // Change to Wall Grab State
        if (Player.IsTouchedWall && (Player.RecentDir == Mathf.RoundToInt(Player.RawInputs.Movement.x)))
        {
            ChangeState<WallGrabState>();
            return;
        }

        // Change to Dash State
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Player.CanDash && Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0)
            {
                ChangeState<DashState>();
                return;
            }
        }

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
            float xInput = Player.SmoothedInputs.Movement.x;

            // ���߿��� �¿�� ������ �� �ִ�.
            Player.Rigidbody.AddForce(Vector2.right * xInput * _inAirSpeed);

            // ���߿����� �ִ� �̵��ӵ��� �����Ѵ�
            if (Mathf.Abs(Player.Rigidbody.velocity.x) > _maxInAirSpeed)
                Player.Rigidbody.velocity = new Vector2(Mathf.Sign(Player.Rigidbody.velocity.x) * _maxInAirSpeed, Player.Rigidbody.velocity.y);
        }

        // �Ѱ��� ������ �� ���� ������
        if (Player.Rigidbody.velocity.y < _fastDropThreshhold)
        {
            // �������� �ӵ��� �ִ밪 �ο�
            if (Player.Rigidbody.velocity.y < (-1) * _maxDropSpeed)
                Player.Rigidbody.velocity = new Vector2(Player.Rigidbody.velocity.x, (-1) * _maxDropSpeed);
            else
                Player.Rigidbody.AddForce(_fastDropPower * Physics2D.gravity);
        }
    }

    protected override void OnExit()
    {
        Player.Animator.SetBool("IsJump", false);
    }
}