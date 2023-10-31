using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenWaveZone : ITriggerZone
{
    [SerializeField] float _windPower;
    [SerializeField] float _windAngleDegrees;
    float _rotation { get { return transform.rotation.eulerAngles.z + _windAngleDegrees + 90; } }
    Vector2 _windDir { get { return new Vector2(Mathf.Cos(_rotation * Mathf.Deg2Rad), Mathf.Sin(_rotation * Mathf.Deg2Rad)).normalized;} }
    public override void OnPlayerEnter(PlayerBehaviour player)
    {
        player.Rigidbody.velocity += (_windDir * _windPower);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(_windDir.x, _windDir.y, 0) * _windPower);
    }
}
