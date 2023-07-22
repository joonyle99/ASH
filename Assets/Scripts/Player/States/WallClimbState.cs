using UnityEngine;

public class WallClimbState : WallState
{
    [Header("Wall Climb Setting")]
    [SerializeField] float _wallClimbSpeed = 4.0f;
    [SerializeField] float _wallEndJumpPower = 50f;

    protected override void OnEnter()
    {
        base.OnEnter();

        Player.Rigidbody.gravityScale = 0f;

        Animator.SetBool("IsClimb", true);
    }
    protected override void OnUpdate()
    {
        // 위로 올라가기
        if (Player.RawInputs.Movement.y > 0)
        {
            Player.Rigidbody.velocity = moveDirection * _wallClimbSpeed;
        }
        // 아래로 내려가기
        else if (Player.RawInputs.Movement.y < 0)
        {
            Player.Rigidbody.velocity = - moveDirection * _wallClimbSpeed;
        }
        // 가만히 있으면 Wall Grab State로 상태 전이
        else
        {
            ChangeState<WallGrabState>();
            return;
        }

        // TODO : 벽을 타고 올라가다가 벽이 사라지면..? -> 언능 수정하자
        // 1. 더이상 못 올라가게 한다 (Ray중이던 벽의 정보를 가져올 수가 없음)
        // 2. InAirState로 변경한다 (그럼 또 다시 떨어짐,, 버벅임)
        // 3. 점프를 한다 (위로 힘들 주면서)
        if (!Player.IsTouchedWall)
        {
            Player.Rigidbody.velocity = moveDirection * _wallEndJumpPower;
            ChangeState<JumpState>();
            return;
        }

        // Idle State
        if (Player.IsGrounded && Player.RawInputs.Movement.y < 0)
        {
            ChangeState<IdleState>();
            return;
        }
    }

    protected override void OnExit()
    {
        Player.Rigidbody.gravityScale = 5f;

        Animator.SetBool("IsClimb", false);

        base.OnExit();
    }
}
