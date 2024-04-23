/// <summary>
/// �÷��̾ ��ȣ�ۿ� �ϴ� ��쿡 �����ϴ� ����
/// Enum ������ ������ �ִϸ��̼� �� ������ �����Ѵ�
/// </summary>
public class InteractionState : PlayerState
{
    protected override bool OnEnter()
    {
        switch (Player.PlayerInteractionController.ClosetTarget.AnimationType)
        {
            case InteractionAnimationType.Push:
            case InteractionAnimationType.Roll:
                Player.Animator.SetBool("IsPush", true);
                break;
        }
        if (Player.PlayerInteractionController.ClosetTarget.AnimationType != InteractionAnimationType.None)
            Player.Animator.SetTrigger("Interact");

        return true;
    }

    protected override bool OnUpdate()
    {

        return true;
    }

    protected override bool OnExit()
    {
        switch (Player.PlayerInteractionController.ClosetTarget.AnimationType)
        {
            case InteractionAnimationType.Push:
            case InteractionAnimationType.Roll:
                Player.Animator.SetBool("IsPush", false);
                break;
        }

        return true;
    }
}
