/// <summary>
/// �÷��̾ ��ȣ�ۿ� �ϴ� ��쿡 �����ϴ� ����
/// Enum ������ ������ �ִϸ��̼� �� ������ �����Ѵ�
/// </summary>
public class InteractionState : PlayerState
{
    protected override bool OnEnter()
    {
        if (Player.PlayerInteractionController.ClosetTarget.AnimationType != InteractionAnimationType.None)
            Player.Animator.SetTrigger("Interact");

        switch (Player.PlayerInteractionController.ClosetTarget.AnimationType)
        {
            case InteractionAnimationType.Push:
                Player.Animator.SetBool("IsPush", true);
                break;
            case InteractionAnimationType.Roll:
                Player.Animator.SetBool("IsRoll", true);
                break;
        }

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
                Player.Animator.SetBool("IsPush", false);
                break;
            case InteractionAnimationType.Roll:
                Player.Animator.SetBool("IsRoll", false);
                break;
        }

        return true;
    }
}
