using System.Linq;
using UnityEngine;

public class Monster_IdleState : Monster_StateBase
{
    [SerializeField] private float _minIdleTime = 0f;
    [SerializeField] private float _maxIdleTime = 1f;
    [SerializeField] protected float _targetIdleTime;
    [SerializeField] protected float _elapsedIdleTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        _targetIdleTime = Random.Range(_minIdleTime, _maxIdleTime);
        _elapsedIdleTime = 0f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        // patrol
        _elapsedIdleTime += Time.deltaTime;
        if (_elapsedIdleTime > _targetIdleTime)
        {
            _elapsedIdleTime = 0f;
            animator.SetTrigger("Patrol");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
