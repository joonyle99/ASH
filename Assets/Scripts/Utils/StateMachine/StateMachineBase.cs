using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class StateMachineBase : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rigidbody;
    [SerializeField] Animator _animator;
    [SerializeField] StateBase _initialState;

    public Rigidbody2D Rigidbody => _rigidbody;
    public Animator Animator => _animator;
    public StateBase CurrentState { get; set; }
    public StateBase PreviousState { get; set; }

    // Dictionary<Type, Component> _cachedComponents = new Dictionary<Type, Component>();

    protected virtual void Start()
    {
#if UNITY_EDITOR
        if (_initialState == null)
            Debug.LogError(string.Format("Initial state of {0} is missing!", this.gameObject.name));
#endif
        CurrentState = _initialState;
        CurrentState.TriggerEnter(this);
    }

    protected virtual void Update()
    {
        CurrentState.TriggerUpdate();
    }

    //protected virtual void FixedUpdate()
    //{
    //    CurrentState.TriggerFixedUpdate();
    //}

    public NextState ChangeState<NextState>(bool ignoreSameState = false) where NextState : StateBase
    {
        var nextState = GetComponent<NextState>();
        if (ignoreSameState && nextState == CurrentState)
            return nextState;
        CurrentState.TriggerExit();
        PreviousState = CurrentState;
        CurrentState = nextState;
        CurrentState.TriggerEnter(this);
        return nextState;
    }

    public bool StateIs<State>() where State : StateBase
    {
        return CurrentState is State;
    }

    public PrevState GetPreviousStateAs<PrevState>() where PrevState : StateBase
    {
        if (PreviousState is PrevState)
            return PreviousState as PrevState;
        return null;
    }

    //public new T GetComponent<T>() where T : Component
    //{
    //    if (_cachedComponents.ContainsKey(typeof(T)))
    //        return _cachedComponents[typeof(T)] as T;

    //    var component = base.GetComponent<T>();
    //    if (component != null)
    //        _cachedComponents.Add(typeof(T), component);
    //    return component;
    //}
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