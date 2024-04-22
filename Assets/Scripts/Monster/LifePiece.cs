using System.Collections;
using UnityEngine;

/// <summary>
/// 생명 조각이 생성되면 퍼졌다가 플레이어에게 빨려들어온다
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
        _velocityOverLifetimeModule.radial = new ParticleSystem.MinMaxCurve(_radialForSlowing); // 실제 값 적용
        _velocityOverLifetimeModule.speedModifier = new ParticleSystem.MinMaxCurve(_speedForSlowing); // 실제 값 적용

        // delay for gathering
        yield return new WaitForSeconds(_waitForGatheringTime);

        // setting gathering position
        TargetToPlayer();

        // * gathering effect *
        _velocityOverLifetimeModule.radial = new ParticleSystem.MinMaxCurve(_radialForGathering); // 실제 값 적용
        _velocityOverLifetimeModule.speedModifier = new ParticleSystem.MinMaxCurve(_speedForGathering); // 실제 값 적용

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
        // 모이는 위치를 플레이어로 변경
        transform.position = _player.BodyCollider.bounds.center;
        transform.parent = _player.transform;
    }

    private void RecoverPlayerHealth()
    {
        // 체력 회복 프로세스
        _player.RecoverHealth(_healingAmount);
    }
}
