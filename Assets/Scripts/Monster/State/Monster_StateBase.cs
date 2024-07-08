using UnityEngine;

public abstract class Monster_StateBase : StateMachineBehaviour
{
    [Header("Monster StateBase")]
    [Space]

    [SerializeField] private bool isAutoStateTransition;        // 커스텀 에디터에서 사용하기 때문에 프로퍼티가 아닌 필드로 선언
    public bool IsAutoStateTransition => isAutoStateTransition;

    [Space]

    [Tooltip("Transition trigger parameter name")]
    [SerializeField] protected string targetTransitionParam;

    [Space]

    [SerializeField] protected Range stayTimeRange;
    [SerializeField] protected float targetStayTime;
    public float ElapsedStayTime;

    // Monster Behavior
    public MonsterBehaviour Monster { get; private set; }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Debug.Log($"{this.GetType().Name} enter");

        Monster = animator.GetComponent<MonsterBehaviour>();     // Get Monster Behavior when State Enter
        Monster.UpdateState(this);                              // Update Monster State when State Enter

        if (IsAutoStateTransition)
        {
            ElapsedStayTime = 0f;
            targetStayTime = stayTimeRange.Random();
        }

        if (Monster.FloatingMovementModule)
        {
            // when enter not movable state, set agent stop
            if (Monster.CurrentState is not IMovingState)
            {
                Monster.FloatingMovementModule.SetStopAgent(true);
            }
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Debug.Log($"{this.GetType().Name} update");

        if (Monster.IsDead) return;

        // auto change to next state
        if (IsAutoStateTransition)
        {
            ElapsedStayTime += Time.deltaTime;
            if (ElapsedStayTime > targetStayTime)
            {
                ElapsedStayTime = 0f;
                Monster.StartChangeStateCoroutine(targetTransitionParam, Monster.CurrentState);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Debug.Log($"{this.GetType().Name} exit");

        // monster behavior가 enable false인 채로 시작하고
        // monster의 state가 exit가 되는 경우 enter가 없었기 때문에 monster가 null일 수 있다.
        if (Monster)
        {
            if (Monster.FloatingMovementModule)
            {
                if (Monster.CurrentState is not IMovingState)
                {
                    Monster.FloatingMovementModule.SetStopAgent(false);
                }
            }
        }
    }
}
