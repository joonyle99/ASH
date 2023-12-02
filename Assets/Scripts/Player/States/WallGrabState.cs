using UnityEngine;

/// <summary>
/// wall grab state를 통해
/// wall slide / wall climb state로 갈 수 있다
/// 즉 wall state 중 가장 기본인 상태
/// </summary>
public class WallGrabState : WallState
{
    private float _paddingValue = 0.5f;
    private float _prevGravity;

    protected override void OnEnter()
    {
        base.OnEnter();

        _prevGravity = Player.Rigidbody.gravityScale;
        Player.Rigidbody.gravityScale = 0f;
        Player.Rigidbody.velocity = Vector2.zero;

        Animator.SetBool("IsGrab", true);

        transform.position = new Vector3(wallHitPos.x - _paddingValue * Player.PlayerLookDir2D.x, transform.position.y, transform.position.z);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        // Debug.Log("Grab");

        // Wall Climb State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.y) != 0)
        {
            ChangeState<WallClimbState>();
            return;
        }

        /*
        // Wall Slide State
        // 방향키 입력 정보가 없을때
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) == 0 && Mathf.RoundToInt(Player.RawInputs.Movement.y) == 0)
        {
            ChangeState<WallSlideState>();
            return;
        }
        */
    }
    protected override void OnExit()
    {
        Player.Rigidbody.gravityScale = _prevGravity;

        Animator.SetBool("IsGrab", false);

        base.OnExit();
    }
}
