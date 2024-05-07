using UnityEngine;

public class BlackPanther_RoarState : Boss_StateBase
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        var blackPanther = (BlackPanther)Boss;

        // TODO: 흑표범 포효 연출을 실행시킨다
        Debug.Log("흑표범 연출 시작");

        // TODO: 흑표범 빛 문양은 연출 안에 포함되어야 하는데 임시로 여기에 작성한다
        if (!blackPanther.isActiveLuminescence)
            blackPanther.SetActiveLuminescence(true);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
