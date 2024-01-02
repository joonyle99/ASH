using UnityEngine;

/// <summary>
/// 몬스터의 공통 AttackState
/// </summary>
public class Monster_AttackState : Monster_StateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex) ;

        Monster.AttackEvaluator.StartAttackableTimer();

        Monster.IsGodMode = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        Monster.IsGodMode = false;

        animator.SetTrigger("Idle");
    }
}