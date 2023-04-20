using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class WallSlideState : WallState
{
    [SerializeField] private float _wallSlideSpeed = 1.5f;

    protected override void OnEnter()
    {
        //Debug.Log("Enter WallSlide");
        Animator.SetBool("WallSlide", true);
    }
    protected override void OnUpdate()
    {
        // 서서히 땅에 떨어지는 기능 추가
        Player.Rigidbody.velocity = Vector2.down * _wallSlideSpeed;

        // Wall Grab State
        if (Player.RawInputs.Movement.x != 0 && Player.RawInputs.Movement.y == 0)
        {
            ChangeState<WallGrabState>();
            return;
        }

        // Wall Climb State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.y) != 0)
        {
            ChangeState<WallClimbState>();
            return;
        }

        // InAirState
        if (!Player.IsTouchedWall || (Player.RecentDir == (-1) * Mathf.RoundToInt(Player.RawInputs.Movement.x)))
        {
            // Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ChangeState<InAirState>();
                return;
            }
        }

        // IdleState로
        if (Player.IsGrounded)
        {
            ChangeState<IdleState>();
            return;
        }
    }

    protected override void OnExit()
    {
        //Debug.Log("Exit WallSlide");
        Animator.SetBool("WallSlide", false);
    }
}
