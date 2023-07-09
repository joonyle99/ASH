using UnityEngine;

public class InAirState : PlayerState
{
    [Header("InAir Setting")]
    [SerializeField] float _inAirSpeed = 7f;            // ���߿��� �¿� �����̴� ���ǵ�
    [SerializeField] float _fastDropThreshhold = 7f;    // ���� �������� �Ѱ� ����
    [SerializeField] float _fastDropPower = 1f;         // ���� �������� ��
    [SerializeField] float _maxDropSpeed = -80f;        // �������� �ӵ� �ִ밪

    protected override void OnEnter()
    {
        //Debug.Log("InAir Enter");
    }

    protected override void OnUpdate()
    {
        // Idle State
        if (Player.IsGrounded)
        {
            ChangeState<IdleState>();
            return;
        }

        // Wall Grab State
        if (Player.IsTouchedWall)
        {
            ChangeState<WallGrabState>();
            return;
        }

        // Wall Jump���� In Air State�� �Ѿ�� ���
        if (Player.IsWallJump)
        {
            Player.Rigidbody.velocity = new Vector2(Player.Rigidbody.velocity.x, Player.Rigidbody.velocity.y);
            Player.IsWallJump = false;
        }
        // jump -> In Air
        else
        {
            float xInput = Player.SmoothedInputs.Movement.x;
            Player.Rigidbody.velocity = new Vector2(xInput * _inAirSpeed, Player.Rigidbody.velocity.y);
        }

        // �Ѱ��� ������ �� ���� ������
        if (Player.Rigidbody.velocity.y < _fastDropThreshhold)
        {
            // �������� �ӵ��� �ִ밪 �ο�
            if (Player.Rigidbody.velocity.y < _maxDropSpeed)
                Player.Rigidbody.velocity = Vector2.up * _maxDropSpeed;
            else
                Player.Rigidbody.velocity += _fastDropPower * Physics2D.gravity * Time.deltaTime;
        }
    }

    protected override void OnExit()
    {
        //Debug.Log("InAir Exit");


        Player.Animator.SetBool("Jump", false);

    }
}