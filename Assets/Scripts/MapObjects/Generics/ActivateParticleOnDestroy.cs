using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateParticleOnDestroy : MonoBehaviour
{
    [SerializeField] bool _resetRotation = true;
    ParticleHelper[] _particles;
    private void Awake()
    {
        _particles = GetComponentsInChildren<ParticleHelper>(true);
    }
    public void SetEmisionRotations(Vector3 worldRotation)
    {
        foreach (ParticleHelper particle in _particles)
            particle.SetEmisionRotation(worldRotation);
    }
    void OnDestroy()
    {
        foreach (ParticleHelper particle in _particles)
        {
            particle.Activate();
            particle.transform.parent = null;
            particle.transform.localScale = Vector3.one;
            if(_resetRotation)
                particle.transform.rotation = Quaternion.identity;
        }
    }

}
