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

    [SerializeField] private float xInput;
    [SerializeField] private float targetSpeed;
    [SerializeField] private float speedDiff;
    [SerializeField] private float accelRate;
    [SerializeField] private float moveForce;
    [SerializeField] private Vector2 velocity;

    protected override void OnEnter()
    {
        Player.Animator.SetBool("IsRun", true);
    }

    protected override void OnUpdate()
    {
        // player xInput direction
        xInput = Player.RawInputs.Movement.x;

        velocity = Player.Rigidbody.velocity;

        /*
        if (Player.Rigidbody.velocity.x * xInput < 0f)
            Player.Rigidbody.velocity = new Vector2(0f, Player.Rigidbody.velocity.y);
        */

        /*
        // 타겟 속도를 계산한다. 속도는 벡터값이며 스칼라와 방향을 가진다. (x축이므로 1차원)
        targetSpeed = xInput * _moveSpeed;

        // 타겟 속도와 현재 속도를 차이를 구하면서 앞으로 가해질 힘의 방향을 구할 수 있다.
        speedDiff = targetSpeed - Player.Rigidbody.velocity.x;

        // 타겟 속도가 0.01f보다 크다는 것은 움직이고 있는 방향으로 계속해서 가속하는 것을 의미하므로 _acceleration을 사용한다.
        accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _acceleration : _decceleration;

        // 이동 시키는 힘을 구한다.
        moveForce = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, _velPower) * Mathf.Sign(speedDiff);

        // 플레이어 이동에 힘을 적용시킨다
        Player.Rigidbody.AddForce(moveForce * Vector2.right);
        */

        Player.Rigidbody.AddForce(xInput * 50f * Vector2.right);

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