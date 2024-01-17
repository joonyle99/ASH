using System.ComponentModel;
using UnityEngine;

public class Monster_StateBase : StateMachineBehaviour
{
    [Header("State Base")]
    [Space]

    [Header("Auto Change State")]
    [Space]

    [SerializeField] protected bool _isAutoStateTransition = false;
    [SerializeField] protected string _targetTransitionParam;
    [SerializeField] protected float _minStayTime = 0f;
    [SerializeField] protected float _maxStayTime = 0f;

    [Space]

    [SerializeField] protected float _targetStayTime = 0f;
    [SerializeField] protected float _elapsedStayTime = 0f;

    // Monster Behavior
    public MonsterBehavior Monster { get; private set; }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Monster = animator.GetComponent<MonsterBehavior>();
        Monster.UpdateState(this);

        _targetStayTime = 0f;
        _elapsedStayTime = 0f;

        if (_isAutoStateTransition)
        {
            // set random time for auto change state
            _targetStayTime = Random.Range(_minStayTime, _maxStayTime);
            _elapsedStayTime = 0f;
        }

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
        // auto change to next state
        if (_isAutoStateTransition)
        {
            _elapsedStayTime += Time.deltaTime;
            if (_elapsedStayTime > _targetStayTime)
            {
                _elapsedStayTime = 0f;
                animator.SetTrigger(_targetTransitionParam);

                return;
            }
        }
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