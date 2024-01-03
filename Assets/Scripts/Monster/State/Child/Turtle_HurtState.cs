using UnityEngine;

public class Turtle_HurtState : Monster_StateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        // Start Hurt
        Monster.IsHurt = true;

        // Start Alpha Blink
        Monster.StartBlink();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        // End Hurt
        Monster.IsHurt = false;

        // Change to Hide State
        animator.SetTrigger("Hide");
    }
}
