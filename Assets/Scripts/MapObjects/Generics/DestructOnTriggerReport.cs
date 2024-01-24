using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DestructEventCaller))]
public class DestructOnTriggerReport : MonoBehaviour, ITriggerListener
{
    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    {
        if (activator.Type == ActivatorType.Player)
            Destruction.Destruct(gameObject);
    }
}
