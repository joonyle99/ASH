using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossKey : MonoBehaviour, ITriggerListener
{
    [SerializeField] int _ID;
    public int ID => _ID;
    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    {
        if (activator.Type == ActivatorType.Player)
        {
            BossDungeonManager.Instance.OnKeyObtained(this);
            Destruction.Destruct(gameObject);
        }
    }
}
