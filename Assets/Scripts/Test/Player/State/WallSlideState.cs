using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class WallSlideState : PlayerState
{
    [SerializeField] private float _wallSlideSpeed = -3f;

    protected override void OnEnter()
    {
        Debug.Log("Enter WallSlide");
    }
    protected override void OnUpdate()
    {
        // 그리고 서서히 땅에 떨어지는 기능 추가
        Player.Rigidbody.velocity = new Vector2(0, _wallSlideSpeed);

        // 땅에 떨어지면 IdleState로
        if (Player.IsGrounded)
            ChangeState<IdleState>();

        // Grab Input 시 WallGrabState로
        //if (grabInput && yInput == 0)
        //    ChangeState<WallGrabState>();
    }

    protected override void OnExit()
    {
        Debug.Log("Exit WallSlide");
    }
}
