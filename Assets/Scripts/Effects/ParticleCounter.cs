using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleCounter : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private ParticleSystem.EmissionModule _emissionModule;
    private ParticleSystem.TriggerModule _triggerModule;

    private int _initialParticleCount;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _emissionModule = _particleSystem.emission;
        _triggerModule = _particleSystem.trigger;
    }

    private void Start()
    {
        // 초기 파티클 개수 설정
        _initialParticleCount = (int)_emissionModule.GetBurst(0).count.constant;

        // 타겟 콜라이더 세팅하기
        var playerCollider = SceneContext.Current.Player.HeartCollider;
        _triggerModule.SetCollider(0, playerCollider);
    }

    public bool IsGetHalfParticle()
    {
        return _particleSystem.particleCount <= _initialParticleCount / 2;
    }

    public bool IsGetAllOfParticle()
    {
        return _particleSystem.particleCount <= 0;
    }
}
