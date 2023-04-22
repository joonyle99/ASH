using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGrabState : WallState
{
    protected override void OnEnter()
    {
        //Debug.Log("Enter Wall Grab");
        Player.Rigidbody.gravityScale = 0f;
    }

    protected override void OnUpdate()
    {
        // Player Stop
        Player.Rigidbody.velocity = Vector2.zero;

        // Wall Climb State
        if (Player.RawInputs.Movement.y != 0f )
        {
            ChangeState<WallClimbState>();
            return;
        }

        // Wall Slide State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) == 0 && Mathf.RoundToInt(Player.RawInputs.Movement.y) == 0)
        {
            ChangeState<WallSlideState>();
            return;
        }
    }
    protected override void OnExit()
    {
        //Debug.Log("Exit Wall Grab");
        Player.Rigidbody.gravityScale = 5f;
    }
}
