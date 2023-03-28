using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class StateMachineBase : MonoBehaviour
{
    [SerializeField] StateBase _initialState;
    [SerializeField] Animator _animator;

    public StateBase CurrentState { get { return _currentState; } }
    public StateBase PreviousState { get { return _previousState; } }
    public Animator Animator { get { return _animator;  } }

    StateBase _currentState;
    StateBase _previousState = null;
    
    Dictionary<Type, Component> _cachedComponents = new Dictionary<Type, Component>();

    protected virtual void Start()
    {
#if UNITY_EDITOR
        if (_initialState == null)
            Debug.LogError(string.Format("Initial state of {0} is missing!", this.gameObject.name));
#endif
        _currentState = _initialState;
        _currentState.TriggerEnter(this);
    }

    protected virtual void Update()
    {
        _currentState.TriggerUpdate();
    }
    public NextState ChangeState<NextState>(bool ignoreSameState = false) where NextState : StateBase
    {
        var nextState = GetComponent<NextState>();
        if (ignoreSameState && nextState == _currentState)
            return nextState;
        _currentState.TriggerExit();
        _previousState = _currentState;
        _currentState = nextState; 
        _currentState.TriggerEnter(this);
        return nextState;
    }
    public bool StateIs<State>() where State:StateBase
    {
        return _currentState is State;
    }
    public PrevState GetPreviousStateAs<PrevState>() where PrevState : StateBase
    {
        if (_previousState is PrevState)
            return _previousState as PrevState;
        return null;
    }

    public new T GetComponent<T>() where T : Component
    {
        if (_cachedComponents.ContainsKey(typeof(T)))
            return _cachedComponents[typeof(T)] as T;

        var component = base.GetComponent<T>();
        if (component != null)
            _cachedComponents.Add(typeof(T), component);
        return component;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(StateMachineBase), true), CanEditMultipleObjects]
public class StateMachineBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {

        StateMachineBase stateMachine = (StateMachineBase)target;
        if (stateMachine.CurrentState == null)
            EditorGUILayout.LabelField("Current State : ", "null");
        else
            EditorGUILayout.LabelField("Current State : ", stateMachine.CurrentState.GetType().Name);
        DrawDefaultInspector();
        Repaint();
    }
}
#endif