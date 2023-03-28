using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : PlayerState
{
    [SerializeField] float _groundJumpPower = 9f;
    [SerializeField] float _inAirJumpPower = 9f;

    bool _isGroundJump { get { return Player.MaxJumpCount == Player.RemainingJumpCount; } }
    public void ExecuteJump()
    {
        float jumpPower = _isGroundJump ? _groundJumpPower : _inAirJumpPower;
        Player.Rigidbody.velocity = new Vector2(Player.Rigidbody.velocity.x, jumpPower);
       
        Player.RemainingJumpCount -= 1;
        ChangeState<InAirState>();
    }

    protected override void OnEnter()
    {
        //TODO : ExecuteJump�� �ִϸ��̼� �̺�Ʈ�� ����
        ExecuteJump();
    }

    protected override void OnUpdate()
    {
    }
    protected override void OnExit()
    {
    }
}
