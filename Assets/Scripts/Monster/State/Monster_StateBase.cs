using UnityEngine;

public class Monster_StateBase : StateMachineBehaviour
{
    // Monster Behavior
    public MonsterBehavior Monster { get; private set; }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Monster = animator.GetComponent<MonsterBehavior>();
        Monster.UpdateState(this);

        if (Monster.NavMeshMoveModule)
        {
            // NavMesh Agent Stop
            if (Monster.CurrentStateIs<Monster_AttackState>() ||
                Monster.CurrentStateIs<Monster_HurtState>() ||
                Monster.CurrentStateIs<Monster_DieState>())
                Monster.NavMeshMoveModule.SetStopAgent(true);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Monster.NavMeshMoveModule)
        {
            // NavMesh Agent Resume
            if (Monster.CurrentStateIs<Monster_AttackState>() ||
                Monster.CurrentStateIs<Monster_HurtState>() ||
                Monster.CurrentStateIs<Monster_DieState>())
                Monster.NavMeshMoveModule.SetStopAgent(false);
        }
    }
}