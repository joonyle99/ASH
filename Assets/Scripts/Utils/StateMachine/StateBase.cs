using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase : MonoBehaviour
{
    // [SerializeField] StateAnimatorParamData _animatorParameters;

    protected StateMachineBase StateMachine { get; private set; }
    protected Animator Animator => StateMachine.Animator;

    private void Awake()
    {
        // 여러 State를 가진 GameObject는 하나의 StateMachine을 가지고 있다.
        StateMachine = GetComponent<StateMachineBase>();
    }

    public void TriggerEnter()
    {
        // _animatorParameters.InvokeEnter(StateMachine.Animator);

        OnEnter();
    }
    public void TriggerUpdate()
    {
        OnUpdate();
    }
    public void TriggerFixedUpdate()
    {
        OnFixedUpdate();
    }
    public void TriggerExit()
    {
        // _animatorParameters.InvokeExit(StateMachine.Animator);

        OnExit();
    }

    protected abstract bool OnEnter();
    protected abstract bool OnUpdate();
    protected abstract bool OnExit();
    protected virtual bool OnFixedUpdate() { return true; }

    public TNextState ChangeState<TNextState>(bool ignoreSameState = false) where TNextState : StateBase
    {
        return StateMachine.ChangeState<TNextState>(ignoreSameState);
    }
    public bool CurrentStateIs<TState>() where TState : StateBase
    {
        return StateMachine.CurrentState is TState;
    }

    //public new T GetComponent<T>() where T : Component
    //{
    //    return StateMachine.GetComponent<T>();
    //}
}

/*
[System.Serializable]
public class StateAnimatorParamData
{
    [SerializeField] List<AnimatorParamData> _enterParams;
    [SerializeField] List<AnimatorParamData> _exitParams;
    public void InvokeEnter(Animator animator)
    {
        foreach (var animParamData in _enterParams)
            animParamData.Invoke(animator);
    }
    public void InvokeExit(Animator animator)
    {
        foreach (var animParamData in _exitParams)
            animParamData.Invoke(animator);
    }
}
*/
