using UnityEngine;

public abstract class Monster_StateBase : StateMachineBehaviour
{
    [Header("Monster_StateBase")]
    [Space]

    [SerializeField] protected bool isAutoStateTransition = false;

    [Space]

    [SerializeField] protected string targetTransitionParam;

    [Space]

    [SerializeField] protected Range stayTime;

    [Space]

    [SerializeField] protected float targetStayTime;
    [SerializeField] protected float elapsedStayTime;
    public float ElaspedStayTime
    {
        get { return elapsedStayTime; }
        set { elapsedStayTime = value; }
    }

    // Monster Behavior
    public MonsterBehavior Monster { get; private set; }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Monster = animator.GetComponent<MonsterBehavior>();     // Get Monster Behavior
        Monster.UpdateState(this);                              // Update Monster State

        if (isAutoStateTransition)
        {
            elapsedStayTime = 0f;
            targetStayTime = stayTime.Random();
        }

        if (Monster.NavMeshMovementModule)
        {
            // NavMesh Agent Stop
            if (Monster.CurrentState is not IMovableState)
                Monster.NavMeshMovementModule.SetStopAgent(true, false);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // auto change to next state
        if (isAutoStateTransition)
        {
            elapsedStayTime += Time.deltaTime;
            if (elapsedStayTime > targetStayTime)
            {
                elapsedStayTime = 0f;
                Monster.StartChangeStateCoroutine(targetTransitionParam, Monster.CurrentState);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Monster.NavMeshMovementModule)
        {
            // NavMesh Agent Resume
            if (Monster.CurrentState is not IMovableState)
                Monster.NavMeshMovementModule.SetStopAgent(false, false);
        }

        if (isAutoStateTransition)
        {
            elapsedStayTime = 0f;
            targetStayTime = 0f;
        }
    }
}
