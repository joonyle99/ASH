using System.Linq;
using UnityEngine;

public class Frog_IdleState : Monster_IdleState
{
    [SerializeField] protected bool hasJumpParameter;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        hasJumpParameter = animator.parameters.Any(param => param.name == "Jump");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (hasJumpParameter)
        {
            // change to patrol
            _elapsedIdleTime += Time.deltaTime;
            if (_elapsedIdleTime > _targetIdleTime)
            {
                _elapsedIdleTime = 0f;
                animator.SetTrigger("Jump");

                return;
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
