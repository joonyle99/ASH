using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleRotator : MonoBehaviour
{
    public Transform Target;

    private ParticleSystem _particleSystem;
    private ParticleSystem.Particle[] _particles;

    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
    }

    void LateUpdate()
    {
        if (Target == null) return;

        int aliveParticles = _particleSystem.GetParticles(_particles);

        for (int i = 0; i < aliveParticles; i++)
        {
            Vector3 dir = (Target.position - _particles[i].position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // 각도를 0-360 범위로 조정
            angle = (angle + 360f) % 360f;

            _particles[i].rotation = angle;
        }

        _particleSystem.SetParticles(_particles, aliveParticles);
    }

    public void SetTarget(Transform target)
    {
        Target = target;

        /*
        int aliveParticles = _particleSystem.GetParticles(_particles);

        Debug.Log("==============================================");

        for (int i = 0; i < aliveParticles; i++)
        {
            Vector3 dir = (Target.position - _particles[i].position);
            Vector3 normalDir = dir.normalized;
            float angle = Mathf.Atan2(normalDir.y, normalDir.x) * Mathf.Rad2Deg;

            Debug.DrawRay(_particles[i].position, dir, Color.red, 10f);

            Debug.Log($"<color=orange>Now Rotation</color>: {_particles[i].rotation} <color=yellow>Angle to Player</color>: {angle} / <color=magenta>Particle Position</color>: {_particles[i].position} & <color=cyan>Player Position</color>: {Target.position}");
        }

        Debug.Log("==============================================");
        */
    }
}
