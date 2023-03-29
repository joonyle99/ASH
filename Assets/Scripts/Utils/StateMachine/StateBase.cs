using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase : MonoBehaviour
{
    [SerializeField] StateAnimatorParamData _animatorParameters;
    StateMachineBase _stateMachine;

    protected Animator Animator { get { return _stateMachine.Animator; } }
    protected StateMachineBase StateMachine { get { return _stateMachine; } }

    public void TriggerEnter(StateMachineBase stateMachine)
    {
        _stateMachine = stateMachine;
        _animatorParameters.InvokeEnter(_stateMachine.Animator);
        OnEnter();
    }
    public void TriggerUpdate()
    {
        OnUpdate();
    }
    public void TriggerExit()
    {
        _animatorParameters.InvokeExit(_stateMachine.Animator);
        OnExit();
        _stateMachine = null;
    }
    public void TriggerFixedUpdate()
    {
        OnFixedUpdate();
    }

    protected abstract void OnEnter();
    protected abstract void OnUpdate();
    protected virtual void OnFixedUpdate() { }
    protected abstract void OnExit();
    public NextState ChangeState<NextState>(bool ignoreSameState = false) where NextState : StateBase
    {
        return _stateMachine.ChangeState<NextState>(ignoreSameState);
    }
    public new T GetComponent<T>() where T : Component
    {
        return _stateMachine.GetComponent<T>();
    }

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