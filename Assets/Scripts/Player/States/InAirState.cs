using UnityEngine;

public class InAirState : PlayerState
{
    [Header("InAir Setting")]
    [SerializeField] float _inAirSpeed = 7f;            // 공중에서 좌우 움직이는 스피드
    [SerializeField] float _fastDropThreshhold = 7f;    // 빨리 떨어지는 한계 높이
    [SerializeField] float _fastDropPower = 1f;         // 빨리 떨어지는 힘
    [SerializeField] float _maxDropSpeed = -80f;        // 떨어지는 속도 최대값

    protected override void OnEnter()
    {
        //Debug.Log("InAir Enter");
    }

    protected override void OnUpdate()
    {
        // Idle State
        if (Player.IsGrounded)
        {
            ChangeState<IdleState>();
            return;
        }

        // Wall Grab State
        if (Player.IsTouchedWall)
        {
            ChangeState<WallGrabState>();
            return;
        }

        // Wall Jump에서 In Air State로 넘어온 경우
        if (Player.IsWallJump)
        {
            Player.Rigidbody.velocity = new Vector2(Player.Rigidbody.velocity.x, Player.Rigidbody.velocity.y);
            Player.IsWallJump = false;
        }
        // jump -> In Air
        else
        {
            float xInput = Player.SmoothedInputs.Movement.x;
            Player.Rigidbody.velocity = new Vector2(xInput * _inAirSpeed, Player.Rigidbody.velocity.y);
        }

        // 한계점 지나면 더 빨리 떨어짐
        if (Player.Rigidbody.velocity.y < _fastDropThreshhold)
        {
            // 떨어지는 속도에 최대값 부여
            if (Player.Rigidbody.velocity.y < _maxDropSpeed)
                Player.Rigidbody.velocity = Vector2.up * _maxDropSpeed;
            else
                Player.Rigidbody.velocity += _fastDropPower * Physics2D.gravity * Time.deltaTime;
        }
    }

    protected override void OnExit()
    {
        //Debug.Log("InAir Exit");


        Player.Animator.SetBool("Jump", false);

    }
}