using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkBeam : MonoBehaviour
{
    [SerializeField] ParticleHelper _beamParticle;

    [SerializeField] LayerMask _obstacleLayer;
    [SerializeField] float _rotation;
    [SerializeField] float _maxLength;

    private void Update()
    {
        _beamParticle.SetStartRotation(_rotation);
    }
    void ShootRay()
    {
        float rotation = transform.rotation.eulerAngles.z;
        var hit = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)), _maxLength, _obstacleLayer);
        if (hit.collider == null)
            SetParticleLength(_maxLength);
    }
    void SetParticleLength(float length)
    {
        _beamParticle.SetStartSize(new Vector3(length, 1, 0));
    }
}
