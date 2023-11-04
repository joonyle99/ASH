using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerReporter : TriggerZone
{
    [SerializeField] GameObject _reportTargetObject;
    ITriggerListener _reportTarget;

    private void OnValidate()
    {
        if (_reportTargetObject != null && _reportTargetObject.GetComponent<ITriggerListener>() == null)
            Debug.LogErrorFormat("Report target object {0} doesn't have a TriggerListener", _reportTargetObject.name);
    }
    private void Awake()
    {
        _reportTarget = _reportTargetObject.GetComponent<ITriggerListener>();
    }

    public override void OnActivatorEnter(TriggerActivator activator) 
    {
        _reportTarget.OnEnterReported(activator, this);
    }
    public override void OnActivatorExit(TriggerActivator activator)
    {
        _reportTarget.OnExitReported(activator, this);
    }
    public override void OnActivatorStay(TriggerActivator activator)
    {
        _reportTarget.OnStayReported(activator, this);
    }

}
