using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class RunState : PlayerState
{
    [Header("Run Setting")]

    [Space]

    [Range(0f, 30f)][SerializeField] float _moveSpeed = 10f;
    [Range(0f, 50f)][SerializeField] float _acceleration = 7f;
    [Range(0f, 50f)][SerializeField] float _decceleration = 7f;
    [Range(0f, 2f)][SerializeField] float _velPower = 0.9f;

    protected override void OnEnter()
    {
        Player.Animator.SetBool("IsRun", true);
    }

    protected override void OnUpdate()
    {
        // player xInput direction
        float xInput = Player.RawInputs.Movement.x;

        if (Player.Rigidbody.velocity.x * xInput < 0f)
            Player.Rigidbody.velocity = new Vector2(0f, Player.Rigidbody.velocity.y);

        // Ÿ�� �ӵ��� ����Ѵ�. �ӵ��� ���Ͱ��̸� ��Į��� ������ ������. (x���̹Ƿ� 1����)
        float targetSpeed = xInput * _moveSpeed;

        // Ÿ�� �ӵ��� ���� �ӵ��� ���̸� ���ϸ鼭 ������ ������ ���� ������ ���� �� �ִ�.
        float speedDiff = targetSpeed - Player.Rigidbody.velocity.x;

        // Ÿ�� �ӵ��� 0.01f���� ũ�ٴ� ���� �����̰� �ִ� �������� ����ؼ� �����ϴ� ���� �ǹ��ϹǷ� _acceleration�� ����Ѵ�.
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _acceleration : _decceleration;

        // �̵� ��Ű�� ���� ���Ѵ�.
        float moveForce = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, _velPower) * Mathf.Sign(speedDiff);

        // �÷��̾� �̵��� ���� �����Ų��
        Player.Rigidbody.AddForce(moveForce * Vector2.right);

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
        if (Player.IsTouchedWall && (Player.RecentDir == Mathf.RoundToInt(Player.RawInputs.Movement.x)) && Mathf.RoundToInt(Player.RawInputs.Movement.y) > 0)
        {
            ChangeState<WallGrabState>();
            return;
        }
    }

    protected override void OnExit()
    {
        Player.Animator.SetBool("IsRun", false);
    }
}