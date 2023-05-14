using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class InteractableZone : MonoBehaviour, ITriggerZone
{
    InteractableObject _interactableObject;

    void Awake()
    {
        _interactableObject = GetComponentInParent<InteractableObject>();
    }
    public void OnActivatorEnter(TriggerActivator activator)
    {
        if (!activator.IsPlayer)
            return;
        activator.AsPlayer.InteractionController.AddInteractableInRange(_interactableObject);
    }
    public void OnActivatorExit(TriggerActivator activator)
    {
        if (!activator.IsPlayer)
            return;
        activator.AsPlayer.InteractionController.RemoveInteractableInRange(_interactableObject);
    }
}