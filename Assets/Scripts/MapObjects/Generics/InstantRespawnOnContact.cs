using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantRespawnOnContact : TriggerZone
{
    [SerializeField] float _damage = 1;

    public override void OnPlayerEnter(PlayerBehaviour player)
    {
        player.TriggerInstantRespawn(_damage);
    }

}
