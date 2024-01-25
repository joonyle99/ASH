using UnityEngine;

public class SemiBoss_RageState : SemiBoss_StateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        // 격노 상태 시작
        SemiBoss.IsRage = true;

        // TODO : 외관이 좀 바뀌어도 좋을 것 같다.
        // like 머터리얼 변경
        SemiBoss.StartFlashTimer(3f);
        SemiBoss.StartSuperArmorFlash();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        // 보스가 격노하면 이동속도 / 공격속도 증가
        SemiBoss.MoveSpeed *= 1.5f;
        SemiBoss.AttackEvaluator.TargetCheckCoolTime /= 1.5f;
    }
}
