using UnityEngine;

public class WallClimbState : WallState
{
    [Header("Wall Climb State")]
    [Space]

    [Range(0f, 20f)] [SerializeField] float _wallClimbSpeed = 4.0f;

    private float _prevGravity;

    protected override void OnEnter()
    {
        _prevGravity = Player.Rigidbody.gravityScale;
        Player.Rigidbody.gravityScale = 0f;
        Player.Rigidbody.velocity = Vector2.zero;

        Animator.SetBool("IsClimb", true);
    }
    protected override void OnUpdate()
    {
        // 위로 올라가기
        if (Player.IsMoveUpKey)
        {
            // Wall End Jump
            if (!Player.IsTouchedWall && Player.transform.position.y > wallHitPos.y)
            {
                ChangeState<JumpState>();
                return;
            }

            // 머리 위에 뭐가 있으면 이동을 못함
            if (Player.UpwardGroundHit)
                return;

            transform.position += Vector3.up * _wallClimbSpeed * Time.deltaTime;
        }
        // 아래로 내려가기
        else if (Player.IsMoveDownKey)
        {
            if (!Player.IsTouchedWall)
            {
                ChangeState<InAirState>();
                return;
            }

            transform.position -= Vector3.up * _wallClimbSpeed * Time.deltaTime;
        }
        // 가만히 있으면 Wall Grab State로 상태 전이
        else
        {
            ChangeState<WallGrabState>();
            return;
        }

        // Idle State
        if (Player.IsGrounded && Player.IsMoveDownKey)
        {
            ChangeState<IdleState>();
            return;
        }
    }

    protected override void OnExit()
    {
        Player.Rigidbody.gravityScale = _prevGravity;

        Animator.SetBool("IsClimb", false);
    }
}
