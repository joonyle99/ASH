using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionAnimationType
{
    None = 0,

    Push,
    Roll,
    Pull,
}
public enum InteractionStateChangeType
{
    DontChange = 0, 
    InteractionState = 1,
    
}
public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] Transform _interactionMarkerPoint;
    [SerializeField] bool _isInteractable = true;
    [SerializeField] InteractionAnimationType _animationType;
    [SerializeField] InteractionStateChangeType _stateChange = InteractionStateChangeType.InteractionState;
    // TODO : 플레이어 상태 및 입력 override 하는 기능

    protected PlayerBehaviour Player { get { return SceneContext.Current.Player; } }
    public InteractionAnimationType AnimationType { get { return _animationType; } }
    public InteractionStateChangeType StateChange { get { return _stateChange; } }
    public Vector3 InteractionMarkerPoint
    {
        get
        {
            if (_interactionMarkerPoint == null)
                return SceneContext.Current.Player.transform.position;

            return _interactionMarkerPoint.position;
        }
    }
    public bool IsInteractable { get { return _isInteractable; } protected set { _isInteractable = value; } }
    public bool IsInteracting { get; private set; }

    protected bool IsInteractionKeyUp =>  InputManager.Instance.State.InteractionKey.KeyUp;
    protected bool IsPlayerStateChanged { get { return !Player.StateIs<InteractionState>(); } }
    protected abstract void OnInteract();
    public virtual void UpdateInteracting() { }
    public virtual void FixedUpdateInteracting(){}
    protected virtual void OnInteractionExit() { }

    public void Interact()
    {
        IsInteracting = true;
        OnInteract();
    }

    public void OnDestroy()
    {
        if (IsInteracting)
            ExitInteraction();
    }
    public void ExitInteraction()
    {
        IsInteracting = false;
        OnInteractionExit();
        Player.InteractionController.OnInteractionExit();
    }
}