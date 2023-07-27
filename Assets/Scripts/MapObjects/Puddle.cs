using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puddle : ITriggerZone
{
    [SerializeField] float _damage = 1;
    [SerializeField] float _reviveDelay = 1;
    [SerializeField] Transform _spawnPoint;

    //TEMP
    bool _canKill = true;
    public override void OnActivatorEnter(TriggerActivator activator) 
    {
        //TEMP (cankill)
        if(_canKill && activator.IsPlayer)
        {
            activator.AsPlayer.OnHitbyPuddle(_damage, _spawnPoint.position, _reviveDelay);
            SoundManager.Instance.PlayCommonSFXPitched("SE_Puddle_splash");
            //TEMP
            StartCoroutine(SetKillCoroutine());
        }
    }
    //TEMP
    IEnumerator SetKillCoroutine()
    {
        _canKill = false;
        yield return new WaitForSeconds(0.5f);
        _canKill = true;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 1);
        Gizmos.DrawLine(transform.position, _spawnPoint.position);
    }

}
