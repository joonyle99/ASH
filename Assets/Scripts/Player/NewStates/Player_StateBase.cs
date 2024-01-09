using UnityEngine;

public class Player_StateBase : StateMachineBehaviour
{
    // Player Behavior
    public PlayerBehaviour Player { get; private set; }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player = animator.GetComponent<PlayerBehaviour>();
        // Player.UpdateState(this);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
