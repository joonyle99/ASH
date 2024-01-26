using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D.TopDownShooter;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// �÷��̾ ��ȣ�ۿ� �ϴ� ��쿡 �����ϴ� ����
/// Enum ������ ������ �ִϸ��̼� �� ������ �����Ѵ�
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
