using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class InteractMarkerActivationZone : TriggerZone
{
    InteractableObject _interactableObject;

    void Awake()
    {
        _interactableObject = GetComponentInParent<InteractableObject>();
    }
    public override void OnPlayerEnter(PlayerBehaviour player)
    {
        player.InteractionController.AddInteractableInRange(_interactableObject);
    }
    public override void OnPlayerExit(PlayerBehaviour player)
    {
        player.InteractionController.RemoveInteractableInRange(_interactableObject);
    }
}