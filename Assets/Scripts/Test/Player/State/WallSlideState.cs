using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class WallSlideState : WallState
{
    [SerializeField] private float _wallSlideSpeed = 3f;

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
        if (Player.RecentDir == Mathf.RoundToInt(Player.RawInputs.Movement.x))
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
            ChangeState<InAirState>();
            return;
        }

        // Jump
        

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
