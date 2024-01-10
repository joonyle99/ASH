using System.Collections;
using UnityEngine;

public class Monster_HideState : Monster_StateBase
{
    [SerializeField] private float _minHideTime = 2f;
    [SerializeField] private float _maxHideTime = 5f;
    [SerializeField] private float _targetHideTime;
    [SerializeField] private float _elapsedHideTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        _targetHideTime = Random.Range(_minHideTime, _maxHideTime);
        _elapsedHideTime = 0f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        // reset hide time
        if (Monster.IsHit)
            _elapsedHideTime = 0f;

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
