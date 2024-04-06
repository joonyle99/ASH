using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantRespawnOnContact : TriggerZone
{
    [SerializeField] readonly float _damage = 1;

    public override void OnPlayerEnter(PlayerBehaviour player)
    {
        player.TriggerInstantRespawn(_damage);
    }

    public override void OnActivatorEnter(TriggerActivator activator)
    {
        // 활성화된 오브젝트가 몬스터라면
        if (activator.Type == ActivatorType.Monster)
        {
            // 거북이를 사망시킨다
            var turtle = activator.GetComponent<Turtle>();
            if (turtle)
                turtle.Die();
        }
    }
}
