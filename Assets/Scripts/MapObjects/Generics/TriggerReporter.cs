using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TriggerReporter : TriggerZone
{
    [SerializeField] GameObject _reportTargetObject;
    ITriggerListener [] _reportTargets;

    private void OnValidate()
    {
        if (_reportTargetObject != null && _reportTargetObject.GetComponent<ITriggerListener>() == null)
            Debug.LogErrorFormat("Report target object {0} doesn't have a TriggerListener", _reportTargetObject.name);
        if (_reportTargetObject != null && _reportTargetObject.GetComponents<ITriggerListener>().Length > 1)
            Debug.LogWarningFormat("Report target object {0} has multiple TriggerListeners", _reportTargetObject.name);
    }
    private void Awake()
    {
        _reportTargets = _reportTargetObject.GetComponents<ITriggerListener>();
    }

    public override void OnActivatorEnter(TriggerActivator activator) 
    {
        foreach(var target in _reportTargets)
            target.OnEnterReported(activator, this);
    }
    public override void OnActivatorExit(TriggerActivator activator)
    {
        foreach (var target in _reportTargets)
            target.OnExitReported(activator, this);
    }
    public override void OnActivatorStay(TriggerActivator activator)
    {
        foreach (var target in _reportTargets)
            target.OnStayReported(activator, this);
    }

}
