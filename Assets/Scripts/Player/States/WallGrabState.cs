using UnityEngine;

/// <summary>
/// wall grab state를 통해
/// wall slide / wall climb state로 갈 수 있다
/// 즉 wall state 중 가장 기본인 상태
/// </summary>
public class WallGrabState : WallState
{
    protected override void OnEnter()
    {
        base.OnEnter();

        Animator.SetBool("IsGrab", true);

        Player.Rigidbody.gravityScale = 0f;
        Player.Rigidbody.velocity = Vector2.zero;
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
        Animator.SetBool("IsGrab", false);

        Player.Rigidbody.gravityScale = 5f;

        base.OnExit();
    }
}
