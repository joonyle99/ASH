using System.Linq;
using UnityEngine;

public class GroundPatrolState : Monster_MoveState
{
    [SerializeField] private float _minGroundPatrolTime = 5f;
    [SerializeField] private float _maxGroundPatrolTime = 15f;
    [SerializeField] private float _targetGroundPatrolTime;
    [SerializeField] private float _elapsedGroundPatrolTime;

    [SerializeField] private bool _hasIdleParam;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        _hasIdleParam = animator.parameters.Any(param => param.name == "Idle");

        if (_hasIdleParam)
        {
            _targetGroundPatrolTime = Random.Range(_minGroundPatrolTime, _maxGroundPatrolTime);
            _elapsedGroundPatrolTime = 0f;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (_hasIdleParam)
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

        // flip recentDir after wall check
        if (Monster.GroundPatrolEvaluator)
        {
            if (Monster.GroundPatrolEvaluator.IsWallCheck())
                Monster.SetRecentDir(-Monster.RecentDir);
        }

        // set recentDir for chase
        if (Monster.GroundChaseEvaluator)
        {
            if (Monster.GroundChaseEvaluator.IsTargetWithinChaseRange())
                Monster.SetRecentDir(Monster.GroundChaseEvaluator.ChaseDir);
        }

        if (Monster.MoveType == MonsterDefine.MoveType.GroundWalking)
            GroundPatrol();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }

    private void GroundPatrol()
    {
        Vector2 groundNormal = Monster.GroundRayHit.normal;
        Vector2 moveDirection = Monster.RecentDir > 0
            ? (-1) * Vector2.Perpendicular(groundNormal)
            : Vector2.Perpendicular(groundNormal);

        Debug.DrawRay(Monster.GroundRayHit.point, groundNormal);

        Vector2 targetVelocity = moveDirection * Monster.MoveSpeed;
        Vector2 velocityNeeded = targetVelocity - Vector2.Dot(Monster.Rigidbody.velocity, moveDirection) * moveDirection;
        Vector2 moveForce = velocityNeeded * Monster.Acceleration;

        Debug.DrawRay(Monster.transform.position, moveForce);

        Monster.Rigidbody.AddForce(moveForce);
    }
}
