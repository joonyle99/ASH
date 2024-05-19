/// <summary>
/// 플레이어가 상호작용 하는 경우에 진입하는 상태
/// Enum 정보를 가지고 애니메이션 및 동작을 적용한다
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
