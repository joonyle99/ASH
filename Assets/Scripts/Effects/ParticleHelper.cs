using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHelper : MonoBehaviour
{
    ParticleSystem _particle;
    void Awake()
    {
    }
    void Init()
    {
        if (_particle == null)
            _particle = GetComponent<ParticleSystem>();
    }
    public void Activate()
    {
        gameObject.SetActive(true);
    }
    public void SetEmisionRotation(Vector3 rotation)
    {
        Init();
        var shape = _particle.shape;
        shape.rotation = rotation;
    }
}
