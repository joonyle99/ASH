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
        //TODO : ExecuteJumpAnimEvent �ִϸ��̼� �̺�Ʈ�� ����
        _jumpController.ExecuteJumpAnimEvent();
    }

    protected override void OnUpdate()
    {

    }
    protected override void OnExit()
    {
        //Debug.Log("Jump Exit");
    }
}
