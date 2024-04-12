using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class InteractMarkerActivationZone : TriggerZone
{
    [SerializeField] InteractableObject _interactableObject;

    void Awake()
    {
        if(_interactableObject == null )
            _interactableObject = GetComponentInParent<InteractableObject>();
    }
    public override void OnPlayerEnter(PlayerBehaviour player)
    {
        player.PlayerInteractionController.AddInteractableInRange(_interactableObject);
    }
    public override void OnPlayerExit(PlayerBehaviour player)
    {
        player.PlayerInteractionController.RemoveInteractableInRange(_interactableObject);
    }
}