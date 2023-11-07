using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public enum InteractionType
    {
        Push,
        Pull,
        Talk,
    }

    [SerializeField] Transform _interactionMarkerPoint;
    [SerializeField] bool _isInteractable;

    // TODO : 플레이어 상태 및 입력 override 하는 기능


    public Vector3 InteractionMarkerPoint
    {
        get
        {
            if (_interactionMarkerPoint == null)
                return SceneContext.Current.Player.transform.position;

            return _interactionMarkerPoint.position;
        }
    }
    public bool IsInteractable { get { return _isInteractable; } private set { _isInteractable = value; } }
    public bool IsInteracting { get; private set; }

    protected abstract void OnInteract();
    public abstract void UpdateInteracting();

    public void Interact()
    {
        IsInteracting = true;
        OnInteract();
    }

    public void FinishInteraction()
    {
        IsInteracting = false;
    }
}