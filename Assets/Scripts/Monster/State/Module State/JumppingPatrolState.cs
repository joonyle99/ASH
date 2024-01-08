using System.Collections;
using UnityEngine;

public class JumppingPatrolState : Monster_StateBase
{
    private float _jumpPowerX = 20f;
    private float _jumpPowerY = 30f;

    [SerializeField] private float _targetJumppingPatrolTime;
    [SerializeField] private float _elapsedJumppingPatrolTime;

    [SerializeField] private float _targetJumppingCoolTime;
    [SerializeField] private float _elapsedJumppingCoolTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        _targetJumppingPatrolTime = Random.Range(5f, 15f);
        _elapsedJumppingPatrolTime = 0f;

        _targetJumppingCoolTime = Random.Range(1f, 2f);
        _elapsedJumppingCoolTime = 0f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        // change to idle state
        _elapsedJumppingPatrolTime += Time.deltaTime;
        if (_elapsedJumppingPatrolTime > _targetJumppingPatrolTime)
        {
            _elapsedJumppingPatrolTime = 0f;
            animator.SetTrigger("Idle");

            return;
        }

        // jump
        _elapsedJumppingCoolTime += Time.deltaTime;
        if (_elapsedJumppingCoolTime > _targetJumppingCoolTime)
        {
            _elapsedJumppingCoolTime = 0f;
            Vector2 forceVector = new Vector2(_jumpPowerX * Monster.RecentDir, _jumpPowerY);
            Monster.RigidBody.AddForce(forceVector, ForceMode2D.Impulse);

            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
