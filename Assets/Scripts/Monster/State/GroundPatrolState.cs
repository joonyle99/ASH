using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GroundPatrolState : Monster_StateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        // 자기가 바라보는 방향으로 직진
        Monster.RigidBody.velocity = Monster.RecentDir * Vector2.right * Monster.MoveSpeed;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
