using UnityEngine;

public class BossKey : MonoBehaviour, ITriggerListener
{
    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    {
        if (activator.Type == ActivatorType.Player)
        {
            BossDungeonManager.Instance.OnKeyObtained(this);
            Destruction.Destruct(gameObject);
        }
    }
}
