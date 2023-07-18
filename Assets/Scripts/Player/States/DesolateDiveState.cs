using System.Collections;
using UnityEngine;

public class DesolateDiveState : PlayerState
{
    [SerializeField] Transform _explosionPoint;         // 폭발 위치
    [SerializeField] LayerMask _enemyLayers;            // 적 레이어
    [SerializeField] Collider2D[] _targetEnemys;        // 적 콜라이더
    [SerializeField] ParticleSystem _boomParticle;      // 폭발 파티클
    [SerializeField] ParticleSystem _chargingParticle;  // 차징 파티클

    [SerializeField] float _diveSpeed = 15.0f;          // 떨어지는 속도
    [SerializeField] float _fastDiveSpeed = 10.0f;      // 빠른 떨어지는 속도
    [SerializeField] float _explosionSizeX = 5.0f;      // 폭발 너비
    [SerializeField] float _explosionSizeY = 1.0f;      // 폭발 높이
    [SerializeField] int _explosionDamage = 40;         // 폭발 데미지
    [SerializeField] float _knockBackPower = 10f;       // 넉백 파워

    [SerializeField] bool _isCharging = false;          // 차징 상태
    [SerializeField] bool _isDiving = false;            // 다이빙 상태

    protected override void OnEnter()
    {
        // ChargingDive 코루틴 실행
        StartCoroutine(ChargingDive());
    }

    protected override void OnUpdate()
    {
        Player.Animator.SetBool("IsCharging", _isCharging);
        Player.Animator.SetBool("IsDiving", _isDiving);

        if (!_isDiving)
            return;

        // 가속도 적용1
        Player.Rigidbody.velocity -= Vector2.down * _fastDiveSpeed * Physics2D.gravity.y * Time.deltaTime;

        // 내려찍기가 끝나면 => 데미지 & 넉백
        if (Player.IsGrounded)
        {
            // Boom Particle
            Instantiate(_boomParticle, transform.position + new Vector3(0f, 0.5f), Quaternion.identity);

            // 내려찍기 범위 내의 적 판별
            _targetEnemys = Physics2D.OverlapBoxAll(transform.position, new Vector2(_explosionSizeX, _explosionSizeY), 0, _enemyLayers);

            // 적 목록을 전부 순회
            foreach (Collider2D enemy in _targetEnemys)
            {
                float dir = Mathf.Sign(enemy.transform.position.x - transform.position.x);                  // 플레이어가 적을 바라보는 방향
                Vector2 knockBackVector = new Vector2(_knockBackPower * dir, _knockBackPower / 2f);       // 넉백 벡터

                // 만약 슬라임이면
                if (enemy.GetComponent<OncologySlime>() != null)
                {

                }

                //Debug.Log(enemy.gameObject.name);
                enemy.GetComponent<OncologySlime>().OnDamage(_explosionDamage);
                enemy.GetComponent<OncologySlime>().KnockBack(knockBackVector);
            }

            ChangeState<IdleState>();
        }
    }

    // 코루틴을 사용해서 플레이어를 공중에서 멈추게 한다
    IEnumerator ChargingDive()
    {
        // Start Charging

        _isCharging = true;

        // 내려찍기 애니메이션 시작점
        // Player.Animator.SetBool("Dive", true);

        // 공중에 떠있는 상태로 고정
        Player.Rigidbody.gravityScale = 0;
        Player.Rigidbody.velocity = Vector2.zero;

        // 차징 파티클 생성
        ParticleSystem effect = Instantiate(_chargingParticle, transform.position + new Vector3(0f, 0.5f), Quaternion.identity, transform);
        effect.Play();

        yield return new WaitForSeconds(2.0f);

        // End Charging / Start Dive

        // 차징 파티클 종료
        effect.Stop();
        Destroy(effect.gameObject);

        // 내려찍기를 위한 Rigidbody 설정
        Player.Rigidbody.gravityScale = 5;
        Player.Rigidbody.velocity = new Vector2(0, -_diveSpeed);

        _isCharging = false;
        _isDiving = true;
    }

    protected override void OnExit()
    {
        //Debug.Log("Exit Desolate Dive");

        // Charging Animation
        // Player.Animator.SetBool("Dive", false);

        _isDiving = false;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.DrawWireCube(transform.position, new Vector2(_explosionSizeX, _explosionSizeY));
    }
}
