using UnityEngine;

public class Monster_IdleState : Monster_StateBase
{
    [SerializeField] private float _targetStayTime = 2f;
    [SerializeField] private float _elapsedStayTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        _elapsedStayTime += Time.deltaTime;

        if (_elapsedStayTime > _targetStayTime)
        {
            _elapsedStayTime = 0f;
            animator.SetTrigger("Patrol");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
