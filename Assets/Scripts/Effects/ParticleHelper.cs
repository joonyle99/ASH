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

    public float GetTwinkleLifeTime()
    {
        return _particleSystem.main.startLifetime.constantMin + _particleSystem.main.startLifetime.constantMax / 2f;
    }
    public float GetDisintegrateLifeTime()
    {
        // Main Module의 Start Lifetime에서 최대값 가져오기
        var totalDuration = _particleSystem.main.startLifetime.constantMax;

        // Emission 모듈 가져오기
        var emission = _particleSystem.emission;

        // Burst로 인한 추가 지속 시간 계산
        for (int i = 0; i < emission.burstCount; i++)
        {
            var burst = emission.GetBurst(i);
            var burstDuration = burst.time + burst.cycleCount * burst.repeatInterval;
            totalDuration = Mathf.Max(totalDuration, burstDuration + _particleSystem.main.startLifetime.constantMax);
        }

        // Rate Over Time으로 인해 생성된 파티클의 지속 시간 계산
        if (emission.rateOverTime.constant > 0)
        {
            var simulationDuration = _particleSystem.main.duration;
            var rateOverTimeDuration = simulationDuration + _particleSystem.main.startLifetime.constantMax;
            totalDuration = Mathf.Max(totalDuration, rateOverTimeDuration);
        }

        // Debug.Log($"Calculated Total Duration: {totalDuration}");

        /*
        var totalDuration = _particleSystem.main.startLifetime.constantMax;

        var emission = _particleSystem.emission;
        for (int i = 0; i < emission.burstCount; i++)
        {
            var burst = emission.GetBurst(i);
            var burstDuration = burst.time + burst.cycleCount * burst.repeatInterval;
            totalDuration += burstDuration;
        }

        Debug.Log(totalDuration);
        */

        return totalDuration;
    }

    public int GetParticleCount()
    {
        return _particleSystem.particleCount;
    }
}
