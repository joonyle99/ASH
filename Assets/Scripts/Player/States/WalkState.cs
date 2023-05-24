using UnityEngine;

public class WalkState : PlayerState
{
    [Header("Walk Setting")]
    [SerializeField] float _walkSpeed = 7;

    protected override void OnEnter()
    {
        Player.Animator.SetBool("Walk", true);
    }
    protected override void OnUpdate()
    {
        float xInput = Player.SmoothedInputs.Movement.x;
        Vector2 targetVelocity = new Vector2(xInput * _walkSpeed, Player.Rigidbody.velocity.y);
        Player.Rigidbody.velocity = targetVelocity;

        // Idle State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) == 0)
        {
            ChangeState<IdleState>();
            return;
        }

        // Wall Grab State
        if (Player.IsTouchedWall && (Player.RecentDir == Mathf.RoundToInt(Player.RawInputs.Movement.x)))
        {
            ChangeState<WallGrabState>();
            return;
        }
    }

    protected override void OnExit()
    {
        Player.Animator.SetBool("Walk", false);
    }

}
