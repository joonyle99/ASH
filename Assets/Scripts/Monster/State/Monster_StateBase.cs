using UnityEngine;

public abstract class Monster_StateBase : StateMachineBehaviour
{
    [Header("Monster_StateBase")]
    [Space]

    [SerializeField] protected bool _isAutoStateTransition = false;

    [Space]

    [SerializeField] protected string _targetTransitionParam;

    [Space]

    [SerializeField] protected float _minStayTime = 0f;
    [SerializeField] protected float _maxStayTime = 0f;

    [Space]

    [SerializeField] protected float _targetStayTime = 0f;
    [SerializeField] protected float _elapsedStayTime = 0f;

    public float TargetStayTime
    {
        get { return _targetStayTime; }
    }
    public float ElaspedStayTime
    {
        get { return _elapsedStayTime; }
        set { _elapsedStayTime = value; }
    }

    // Monster Behavior
    public MonsterBehavior Monster { get; private set; }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Monster = animator.GetComponent<MonsterBehavior>(); // Get Monster Behavior
        Monster.UpdateState(this);                          // Update Monster State

        if (_isAutoStateTransition)
        {
            _elapsedStayTime = 0f;
            _targetStayTime = Random.Range(_minStayTime, _maxStayTime);
        }

        if (Monster.NavMeshMoveModule)
        {
            // NavMesh Agent Stop
            if (Monster.CurrentState is not IMovableState)
                Monster.NavMeshMoveModule.SetStopAgent(true, false);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // auto change to next state
        if (_isAutoStateTransition)
        {
            _elapsedStayTime += Time.deltaTime;
            if (_elapsedStayTime > _targetStayTime)
            {
                _elapsedStayTime = 0f;
                Monster.StartChangeStateCoroutine(_targetTransitionParam, Monster.CurrentState);

                return;
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Monster.NavMeshMoveModule)
        {
            // NavMesh Agent Resume
            if (Monster.CurrentState is not IMovableState)
                Monster.NavMeshMoveModule.SetStopAgent(false, false);
        }

        if (_isAutoStateTransition)
        {
            _elapsedStayTime = 0f;
            _targetStayTime = 0f;
        }
    }
}
