using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantRespawnOnContact : ITriggerZone
{
    [SerializeField] float _damage = 1;

    public override void OnActivatorEnter(TriggerActivator activator) 
    {
        if(activator.IsPlayer)
        {
            activator.AsPlayer.TriggerInstantRespawn(_damage);
        }
    }

}
