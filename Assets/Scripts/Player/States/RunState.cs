using UnityEngine;

public class RunState : PlayerState
{
    [Header("Run Setting")]

    [Space]

    [SerializeField] float _maxSpeed = 8f;
    [SerializeField] float _acceleration = 700f;
    [SerializeField] float _decceleration = 700f;

    protected override void OnEnter()
    {
        Player.Animator.SetBool("IsRun", true);
    }

    protected override void OnUpdate()
    {
        // 목표 속도 계산
        float targetSpeed = Player.RawInputs.Movement.x * _maxSpeed;

        // 가해야 할 힘의 양을 구하기 위한 속도 차이 계산
        float speedDif = targetSpeed - Player.Rigidbody.velocity.x;

        // 움직임 입력이 있는 경우
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _acceleration : _decceleration;

        // 이동 시키는 힘을 구한다.
        float moveForce = speedDif * accelRate;

        // 플레이어 이동에 힘을 적용시킨다
        Player.Rigidbody.AddForce(Vector2.right * moveForce * Time.deltaTime);

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
    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {
        Player.Animator.SetBool("IsRun", false);
    }
}