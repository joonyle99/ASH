using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class StateMachineBase : MonoBehaviour
{
    [Header("StateMachineBase")]
    [Space]

    [SerializeField] private Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody
    {
        get => _rigidbody;
        private set => _rigidbody = value;
    }
    [SerializeField] private Animator _animator;
    public Animator Animator
    {
        get => _animator;
        private set => _animator = value;
    }

    [SerializeField] StateBase _initialState;
    public StateBase CurrentState { get; private set; }
    public StateBase PreviousState { get; private set; }

    protected virtual void Awake()
    {

    }
    protected virtual void Start()
    {
#if UNITY_EDITOR
        if (_initialState == null)
            Debug.LogError($"Initial state of {this.gameObject.name} is missing!");
#endif
        CurrentState = _initialState;
        CurrentState.TriggerEnter(this);
    }
    protected virtual void Update()
    {
        CurrentState.TriggerUpdate();
    }
    protected virtual void FixedUpdate()
    {
        CurrentState.TriggerFixedUpdate();
    }

    public TState ChangeState<TState>(bool ignoreSameState = false) where TState : StateBase
    {
        var nextState = GetComponent<TState>();
        if (ignoreSameState && (nextState == CurrentState))
            return nextState;

        CurrentState.TriggerExit();

        PreviousState = CurrentState;
        CurrentState = nextState;

        CurrentState.TriggerEnter(this);

        return nextState;
    }
    public bool CurrentStateIs<TState>() where TState : StateBase
    {
        return CurrentState is TState;
    }
    public bool PreviousStateIs<TState>() where TState : StateBase
    {
        return PreviousState is TState;
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