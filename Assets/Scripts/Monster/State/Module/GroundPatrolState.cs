using UnityEngine;

public class GroundPatrolState : Monster_MoveState
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (Monster.IsInAir)
            return;

        // flip recentDir after wall check
        if (Monster.GroundPatrolModule)
        {
            if (Monster.GroundPatrolModule.IsWallCheck())
                Monster.SetRecentDir(-Monster.RecentDir);
        }

        // set recentDir for chase
        if (Monster.GroundChaseEvaluator)
        {
            if (Monster.GroundChaseEvaluator.IsTargetWithinRange())
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
