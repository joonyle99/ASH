using System.Collections;
using UnityEngine;

public class DiveState : PlayerState
{
    [Header("Dive Pre Setting")]
    [SerializeField] LayerMask _enemyLayers;                                            // �� ���̾�
    [SerializeField] ParticleSystem _boomParticle;                                      // ���� ��ƼŬ
    [SerializeField] ParticleSystem _chargingParticle;                                  // ��¡ ��ƼŬ
    [SerializeField] Collider2D[] _targetEnemys;                                        // �� �ݶ��̴�
    [SerializeField] Transform _explosionPoint;                                         // ���� ��ġ

    [Header("Dive Setting")]
    [SerializeField] float _diveSpeed = 15.0f;                                          // �������� �ӵ�
    [SerializeField] float _fastDiveSpeed = 10.0f;                                      // ���ӵ�
    [SerializeField] Vector3 _explosionSize = new Vector3(5.0f, 1.0f);             // Boom ũ��
    [SerializeField] int _explosionDamage = 40;                                         // ���� ������
    [SerializeField] float _knockBackPower = 10f;                                       // �˹� �Ŀ�
    [SerializeField] Vector3 _boomParticlePos = new Vector3(0f, 0.5f);             // Boom ���� ��ġ
    [SerializeField] Vector3 _chargingParticlePos = new Vector3(0f, 0.5f);         // Charging ���� ��ġ
    [SerializeField] float _chargingDelay = 2.0f;                                       // Charging ������


    [SerializeField] ParticleSystem _chargingEffect;    // Charging ����Ʈ �ν��Ͻ�
    [SerializeField] bool _isCharging = false;          // ��¡ ����
    [SerializeField] bool _isDiving = false;            // ���̺� ����

    protected override void OnEnter()
    {
        StartCoroutine(ExcutegDive());
    }

    protected override void OnUpdate()
    {
        if (!_isDiving)
            return;

        // ���ӵ� ����
        Player.Rigidbody.velocity += Vector2.up * _fastDiveSpeed * Physics2D.gravity.y * Time.deltaTime;

        // ������Ⱑ ������ => ������ & �˹�
        if (Player.IsGrounded)
        {
            _isDiving = false;

            // Boom Particle
            Instantiate(_boomParticle, transform.position + _boomParticlePos, Quaternion.identity);

            // ������� ���� ���� �� �Ǻ�
            _targetEnemys = Physics2D.OverlapBoxAll(transform.position, _explosionSize, 0, _enemyLayers);

            // �� ����� ���� ��ȸ
            foreach (Collider2D enemy in _targetEnemys)
            {
                float dir = Mathf.Sign(enemy.transform.position.x - transform.position.x);                  // �÷��̾ ���� �ٶ󺸴� ����
                Vector2 knockBackVector = new Vector2(_knockBackPower * dir, _knockBackPower / 2f);       // �˹� ����

                // ���� �������̸�
                if (enemy.GetComponent<OncologySlime>() != null)
                {

                }

                enemy.GetComponent<OncologySlime>().OnDamage(_explosionDamage);
                enemy.GetComponent<OncologySlime>().KnockBack(knockBackVector);
            }

            // ������Ⱑ ������ Idle State
            ChangeState<IdleState>();
        }
    }

    protected override void OnExit()
    {
        Player.Animator.SetBool("IsCharging", false);
        Player.Animator.SetBool("IsDiving", false);
    }

    // �ڷ�ƾ�� ����ؼ� �÷��̾ ���߿��� ���߰� �Ѵ�
    IEnumerator ExcutegDive()
    {
        Charging();

        yield return new WaitForSeconds(_chargingDelay);

        Dive();
    }

    void Charging()
    {
        // ��¡ ����
        _isCharging = true;
        _isDiving = false;

        // Animation Parameter
        Player.Animator.SetBool("IsCharging", _isCharging);
        Player.Animator.SetBool("IsDiving", _isDiving);

        Player.Rigidbody.gravityScale = 0;
        Player.Rigidbody.velocity = Vector2.zero;

        // ��¡ ��ƼŬ ����
        _chargingEffect = Instantiate(_chargingParticle, transform.position + _chargingParticlePos, Quaternion.identity, transform);
        _chargingEffect.Play();
    }

    void Dive()
    {
        // ��¡ ���� & ���̺� ����
        _chargingEffect.Stop();
        Destroy(_chargingEffect.gameObject);

        Player.Rigidbody.gravityScale = 5;
        Player.Rigidbody.velocity = new Vector2(0, -_diveSpeed);

        _isCharging = false;
        _isDiving = true;

        // Animation Parameter
        Player.Animator.SetBool("IsCharging", _isCharging);
        Player.Animator.SetBool("IsDiving", _isDiving);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, _explosionSize);
    }
}
