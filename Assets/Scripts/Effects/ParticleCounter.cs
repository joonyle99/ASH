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
        // �ʱ� ��ƼŬ ���� ����
        _initialParticleCount = (int)_emissionModule.GetBurst(0).count.constant;

        // Ÿ�� �ݶ��̴� �����ϱ�
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
