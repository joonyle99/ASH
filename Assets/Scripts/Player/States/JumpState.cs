using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : PlayerState
{

    PlayerJumpController _jumpController;

    protected override void OnEnter()
    {
        //Debug.Log("Jump Enter");
        _jumpController = Player.GetComponent<PlayerJumpController>();

        //TODO : ExecuteJumpAnimEvent 애니메이션 이벤트로 실행
        if (Player.PreviousState is WallState)
        {
            Debug.Log("wall jump");
            _jumpController.ExecuteWallJumpAnimEvent();
        }
        else
        {
            Debug.Log("just jump");
            _jumpController.ExecuteJumpAnimEvent();
        }
    }

    protected override void OnUpdate()
    {

    }
    protected override void OnExit()
    {
        //Debug.Log("Jump Exit");
    }
}
