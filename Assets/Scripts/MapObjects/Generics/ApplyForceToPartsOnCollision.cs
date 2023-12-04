using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyForceToPartsOnCollision : MonoBehaviour
{
    [SerializeField] Rigidbody2D[] _parts;
    [SerializeField] float _multiplier = 1f;
    public void OnCollisionEnter2D(Collision2D collision)
    {
        foreach(var part in _parts)
        {
            part.velocity = collision.otherRigidbody.velocity.normalized * _multiplier;
            
        }
    }
}
