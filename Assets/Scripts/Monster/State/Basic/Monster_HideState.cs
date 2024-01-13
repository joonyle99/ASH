using System.Linq;
using UnityEngine;

public class Monster_HideState : Monster_StateBase
{
    [SerializeField] private float _minHideTime = 2f;
    [SerializeField] private float _maxHideTime = 5f;
    [SerializeField] private float _targetHideTime;
    [SerializeField] private float _elapsedHideTime;

    [SerializeField] private bool _hasShowParam;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //Debug.Log("StateMachineBehaviour callbacks - Hide State's OnStateEnter()");

        _hasShowParam = animator.parameters.Any(param => param.name == "Show");

        if (_hasShowParam)
        {
            _targetHideTime = Random.Range(_minHideTime, _maxHideTime);
            _elapsedHideTime = 0f;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        // Show Animation이 있는 경우에만 실행하는 프로세스
        if (_hasShowParam)
        {
            // reset hide time
            if (Monster.IsHit)
                _elapsedHideTime = 0f;

            // show after hide time done
            _elapsedHideTime += Time.deltaTime;
            if (_elapsedHideTime > _targetHideTime)
            {
                _elapsedHideTime = 0f;
                animator.SetTrigger("Show");

                return;
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
