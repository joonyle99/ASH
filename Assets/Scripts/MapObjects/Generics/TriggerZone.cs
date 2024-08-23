using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TriggerActivator와의 충돌 감지를 활용한 트리거존이다
/// 상호작용, 씬 전환, 튜토리얼, 등 다양한 종류의 트리거 종류가 존재한다
/// TriggerActivator 컴포넌트를 통해 TriggerZone의 Target이 될 수 있다. (플레이어도 TriggerActivator 컴포넌트를 부착하고 있다)
/// 레이어로 Trigger Zone 설정이 필요하다
/// </summary>
public abstract class TriggerZone : MonoBehaviour
{
    [Header("─────────── TriggerZone ───────────")]
    [Space]

    [SerializeField] protected List<TriggerActivator> triggerActivators;

    public virtual void OnActivatorEnter(TriggerActivator activator) { }
    public virtual void OnActivatorExit(TriggerActivator activator) { }
    public virtual void OnActivatorStay(TriggerActivator activator) { }

    public virtual void OnPlayerEnter(PlayerBehaviour player) { }
    public virtual void OnPlayerExit(PlayerBehaviour player) { }
    public virtual void OnPlayerStay(PlayerBehaviour player) { }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TriggerActivator activator = other.GetComponent<TriggerActivator>();
        if (!other.isTrigger && activator)
        {
            OnActivatorEnter(activator);

            if (!triggerActivators.Contains(activator))
                triggerActivators.Add(activator);

            if (activator.Type == ActivatorType.Player)
                OnPlayerEnter(activator.GetComponent<PlayerBehaviour>());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        TriggerActivator activator = other.GetComponent<TriggerActivator>();
        if (!other.isTrigger && activator)
        {
            OnActivatorExit(activator);

            if (triggerActivators.Contains(activator))
                triggerActivators.Remove(activator);

            if (activator.Type == ActivatorType.Player)
            {
                // Debug.Log("플레이어 나옴");
                OnPlayerExit(SceneContext.Current.Player);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
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
