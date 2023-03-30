using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerActivator : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        ITriggerZone triggerZone = collision.GetComponent<ITriggerZone>();
        if (triggerZone != null)
        {
            triggerZone.OnActivatorEnter(this);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        ITriggerZone triggerZone = collision.GetComponent<ITriggerZone>();
        if (triggerZone != null)
        {
            triggerZone.OnActivatorExit(this);
        }
    }
}
