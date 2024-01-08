using UnityEngine;

public class Monster_IdleState : Monster_StateBase
{
    [SerializeField] private float _targetIdleTime;
    [SerializeField] private float _elapsedIdleTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        _targetIdleTime = Random.Range(1f, 3f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        _elapsedIdleTime += Time.deltaTime;
        if (_elapsedIdleTime > _targetIdleTime)
        {
            _elapsedIdleTime = 0f;
            animator.SetTrigger("Patrol");

            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
