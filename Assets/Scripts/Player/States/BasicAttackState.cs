using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttackState : PlayerState
{
    int _attackCount;

    protected override void SetAnimsOnEnter()
    {
        base.SetAnimsOnEnter();
        Animator.SetInteger("BasicAttackCount", _attackCount);
    }

    protected override void OnEnter()
    {
        _attackCount++;
        if (_attackCount > 1)
            _attackCount = 0;
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnExit()
    {

    }

    public void AnimEvent_FinishBaseAttackAnim()
    {
        ChangeState<IdleState>();
    }

    public void RefreshAttackCount()
    {
        _attackCount = 0;
    }
}
