using System.Collections;
using UnityEngine;

/// <summary>
/// ���� ������ �����Ǹ� �����ٰ� �÷��̾�� �������´�
/// </summary>
public class LifePiece : MonoBehaviour
{
    [SerializeField] private int _healingAmount;

    [Header("Time")]
    [Space]

    [SerializeField] private float _waitForStartTime;
    [SerializeField] private float _waitForGatheringTime;

    [Header("Velocity")]
    [Space]

    [SerializeField] private float _radialForSlowing;
    [SerializeField] private float _speedForSlowing;
    [SerializeField] private float _radialForGathering;
    [SerializeField] private float _speedForGathering;

    [Space]
    
    [Header("Ring Effect")]
    [Space]
    
    [SerializeField] private GameObject _ringStart;
    [SerializeField] private GameObject _ringEnd;

    private ParticleCounter _particleCounter;
    private ParticleSystem _particleSystem;
    private ParticleSystem.VelocityOverLifetimeModule _velocityOverLifetimeModule;

    private PlayerBehaviour _player;

    private void Awake()
    {
        _particleCounter = GetComponentInChildren<ParticleCounter>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }
    private void Start()
    {
        _velocityOverLifetimeModule = _particleSystem.velocityOverLifetime;
        _player = SceneContext.Current.Player;

        StartCoroutine(LifePieceCoroutine());
    }

    private IEnumerator LifePieceCoroutine()
    {
        // delay for starting effect
        yield return new WaitForSeconds(_waitForStartTime);

        // * slowing effect *
        _velocityOverLifetimeModule.radial = new ParticleSystem.MinMaxCurve(_radialForSlowing); // ���� �� ����
        _velocityOverLifetimeModule.speedModifier = new ParticleSystem.MinMaxCurve(_speedForSlowing); // ���� �� ����

        // delay for gathering
        yield return new WaitForSeconds(_waitForGatheringTime);

        // setting gathering position
        TargetToPlayer();

        // * gathering effect *
        _velocityOverLifetimeModule.radial = new ParticleSystem.MinMaxCurve(_radialForGathering); // ���� �� ����
        _velocityOverLifetimeModule.speedModifier = new ParticleSystem.MinMaxCurve(_speedForGathering); // ���� �� ����

        yield return new WaitUntil(() => _particleCounter.IsGetHalfParticle());

        // when you get half of particle, recover health
        RecoverPlayerHealth();

        // * ring start effect *
        _ringStart.SetActive(true);
        yield return new WaitForSeconds(2f);
        _ringStart.SetActive(false);

        // * ring end effect *
        _ringEnd.SetActive(true);
        yield return new WaitForSeconds(2f);
        _ringEnd.SetActive(true);

        Destroy(gameObject);
    }

    private void TargetToPlayer()
    {
        // ���̴� ��ġ�� �÷��̾�� ����
        transform.position = _player.BodyCollider.bounds.center;
        transform.parent = _player.transform;
    }

    private void RecoverPlayerHealth()
    {
        // ü�� ȸ�� ���μ���
        _player.RecoverHealth(_healingAmount);
    }
}
