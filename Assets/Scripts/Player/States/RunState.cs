using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class RunState : PlayerState
{
    [Header("Run Setting")]

    [Space]

    [SerializeField] float _moveSpeed = 20f;
    [SerializeField] float _acceleration = 500f;
    [SerializeField] float _decceleration = 500f;
    [SerializeField] float _velPower = 0.9f;

    private float _maxSpeed = 10f;
    private float _moveSpeedAdder = 5000f;


    // TEMP
    [SerializeField] Vector2 velocity;

    protected override void OnEnter()
    {
        Player.Animator.SetBool("IsRun", true);
    }

    protected override void OnUpdate()
    {
        // player xInput direction
        float xInput = Player.RawInputs.Movement.x;

        /*
        velocity = Player.Rigidbody.velocity;

        // Ÿ�� �ӵ��� ����Ѵ�. �ӵ��� ���Ͱ��̸� ��Į��� ������ ������. (x���̹Ƿ� 1����)
        float targetSpeed = xInput * _moveSpeed;

        // Ÿ�� �ӵ��� ���� �ӵ��� ���̸� ���ϸ鼭 ������ ������ ���� ������ ���� �� �ִ�.
        float speedDiff = targetSpeed - Player.Rigidbody.velocity.x;

        // Ÿ�� �ӵ��� 0.01f���� ũ�ٴ� ���� �����̰� �ִ� �������� ����ؼ� �����ϴ� ���� �ǹ��ϹǷ� _acceleration�� ����Ѵ�.
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _acceleration : _decceleration;

        // �̵� ��Ű�� ���� ���Ѵ�.
        float moveForce = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, _velPower) * Mathf.Sign(speedDiff);

        Debug.Log(moveForce);

        // �÷��̾� �̵��� ���� �����Ų��
        Player.Rigidbody.AddForce(Vector2.right * moveForce * Time.deltaTime);
        */

        Player.Rigidbody.AddForce(Vector2.right * _moveSpeedAdder * xInput * Time.deltaTime);

        if (Mathf.Abs(Player.Rigidbody.velocity.x) > _maxSpeed)
            Player.Rigidbody.velocity = new Vector2(Mathf.Sign(Player.Rigidbody.velocity.x) * _maxSpeed, Player.Rigidbody.velocity.y);


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