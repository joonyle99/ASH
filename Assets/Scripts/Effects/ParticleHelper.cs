using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ParticleHelper : MonoBehaviour
{
    ParticleSystem _particleSystem;
    protected ParticleSystem ParticleSystem { get { return _particleSystem; } }

    protected void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }
    public void SetStartSize(Vector3 size)
    {
        var main = _particleSystem.main;
        main.startSizeX = size.x;
        main.startSizeY = size.y;
        main.startSizeZ = size.z;
    }
    public void SetStartRotation(float rotationZ)
    {
        var main = _particleSystem.main;
        main.startRotation = rotationZ;
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

    public void Emit(int count)
    {
        _particleSystem.Emit(count);
    }

    public void Play()
    {
        _particleSystem.Play();
    }
}
