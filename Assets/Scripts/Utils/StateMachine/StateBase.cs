using System.Collections;
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
        StateMachine = null;
    }

    protected virtual void SetAnimsOnEnter() {}
    protected abstract void OnEnter();
    protected abstract void OnUpdate();
    protected abstract void OnFixedUpdate();
    protected abstract void OnExit();
    public NextState ChangeState<NextState>(bool ignoreSameState = false) where NextState : StateBase
    {
        return StateMachine.ChangeState<NextState>(ignoreSameState);
    }
    public new T GetComponent<T>() where T : Component
    {
        return StateMachine.GetComponent<T>();
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