using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WalkState : PlayerState
{
    [SerializeField] float _walkSpeed = 7;

    protected override void OnEnter()
    {

    }
    protected override void OnUpdate()
    {
        float xInput = Player.SmoothedInputs.Movement.x;
        Vector3 targetVelocity = new Vector3(xInput * _walkSpeed, Player.Rigidbody.velocity.y);
        Player.Rigidbody.velocity = targetVelocity;
        
        if (Player.RawInputs.Movement.x == 0)
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
    }

}
