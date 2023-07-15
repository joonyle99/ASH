using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puddle : ITriggerZone
{
    [SerializeField] float _damage = 1;
    [SerializeField] Transform _spawnPoint;
    public override void OnActivatorStay(TriggerActivator activator) 
    {
        if(activator.IsPlayer)
        {
            activator.AsPlayer.OnHitbyWater(_damage, _spawnPoint.position);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 1);
        Gizmos.DrawLine(transform.position, _spawnPoint.position);
    }

}
