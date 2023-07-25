using System.Collections;
using UnityEngine;

public class DiveState : PlayerState
{
    [Header("Dive Pre Setting")]

    [Space]

    [SerializeField] LayerMask _enemyLayers;                                            // 적 레이어
    [SerializeField] ParticleSystem _boomParticle;                                      // 폭발 파티클
    [SerializeField] ParticleSystem _chargingParticle;                                  // 차징 파티클
    [SerializeField] Transform _explosionPoint;                                         // 폭발 위치

    [Header("Dive Setting")]

    [Space]

    [Range(0f, 30f)] [SerializeField] float _diveSpeed = 15.0f;                         // 떨어지는 속도
    [Range(0f, 30f)] [SerializeField] float _fastDiveSpeed = 10.0f;                     // 떨어지는 가속도
    [Range(0f, 100f)][SerializeField] int _explosionDamage = 40;                        // 폭발 데미지
    [Range(0f, 50f)][SerializeField] float _knockBackPower = 10f;                       // 넉백 파워
    [Range(0f, 5f)][SerializeField] float _chargingDelay = 2.0f;                        // 차징 딜레이
    [SerializeField] Vector3 _explosionSize = new Vector3(5.0f, 1.0f);             // 폭발 범위
    [SerializeField] Vector3 _chargingParticlePos = new Vector3(0f, 0.5f);         // 차징 파티클 생성 위치 보정

    Collider2D[] _targetEnemys; // 적 콜라이더
    ParticleSystem _chargingEffect;    // Charging 이펙트 인스턴스

    bool _isCharging = false;          // 차징 상태
    bool _isDiving = false;            // 다이빙 상태

    protected override void OnEnter()
    {
        StartCoroutine(ExcuteDive());
    }

    protected override void OnUpdate()
    {
        if (!_isDiving)
            return;

        // 가속도 적용
        Player.Rigidbody.velocity += Vector2.up * _fastDiveSpeed * Physics2D.gravity.y * Time.deltaTime;

        // 내려찍기가 끝나면 => 데미지 & 넉백
        if (Player.IsGrounded)
        {
            _isDiving = false;

            // 내려찍기 범위 내의 적 탐지
            _targetEnemys = Physics2D.OverlapBoxAll(_explosionPoint.position, _explosionSize, 0, _enemyLayers);

            // 적 목록을 전부 순회
            foreach (Collider2D enemy in _targetEnemys)
            {
                float dir = Mathf.Sign(enemy.transform.position.x - transform.position.x);                  // 플레이어가 적을 바라보는 방향
                Vector2 knockBackVector = new Vector2(_knockBackPower * dir, _knockBackPower / 2f);       // 넉백 벡터

                // 만약 슬라임이면
                if (enemy.GetComponent<OncologySlime>() != null)
                {

                }

                enemy.GetComponent<OncologySlime>().OnDamage(_explosionDamage);
                enemy.GetComponent<OncologySlime>().KnockBack(knockBackVector);
            }

            // Boom Particle
            // TODO : 자동으로 삭제된다??
            Instantiate(_boomParticle, _explosionPoint.position, Quaternion.identity);

            // 내려찍기가 끝나면 Idle State
            ChangeState<IdleState>();
        }
    }

    protected override void OnExit()
    {
        Player.Animator.SetBool("IsCharging", false);
        Player.Animator.SetBool("IsDiving", false);
    }

    // 코루틴을 사용해서 플레이어를 공중에서 멈추게 한다
    IEnumerator ExcuteDive()
    {
        Charging();

        yield return new WaitForSeconds(_chargingDelay);

        Dive();
    }

    void Charging()
    {
        _isCharging = true;
        _isDiving = false;

        Player.Animator.SetBool("IsCharging", _isCharging);
        Player.Animator.SetBool("IsDiving", _isDiving);


        Player.Rigidbody.gravityScale = 0;
        Player.Rigidbody.velocity = Vector2.zero;

        // 차징 파티클 생성
        _chargingEffect = Instantiate(_chargingParticle, transform.position + _chargingParticlePos, Quaternion.identity, transform);
        _chargingEffect.Play();
    }

    void Dive()
    {
        _isCharging = false;
        _isDiving = true;

        Player.Animator.SetBool("IsCharging", _isCharging);
        Player.Animator.SetBool("IsDiving", _isDiving);

        Player.Rigidbody.gravityScale = 5;
        Player.Rigidbody.velocity = new Vector2(0, -_diveSpeed);

        _chargingEffect.Stop();
        Destroy(_chargingEffect.gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(transform.position, _explosionSize);
    }
}
