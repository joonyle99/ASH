using UnityEngine;

/// <summary>
/// wall grab state를 통해
/// wall slide / wall climb state로 갈 수 있다
/// 즉 wall state 중 가장 기본인 상태
/// </summary>
public class WallGrabState : WallState
{
    private float _prevGravity;

    protected override bool OnEnter()
    {
        // Wall Normal, Perpendicular 정보를 받음
        bool isPass = base.OnEnter();
        if (!isPass) return false;

        _prevGravity = Player.Rigidbody.gravityScale;
        Player.Rigidbody.gravityScale = 0f;
        Player.Rigidbody.velocity = Vector2.zero;

        Animator.SetBool("IsGrab", true);

        transform.position = new Vector3(wallHitPos.x - 0.5f * Player.PlayerLookDir2D.x, transform.position.y, transform.position.z);

        return true;
    }

    protected override bool OnUpdate()
    {
        // Wall Climb State
        if (Player.IsMoveYKey)
        {
            ChangeState<WallClimbState>();
            return true;
        }

        return true;
    }
    protected override bool OnExit()
    {
        Player.Rigidbody.gravityScale = _prevGravity;

        Animator.SetBool("IsGrab", false);

        base.OnExit();

        return true;
    }
}
