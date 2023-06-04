using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class InteractableZone : ITriggerZone
{
    InteractableObject _interactableObject;

    void Awake()
    {
        _interactableObject = GetComponentInParent<InteractableObject>();
    }
    public override void OnActivatorEnter(TriggerActivator activator)
    {
        if (!activator.IsPlayer)
            return;
        activator.AsPlayer.InteractionController.AddInteractableInRange(_interactableObject);
    }
    public override void OnActivatorExit(TriggerActivator activator)
    {
        if (!activator.IsPlayer)
            return;
        activator.AsPlayer.InteractionController.RemoveInteractableInRange(_interactableObject);
    }
}