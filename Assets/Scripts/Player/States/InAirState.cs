using UnityEngine;

public class InAirState : PlayerState
{
    [Header("InAir Setting")]

    [Space]

    [Range(0f, 20f)] [SerializeField] float _inAirSpeed = 7f;            // 공중에서 좌우로 움직이는 스피드
    [Range(0f, 20f)] [SerializeField] float _fastDropThreshhold = 7f;    // 빨리 떨어지기 시작하는 높이
    [Range(0f, 5f)] [SerializeField] float _fastDropPower = 1.1f;        // 빨리 떨어지는 힘
    [Range(0f, 100f)] [SerializeField] float _maxDropSpeed = 80f;        // 떨어지는 속도 최대값

    protected override void OnEnter()
    {

    }

    protected override void OnUpdate()
    {
        // Idle State
        if (Player.IsGrounded)
        {
            // 공중에서 바닥에 닿으면 나는 사운드
            Player.PlaySound_SE_Jump_02();

            ChangeState<IdleState>();
            return;
        }

        // Wall Grab State
        if (Player.IsTouchedWall && (Player.RecentDir == Mathf.RoundToInt(Player.RawInputs.Movement.x)))
        {
            ChangeState<WallGrabState>();
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

        // Dive State
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
        // jump -> In Air
        else
        {
            // 공중에서 좌우로 움직일 수 있다.
            float xInput = Player.SmoothedInputs.Movement.x;
            Player.Rigidbody.velocity = new Vector2(xInput * _inAirSpeed, Player.Rigidbody.velocity.y);
        }

        // 한계점 지나면 더 빨리 떨어짐
        if (Player.Rigidbody.velocity.y < _fastDropThreshhold)
        {
            // 떨어지는 속도에 최대값 부여
            if (Player.Rigidbody.velocity.y < (-1) * _maxDropSpeed)
                Player.Rigidbody.velocity = Vector2.down * _maxDropSpeed;
            else
                Player.Rigidbody.velocity += _fastDropPower * Physics2D.gravity * Time.deltaTime;
        }
    }

    protected override void OnExit()
    {
        Player.Animator.SetBool("IsJump", false);
    }
}