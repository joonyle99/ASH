using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClimbState : WallState
{
    [SerializeField] private float _wallClimbSpeed = 2.5f;
    protected override void OnEnter()
    {
        Debug.Log("Enter Wall Climb");
        Player.Rigidbody.gravityScale = 0f;
        Animator.SetBool("WallClimb", true);
    }
    protected override void OnUpdate()
    {
        // Move To Up & Down
        if (Player.RawInputs.Movement.y > 0)
            Player.Rigidbody.velocity = Vector2.up * _wallClimbSpeed;
        else if (Player.RawInputs.Movement.y < 0)
            Player.Rigidbody.velocity = Vector2.down * _wallClimbSpeed;
        // Stop Move
        else
        {
            ChangeState<WallGrabState>();
            return;
        }

        // IdleState
        if (Player.IsGrounded)
        {
            ChangeState<IdleState>();
            return;
        }

        // Wall Jump
        // 벽 반대방향 키
        if(Player.RecentDir == (-1) * Mathf.RoundToInt(Player.RawInputs.Movement.x))
        {
            // 위쪽 키
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Wall Jump");
            }
        }

        
        //// InAirState
        //if (!Player.IsTouchedWall || (Player.RecentDir == (-1) * Mathf.RoundToInt(Player.RawInputs.Movement.x)))
        //{
        //    ChangeState<InAirState>();
        //    return;
        //}
    }

    protected override void OnExit()
    {
        Debug.Log("Exit Wall Climb");
        Player.Rigidbody.gravityScale = 5f;
        Animator.SetBool("WallClimb", false);
    }
}
