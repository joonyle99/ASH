using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTriggerReport : MonoBehaviour, ITriggerListener
{
    [SerializeField] bool _destroyReporter = true;
    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    {
        if(_destroyReporter)
            Destroy(reporter);
        Destroy(gameObject);
    }
}
