using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] Transform _interactionMarkerPoint;

    public Vector3 InteractionMarkerPoint { get { return _interactionMarkerPoint.position; } }

    public abstract void Interact();
}
