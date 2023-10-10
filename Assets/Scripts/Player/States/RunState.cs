using UnityEngine;

public class RunState : PlayerState
{
    [Header("Run Setting")]

    [Space]

    [Range(0f, 200f)][SerializeField] float _speedAdder = 60f;
    [Range(0f, 10f)][SerializeField] float _maxSpeed = 3f;

    protected override void OnEnter()
    {
        Player.Animator.SetBool("IsRun", true);
    }

    protected override void OnUpdate()
    {
        // 플레이어 이동
        float xInput = Player.SmoothedInputs.Movement.x;

        // TODO : speedAdder값을 변경하는데 이동속도가 바뀐다..? 이건 좀 이상한데,,
        // TODO : AddForce & velocity = & vecocity += 의 차이점에 대해 명확히 알아야 할 듯

        // Player.Rigidbody.AddForce(Vector2.right * xInput * _speedAdder);
        Player.Rigidbody.velocity += Vector2.right * xInput * _speedAdder;

        // 플레이어의 최대 이동속도를 제한한다
        // 분명히 최대 속도는 이렇게 될텐데 왜 더 빨라지지..?
        if (Mathf.Abs(Player.Rigidbody.velocity.x) > _maxSpeed)
            Player.Rigidbody.velocity = new Vector2(Mathf.Sign(Player.Rigidbody.velocity.x) * _maxSpeed, Player.Rigidbody.velocity.y);

        // Idle State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) == 0)
        {
            ChangeState<IdleState>();
            return;
        }

        // Dash State
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Player.CanDash && Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0)
            {
                ChangeState<DashState>();
                return;
            }
        }

        // Wall Grab State
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