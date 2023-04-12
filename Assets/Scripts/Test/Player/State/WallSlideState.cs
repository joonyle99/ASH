using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSlideState : PlayerState
{
    protected override void OnEnter()
    {
        Debug.Log("Enter WallSlide");
    }
    protected override void OnUpdate()
    {
        // 땅에 떨어지면 IdleState로

        // Grab Input 시 WallState로

        // 그리고 서서히 땅에 떨어지는 기능 추가
    }

    protected override void OnExit()
    {

    }
}
