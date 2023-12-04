using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ParticleHelper : MonoBehaviour
{
    ParticleSystem _particleSystem;
    protected ParticleSystem ParticleSystem { get { return _particleSystem; } }

    protected void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }
    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void SetEmissionRotation(Vector3 rotation)
    {
        if (_particleSystem == null)
            _particleSystem = GetComponent<ParticleSystem>();
        var shape = _particleSystem.shape;
        shape.rotation = rotation;
    }
    public void AddEmissionRotation(float degree)
    {
        if (_particleSystem == null)
            _particleSystem = GetComponent<ParticleSystem>();
        var shape = _particleSystem.shape;
        shape.rotation = new Vector3(shape.rotation.x, shape.rotation.y, shape.rotation.z + degree);
    }

}
