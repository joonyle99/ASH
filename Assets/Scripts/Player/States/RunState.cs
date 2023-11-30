using UnityEngine;
using UnityEngine.EventSystems;

public class RunState : PlayerState
{
    [Header("Run Setting")]

    [Space]

    [SerializeField] bool _isPlayerOnSlope;
    [SerializeField] Vector2 _curVelocity;
    [SerializeField] Vector2 _moveDir;
    [SerializeField] float _moveDirLength = 2.5f;
    [SerializeField] float _moveForce;
    [SerializeField] float _angleWithGround;
    [SerializeField] float _maxSpeed = 8f;
    [SerializeField] float _acceleration = 15f;
    [SerializeField] float _decceleration = 15f;

    protected override void OnEnter()
    {
        Player.Animator.SetBool("IsRun", true);
    }

    protected override void OnUpdate()
    {
        // Change to Idle State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) == 0)
        {
            ChangeState<IdleState>();
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

        // Change to Wall Grab State
        if (Player.IsTouchedWall && Player.IsLookForceSync && Mathf.RoundToInt(Player.RawInputs.Movement.y) > 0)
        {
            ChangeState<WallGrabState>();
            return;
        }

        // ���� Velocity ����
        _curVelocity = Player.Rigidbody.velocity;

        // -------------------------------------- //
        //              �̵� �ӵ� ����             //
        // -------------------------------------- //

        // ��ǥ �ӵ� ���
        float targetSpeed = Player.RawInputs.Movement.x * _maxSpeed;

        // ���ؾ� �� ���� ���� ���ϱ� ���� �ӵ� ���� ���
        float speedDif = targetSpeed - Player.Rigidbody.velocity.x;

        // ������ �Է��� �ִ� ���
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _acceleration : _decceleration;

        // �̵� ��Ű�� ���� ���Ѵ�.
        _moveForce = speedDif * accelRate;

        // -------------------------------------- //
        //              �̵� ���� ����             //
        // -------------------------------------- //

        // ������ ��
        RaycastHit2D ground = Player.GroundHitWithRayCast;

        // ������ ������ ����
        _angleWithGround = Vector2.Angle(ground.normal, Player.PlayerLookDir2D);

        // ���鿡 �ִ��� üũ
        _isPlayerOnSlope = Mathf.Abs(90f - _angleWithGround) > 10f;

        // ������ ���鿡 ���� MoveDir ����� �޶�����.
        if (_isPlayerOnSlope)
        {
            // ���� �������Ϳ� �÷��̾ �ٶ󺸴� ������ ������ ���Ѵ�
            float dot = Vector2.Dot(ground.normal, Player.PlayerLookDir2D);

            // ���� > 0 == ���� == ��������
            if (dot > 0)
            {
                // Debug.Log("LookDir�� GroundNormal ���̴� ����, �÷��̾�� �������� �����ִ�.");

                // ���������� ���������� ����
                if (Player.PlayerLookDir2D.x > 0f)
                {
                    // Perpendicular()�� ���� ���ʹ� �ð� �ݴ� �������� �׻� 90�� ȸ���� ����
                    _moveDir = (-1) * Vector2.Perpendicular(ground.normal).normalized;
                }
                // ���������� �������� ����
                else
                    _moveDir = Vector2.Perpendicular(ground.normal).normalized;
            }
            // ������ �а�
            // ���� < 0 == �а� == ��������
            else
            {
                // Debug.Log("LookDir�� GroundNormal ���̴� �а�, �÷��̾�� �������� �����ִ�.");

                // ���������� ���������� ����
                if (Player.PlayerLookDir2D.x > 0f)
                    _moveDir = (-1) * Vector2.Perpendicular(ground.normal).normalized;
                // ���������� �������� ����
                else
                    _moveDir = Vector2.Perpendicular(ground.normal).normalized;
            }
        }
        else
        {
            _moveDir = Player.PlayerLookDir2D;
        }
    }
    protected override void OnFixedUpdate()
    {
        /*
        if (_isPlayerOnSlope)
        {
            // �÷��̾� �̵��� ���� �����Ų��
            Player.Rigidbody.AddForce(_moveDir * Mathf.Abs(_moveForce));
        }
        else
        {
            // �÷��̾� �̵��� ���� �����Ų��
            Player.Rigidbody.AddForce(_moveDir * Mathf.Abs(_moveForce));
        }
        */

        // �÷��̾� �̵��� ���� �����Ų��
        Player.Rigidbody.AddForce(Vector2.right * _moveForce);
    }

    protected override void OnExit()
    {
        Player.Animator.SetBool("IsRun", false);
    }

    private void OnDrawGizmosSelected()
    {
        // �÷��̾ �̵��ϴ� ����
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(_moveDir.x, _moveDir.y, 0) * _moveDirLength);
    }
}