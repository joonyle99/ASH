using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public virtual void OnActivatorEnter(TriggerActivator activator) { }
    public virtual void OnActivatorExit(TriggerActivator activator) { }
    public virtual void OnActivatorStay(TriggerActivator activator) { }

    public virtual void OnPlayerEnter(PlayerBehaviour player) { }
    public virtual void OnPlayerExit(PlayerBehaviour player) { }
    public virtual void OnPlayerStay(PlayerBehaviour player) { }

    void OnTriggerEnter2D(Collider2D other)
    {
        TriggerActivator activator = other.GetComponent<TriggerActivator>();
        if (!other.isTrigger && activator)
        {
            OnActivatorEnter(activator);
            if (activator.Type == ActivatorType.Player)
                OnPlayerEnter(SceneContext.Current.Player);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        TriggerActivator activator = other.GetComponent<TriggerActivator>();
        if (!other.isTrigger && activator)
        {
            OnActivatorExit(activator);
            if (activator.Type == ActivatorType.Player)
                OnPlayerExit(SceneContext.Current.Player);
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        TriggerActivator activator = other.GetComponent<TriggerActivator>();
        if (!other.isTrigger && activator)
        {
            OnActivatorStay(activator);
            if (activator.Type == ActivatorType.Player)
                OnPlayerStay(SceneContext.Current.Player);
        }
    }
}
