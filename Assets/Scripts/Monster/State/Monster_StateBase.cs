using UnityEngine;

public abstract class Monster_StateBase : StateMachineBehaviour
{
    [Header("Monster StateBase")]
    [Space]

    [SerializeField] protected bool isAutoStateTransition = false;
    public bool IsAutoStateTransition
    {
        get { return isAutoStateTransition; }
        private set { isAutoStateTransition = value; }
    }

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

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Monster = animator.GetComponent<MonsterBehavior>();     // Get Monster Behavior when State Enter
        Monster.UpdateState(this);                              // Update Monster State when State Enter

        if (isAutoStateTransition)
        {
            elapsedStayTime = 0f;
            targetStayTime = stayTime.Random();
        }

        if (Monster.FloatingMovementModule)
        {
            // NavMesh Agent Stop
            if (Monster.CurrentState is not IMovingState)
                Monster.FloatingMovementModule.SetStopAgent(true, false);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
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

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Monster.FloatingMovementModule)
        {
            // NavMesh Agent Resume
            if (Monster.CurrentState is not IMovingState)
                Monster.FloatingMovementModule.SetStopAgent(false, false);
        }

        if (isAutoStateTransition)
        {
            elapsedStayTime = 0f;
            targetStayTime = 0f;
        }
    }
}
