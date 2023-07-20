using System.Collections;
using UnityEngine;

public class DiveState : PlayerState
{
    [Header("Prepare Setting")]
    [SerializeField] Collider2D[] _targetEnemys;                                        // 적 콜라이더
    [SerializeField] LayerMask _enemyLayers;                                            // 적 레이어
    [SerializeField] Transform _explosionPoint;                                         // 폭발 위치
    [SerializeField] ParticleSystem _boomParticle;                                      // 폭발 파티클
    [SerializeField] ParticleSystem _chargingParticle;                                  // 차징 파티클

    [Header("DiveHit Setting")]
    [SerializeField] float _diveSpeed = 15.0f;                                          // 떨어지는 속도
    [SerializeField] float _fastDiveSpeed = 10.0f;                                      // 빠른 떨어지는 속도
    [SerializeField] Vector3 _explosionSize = new Vector3(5.0f, 1.0f);             // Boom 크기
    [SerializeField] int _explosionDamage = 40;                                         // 폭발 데미지
    [SerializeField] float _knockBackPower = 10f;                                       // 넉백 파워
    [SerializeField] Vector3 _boomParticlePos = new Vector3(0f, 0.5f);             // Boom 생성 위치
    [SerializeField] Vector3 _chargingParticlePos = new Vector3(0f, 0.5f);         // Charging 생성 위치
    [SerializeField] float _chargingDelay = 2.0f;                                       // Charging 딜레이


    [SerializeField] bool _isCharging = false;          // 차징 상태
    [SerializeField] bool _isDiving = false;            // 다이빙 상태

    protected override void OnEnter()
    {
        // ChargingDive 코루틴 실행
        StartCoroutine(ChargingDive());
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
            // Boom Particle
            Instantiate(_boomParticle, transform.position + _boomParticlePos, Quaternion.identity);

            // 내려찍기 범위 내의 적 판별
            _targetEnemys = Physics2D.OverlapBoxAll(transform.position, _explosionSize, 0, _enemyLayers);

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

            // 내려찍기가 끝나면 Idle State
            ChangeState<IdleState>();
        }
    }

    // 코루틴을 사용해서 플레이어를 공중에서 멈추게 한다
    IEnumerator ChargingDive()
    {
        // 차징 시작
        _isCharging = true;

        // Anim
        Player.Animator.SetBool("IsCharging", true);
        Player.Animator.SetBool("IsDiving", false);

        Player.Rigidbody.gravityScale = 0;
        Player.Rigidbody.velocity = Vector2.zero;

        // 차징 파티클 생성
        // TODO : transform은 부모를 말한다. 해당 파티클은 부모 아래에 Instantiate 되는것을 말한다
        ParticleSystem chargingEffect = Instantiate(_chargingParticle, transform.position + _chargingParticlePos, Quaternion.identity, transform);
        chargingEffect.Play();

        yield return new WaitForSeconds(_chargingDelay);

        // 차징 종료 & 다이브 시작
        chargingEffect.Stop();
        Destroy(chargingEffect.gameObject);

        // 내려찍기를 위한 Rigidbody 설정
        Player.Rigidbody.gravityScale = 5;
        Player.Rigidbody.velocity = new Vector2(0, -_diveSpeed);

        _isCharging = false;
        _isDiving = true;

        Player.Animator.SetBool("IsCharging", false);
        Player.Animator.SetBool("IsDiving", true);
    }

    protected override void OnExit()
    {
        _isDiving = false;

        Player.Animator.SetBool("IsCharging", false);
        Player.Animator.SetBool("IsDiving", false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, _explosionSize);
    }
}
