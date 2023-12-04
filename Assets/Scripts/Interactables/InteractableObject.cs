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
public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] Transform _interactionMarkerPoint;
    [SerializeField] bool _isInteractable;
    [SerializeField] InteractionAnimationType _animationType;
    // TODO : 플레이어 상태 및 입력 override 하는 기능

    protected PlayerBehaviour Player { get { return SceneContext.Current.Player; } }
    public InteractionAnimationType AnimationType { get { return _animationType; } }
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

    protected bool IsInteractionKeyUp { get { return InputManager.Instance.InteractionKey.KeyUp; } }
    protected bool IsPlayerStateChanged { get { return !Player.StateIs<InteractionState>(); } }
    protected abstract void OnInteract();
    public abstract void UpdateInteracting();
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