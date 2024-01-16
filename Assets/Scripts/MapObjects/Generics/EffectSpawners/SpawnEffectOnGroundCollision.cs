using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnEffectOnGroundCollision : MonoBehaviour
{
    [SerializeField] bool _playOnce = false;
    [SerializeField] float _velocity;
    [SerializeField] bool _logVelocity = false;
    [SerializeField] ParticleHelper _effectPrefab;


    bool _played = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_playOnce && _played)
            return;
        // Å« µ¹ÀÌ¶û Ãæµ¹ ½Ã ¾²·¯Áü
        var otherObject = collision.gameObject.GetComponent<ASHObject>();

        if (otherObject != null && otherObject.Type == ObjectType.Ground)
        {
            if (_logVelocity)
                Debug.Log("Collision with " + otherObject.Type + ": " + collision.relativeVelocity.magnitude);
            if (collision.relativeVelocity.sqrMagnitude > _velocity * _velocity)
            {
                var angle = Mathf.Atan2(collision.contacts[0].normal.y, collision.contacts[0].normal.x);
                var effect = Instantiate(_effectPrefab, collision.contacts[0].point, Quaternion.Euler(0, 0, angle));
                _played = true;
            }
        }
    }
}
