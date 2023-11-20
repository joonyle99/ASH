using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTriggerReport : MonoBehaviour, ITriggerListener
{
    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    {
        Destroy(reporter);
        Destroy(gameObject);
    }
}
