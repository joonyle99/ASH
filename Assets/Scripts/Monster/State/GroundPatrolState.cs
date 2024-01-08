using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GroundPatrolState : Monster_StateBase
{
    [SerializeField] private float _targetPatrolTime;
    [SerializeField] private float _elapsedPatrolTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        _targetPatrolTime = Random.Range(5f, 15f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        _elapsedPatrolTime += Time.deltaTime;
        if (_elapsedPatrolTime > _targetPatrolTime)
        {
            _elapsedPatrolTime = 0f;
            animator.SetTrigger("Idle");

            return;
        }

        if (Monster.GroundPatrolEvaluator.IsCheckWall())
            Monster.UpdateImageFlip();

        // 자기가 바라보는 방향으로 직진
        Monster.RigidBody.velocity = Monster.RecentDir * Vector2.right * Monster.MoveSpeed;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
