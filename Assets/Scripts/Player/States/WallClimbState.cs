using UnityEngine;

public class WallClimbState : WallState
{
    [Header("Wall Climb State")]
    [Space]

    [Range(0f, 20f)] [SerializeField] float _wallClimbSpeed = 4.0f;

    private float _prevGravity;

    bool IsAboveWall { get { return Player.transform.position.y > wallHitPos.y; } }

    protected override void OnEnter()
    {
        base.OnEnter();

        _prevGravity = Player.Rigidbody.gravityScale;
        Player.Rigidbody.gravityScale = 0f;
        Player.Rigidbody.velocity = Vector2.zero;

        Animator.SetBool("IsClimb", true);
    }

    protected override void OnUpdate()
    {
        // base.OnUpdate();

        // Debug.Log("Climb");

        // 위로 올라가기
        if (Player.IsMoveUpKey)
        {
            // Wall End Jump
            if (!Player.IsTouchedWall && IsAboveWall)
            {
                ChangeState<JumpState>();
                return;
            }

<<<<<<< Updated upstream
            // 머리 위에 뭐가 있으면 이동을 못함
            if (Player.UpwardHit)
=======
            // 머리 위에 땅이 있으면 이동을 못함
            if (Player.UpwardGroundHit)
>>>>>>> Stashed changes
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

        base.OnExit();
    }
}
