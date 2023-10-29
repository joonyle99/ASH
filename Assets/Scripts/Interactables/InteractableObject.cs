using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] Transform _interactionMarkerPoint;
    [SerializeField] bool _isInteractable;

    public bool IsIsInteractable { get { return _isInteractable; } private set { _isInteractable = value; } }
    public Vector3 InteractionMarkerPoint
    {
        get
        {
            if (_interactionMarkerPoint == null)
                return SceneContext.Current.Player.transform.position;
            return _interactionMarkerPoint.position;
        }
    }
}