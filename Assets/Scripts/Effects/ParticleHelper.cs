using UnityEngine;

public class ParticleHelper : MonoBehaviour
{
    ParticleSystem _particleSystem;
    public ParticleSystem ParticleSystem => _particleSystem;

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
    public void PlayAll()
    {
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particle in particles)
        {
            particle.Play();
        }
    }
    public void Play()
    {
        _particleSystem.Play();
    }
    public void Stop()
    {
        _particleSystem.Stop();
    }

    public float GetEmissionLifeTime()
    {
        var totalDuration = (_particleSystem.main.startLifetime.constantMin + _particleSystem.main.startLifetime.constantMax) / 2f;

        /*
        var emission = _particleSystem.emission;
        for (int i = 0; i < emission.burstCount; i++)
        {
            var burst = emission.GetBurst(i);
            var burstDuration = burst.time + burst.cycleCount * burst.repeatInterval;
            totalDuration += burstDuration;
        }
        */

        Debug.Log($"totalDuration: {totalDuration}");

        return totalDuration;
    }
    public int GetParticleCount()
    {
        return _particleSystem.particleCount;
    }
}
