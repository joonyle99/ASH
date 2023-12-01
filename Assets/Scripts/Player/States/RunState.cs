using UnityEngine;
using UnityEngine.EventSystems;

public class RunState : PlayerState
{
    [Header("Run Setting")]

    [Space]

    [SerializeField] bool _isPlayerOnSlope;
    [SerializeField] float _movePower;
    [SerializeField] float _angleWithGround;
    [SerializeField] float _maxSpeed = 8f;
    [SerializeField] float _acceleration = 15f;
    [SerializeField] float _decceleration = 15f;

    Vector2 _moveForce;
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
        if (Player.IsTouchedWall && Player.IsDirSync && Mathf.RoundToInt(Player.RawInputs.Movement.y) > 0)
        {
            ChangeState<WallGrabState>();
            return;
        }


        // -------------------------------------- //
        //              �̵� �ӵ� ����             //
        // -------------------------------------- //

        // ��ǥ �ӵ� ���
        Vector2 targetVelocity = Player.GroundAlignedMoveDir * _maxSpeed;

        // ���ؾ� �� ���� ���� ���ϱ� ���� �ӵ� ���� ���
        Vector2 speedDif = targetVelocity - Player.Rigidbody.velocity;

        // ������ �Է��� �ִ� ���
        float accelRate = (speedDif.magnitude > 0.01f) ? _acceleration : _decceleration;

        //�̵� ��Ű�� ���� ���Ѵ�.
        _moveForce = speedDif * accelRate;

    }
    protected override void OnFixedUpdate()
    {
        // �÷��̾� �̵��� ���� �����Ų��
        Player.Rigidbody.AddForce(_moveForce * _movePower);
    }

    protected override void OnExit()
    {
        Player.Animator.SetBool("IsRun", false);
    }

    private void OnDrawGizmosSelected()
    {
        // �÷��̾ �̵��ϴ� ����
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(_moveForce.x, _moveForce.y, 0));
    }
}