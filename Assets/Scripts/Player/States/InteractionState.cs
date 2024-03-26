using UnityEngine;

/// <summary>
/// �÷��̾ ��ȣ�ۿ� �ϴ� ��쿡 �����ϴ� ����
/// Enum ������ ������ �ִϸ��̼� �� ������ �����Ѵ�
/// </summary>
public class InteractionState : PlayerState
{
    protected override bool OnEnter()
    {
        switch (Player.InteractionController.InteractionTarget.AnimationType)
        {
            case InteractionAnimationType.Push:
            case InteractionAnimationType.Roll:
                Player.Animator.SetBool("IsPush", true);
                break;
        }
        if (Player.InteractionController.InteractionTarget.AnimationType != InteractionAnimationType.None)
            Player.Animator.SetTrigger("Interact");

        return true;
    }

    protected override bool OnUpdate()
    {

        return true;
    }

    protected override bool OnExit()
    {
        switch (Player.InteractionController.InteractionTarget.AnimationType)
        {
            case InteractionAnimationType.Push:
            case InteractionAnimationType.Roll:
                Player.Animator.SetBool("IsPush", false);
                break;
        }

        return true;
    }
}
