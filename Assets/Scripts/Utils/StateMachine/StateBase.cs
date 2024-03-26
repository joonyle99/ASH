using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase : MonoBehaviour
{
    [SerializeField] StateAnimatorParamData _animatorParameters;

    protected StateMachineBase StateMachine { get; set; }
    protected Animator Animator { get => StateMachine.Animator; }

    public void TriggerEnter(StateMachineBase stateMachine)
    {
        StateMachine = stateMachine;
        SetAnimsOnEnter();
        _animatorParameters.InvokeEnter(StateMachine.Animator);
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
        _animatorParameters.InvokeExit(StateMachine.Animator);
        OnExit();
        //StateMachine = null;
    }

    protected virtual void SetAnimsOnEnter() {}
    protected abstract bool OnEnter();
    protected abstract bool OnUpdate();
    protected virtual bool OnFixedUpdate() { return true;}
    protected abstract bool OnExit();

    public NextState ChangeState<NextState>(bool ignoreSameState = false) where NextState : StateBase
    {
        return StateMachine.ChangeState<NextState>(ignoreSameState);
    }

    public bool StateIs<State>() where State : StateBase
    {
        return StateMachine.CurrentState is State;
    }

    //public new T GetComponent<T>() where T : Component
    //{
    //    return StateMachine.GetComponent<T>();
    //}
}

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