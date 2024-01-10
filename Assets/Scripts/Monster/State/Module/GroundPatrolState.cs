using System.Collections;
using System.Linq;
using UnityEngine;

public class GroundPatrolState : Monster_StateBase
{
    [SerializeField] private float _targetGroundPatrolTime;
    [SerializeField] private float _elapsedGroundPatrolTime;

    [SerializeField] private bool hasIdleParameter;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        hasIdleParameter = animator.parameters.Any(param => param.name == "Idle");

        _targetGroundPatrolTime = Random.Range(5f, 15f);
        _elapsedGroundPatrolTime = 0f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (hasIdleParameter)
        {
            // change to idle state
            _elapsedGroundPatrolTime += Time.deltaTime;
            if (_elapsedGroundPatrolTime > _targetGroundPatrolTime)
            {
                _elapsedGroundPatrolTime = 0f;
                animator.SetTrigger("Idle");

                return;
            }
        }

        if (Monster.GroundChaseEvaluator)
        {
            // change to chase
            if (Monster.GroundChaseEvaluator.IsTargetWithinChaseRange())
                Monster.SetRecentDir(Monster.GroundChaseEvaluator.ChaseDir);
        }

        if(Monster.MonsterBehav == MonsterDefine.MONSTER_BEHAV.GroundWalk)
            Monster.RigidBody.velocity = Monster.RecentDir * Vector2.right * Monster.MoveSpeed;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
