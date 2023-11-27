using UnityEngine;

public class RunState : PlayerState
{
    [Header("Run Setting")]

    [Space]

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
    }
    protected override void OnFixedUpdate()
    {
        // ��ǥ �ӵ� ���
        float targetSpeed = Player.RawInputs.Movement.x * _maxSpeed;

        // ���ؾ� �� ���� ���� ���ϱ� ���� �ӵ� ���� ���
        float speedDif = targetSpeed - Player.Rigidbody.velocity.x;

        // ������ �Է��� �ִ� ���
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _acceleration : _decceleration;

        // �̵� ��Ű�� ���� ���Ѵ�.
        float moveForce = speedDif * accelRate;

        // �÷��̾� �̵��� ���� �����Ų��
        Player.Rigidbody.AddForce(Vector2.right * moveForce);
    }

    protected override void OnExit()
    {
        Player.Animator.SetBool("IsRun", false);
    }
}