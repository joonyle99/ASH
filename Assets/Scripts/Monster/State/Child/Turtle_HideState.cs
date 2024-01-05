using System.Collections;
using UnityEngine;

public class Turtle_HideState : Monster_StateBase
{
    [SerializeField] private float _targetHideTime = 1.5f;
    [SerializeField] private float _elapsedHideTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        // reset hide time
        if (Monster.IsHit)
        {
            Monster.SetIsHit(false);
            _elapsedHideTime = 0f;
        }

        // show after hide time done
        _elapsedHideTime += Time.deltaTime;
        if (_elapsedHideTime > _targetHideTime)
        {
            _elapsedHideTime = 0f;
            animator.SetTrigger("Show");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
