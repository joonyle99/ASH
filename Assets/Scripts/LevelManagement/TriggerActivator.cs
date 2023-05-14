using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerActivator : MonoBehaviour
{
    List<ITriggerZone> activatedTriggers = new List<ITriggerZone>();
    public bool IsPlayer { get; private set; }
    public PlayerBehaviour AsPlayer { get { return _playerComponent; } }

    PlayerBehaviour _playerComponent;
    private void Awake()
    {
        _playerComponent = GetComponent<PlayerBehaviour>();
        if (_playerComponent != null)
            IsPlayer = true;
        else
            IsPlayer = false;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        ITriggerZone triggerZone = collision.GetComponent<ITriggerZone>();
        if (triggerZone != null && !activatedTriggers.Contains(triggerZone))
        {
            activatedTriggers.Add(triggerZone);
            triggerZone.OnActivatorEnter(this);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        ITriggerZone triggerZone = collision.GetComponent<ITriggerZone>();
        if (triggerZone != null && activatedTriggers.Contains(triggerZone))
        {
            activatedTriggers.Remove(triggerZone);
            triggerZone.OnActivatorExit(this);
        }
    }
}
