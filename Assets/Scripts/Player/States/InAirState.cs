using UnityEngine;

public class InAirState : PlayerState
{
    [Header("InAir Setting")]

    [Space]

    [Range(0f, 100f)][SerializeField] float _inAirSpeed = 50f;          // 공중에서 좌우로 움직이는 스피드
    [Range(0f, 20f)][SerializeField] float _maxInAirSpeed = 5f;         // 공중에서 좌우로 움직이는 최대 스피드
    [Range(0f, 20f)][SerializeField] float _fastDropThreshhold = 4f;    // 빨리 떨어지기 시작하는 높이
    [Range(0f, 5f)][SerializeField] float _fastDropPower = 1.0005f;        // 빨리 떨어지는 힘
    [Range(0f, 100f)][SerializeField] float _maxDropSpeed = 80f;        // 떨어지는 속도 최대값

    protected override void OnEnter()
    {

    }

    protected override void OnUpdate()
    {
        // Change to Idle State
        if (Player.IsGrounded)
        {
            // 공중에서 바닥에 닿으면 나는 사운드
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

        // Wall Jump에서 In Air State로 넘어온 경우
        if (Player.IsWallJump)
        {
            // 플레이어의 velocity를 그대로 가지고 간다
            Player.Rigidbody.velocity = new Vector2(Player.Rigidbody.velocity.x, Player.Rigidbody.velocity.y);
            Player.IsWallJump = false;
        }
        // Basic Jump에서 In Air State로 넘어온 경우
        else
        {
            float xInput = Player.SmoothedInputs.Movement.x;

            // 공중에서 좌우로 움직일 수 있다.
            Player.Rigidbody.AddForce(Vector2.right * xInput * _inAirSpeed);

            // 공중에서의 최대 이동속도를 제한한다
            if (Mathf.Abs(Player.Rigidbody.velocity.x) > _maxInAirSpeed)
                Player.Rigidbody.velocity = new Vector2(Mathf.Sign(Player.Rigidbody.velocity.x) * _maxInAirSpeed, Player.Rigidbody.velocity.y);
        }

        // 한계점 지나면 더 빨리 떨어짐
        if (Player.Rigidbody.velocity.y < _fastDropThreshhold)
        {
            // 떨어지는 속도에 최대값 부여
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