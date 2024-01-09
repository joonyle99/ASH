using UnityEngine;

public class Monster_AttackState : Monster_StateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex) ;

        // Start God Mode
        Monster.IsGodMode = true;

        // Start Super Armor Flash
        Monster.StartSuperArmorFlash();

        // Start Attack CoolTime
        Monster.AttackEvaluator.StartAttackableTimer();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        // End God Mode
        Monster.IsGodMode = false;

        Monster.EndState();
    }
}