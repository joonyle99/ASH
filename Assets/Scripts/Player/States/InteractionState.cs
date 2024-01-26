using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D.TopDownShooter;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 플레이어가 상호작용 하는 경우에 진입하는 상태
/// Enum 정보를 가지고 애니메이션 및 동작을 적용한다
/// </summary>
public class InteractionState : PlayerState
{
    protected override void OnEnter()
    {
        switch (Player.InteractionController.InteractionTarget.AnimationType)
        {
            case InteractionAnimationType.Push:
            case InteractionAnimationType.Roll:
                Player.Animator.SetBool("IsPush", true);
                break;
        }
        Player.Animator.SetTrigger("Interact");
    }

    protected override void OnUpdate()
    {
    }

    protected override void OnExit()
    {
        switch (Player.InteractionController.InteractionTarget.AnimationType)
        {
            case InteractionAnimationType.Push:
            case InteractionAnimationType.Roll:
                Player.Animator.SetBool("IsPush", false);
                break;
        }
    }
}
