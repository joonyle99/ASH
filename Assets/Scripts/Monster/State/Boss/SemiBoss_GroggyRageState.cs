using UnityEngine;

public class SemiBoss_GroggyRageState : SemiBoss_StateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        // 무적 시작. 피격될 수 없다.
        SemiBoss.IsGodMode = true;

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

        SemiBoss.GroggyPostProcess();

        // 보스가 격노하면 이동속도 / 공격속도 증가
        SemiBoss.MoveSpeed *= 2f;
        SemiBoss.AttackEvaluator.TargetCheckCoolTime /= 2f;
    }
}
