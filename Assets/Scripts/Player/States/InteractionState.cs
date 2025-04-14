using UnityEngine;

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
        //PlayerAnimator의 Interaction_Entry진입 전 OnEnter(), OnExit()호출 시
        //IsPush 와 IsRoll이 둘 다 false가 되어 다음 스테이트로 진입하지 못함

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
