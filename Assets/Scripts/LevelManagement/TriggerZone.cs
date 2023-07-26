using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ITriggerZone : MonoBehaviour
{
    public virtual void OnActivatorEnter(TriggerActivator activator) { }
    public virtual void OnActivatorExit(TriggerActivator activator) { }
    public virtual void OnActivatorStay(TriggerActivator activator) { } 

    void OnTriggerEnter2D(Collider2D other)
    {
        TriggerActivator activator = other.GetComponent<TriggerActivator>();
        if (!other.isTrigger && activator)
        {
            OnActivatorEnter(activator);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        TriggerActivator activator = other.GetComponent<TriggerActivator>();
        if (!other.isTrigger && activator)
        {
            OnActivatorExit(activator);
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        TriggerActivator activator = other.GetComponent<TriggerActivator>();
        if (!other.isTrigger && activator)
        {
            OnActivatorStay(activator);
        }
    }
}
