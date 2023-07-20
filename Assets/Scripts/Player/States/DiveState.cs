using System.Collections;
using UnityEngine;

public class DiveState : PlayerState
{
    [Header("Prepare Setting")]
    [SerializeField] Collider2D[] _targetEnemys;                                        // �� �ݶ��̴�
    [SerializeField] LayerMask _enemyLayers;                                            // �� ���̾�
    [SerializeField] Transform _explosionPoint;                                         // ���� ��ġ
    [SerializeField] ParticleSystem _boomParticle;                                      // ���� ��ƼŬ
    [SerializeField] ParticleSystem _chargingParticle;                                  // ��¡ ��ƼŬ

    [Header("DiveHit Setting")]
    [SerializeField] float _diveSpeed = 15.0f;                                          // �������� �ӵ�
    [SerializeField] float _fastDiveSpeed = 10.0f;                                      // ���� �������� �ӵ�
    [SerializeField] Vector3 _explosionSize = new Vector3(5.0f, 1.0f);             // Boom ũ��
    [SerializeField] int _explosionDamage = 40;                                         // ���� ������
    [SerializeField] float _knockBackPower = 10f;                                       // �˹� �Ŀ�
    [SerializeField] Vector3 _boomParticlePos = new Vector3(0f, 0.5f);             // Boom ���� ��ġ
    [SerializeField] Vector3 _chargingParticlePos = new Vector3(0f, 0.5f);         // Charging ���� ��ġ
    [SerializeField] float _chargingDelay = 2.0f;                                       // Charging ������


    [SerializeField] bool _isCharging = false;          // ��¡ ����
    [SerializeField] bool _isDiving = false;            // ���̺� ����

    protected override void OnEnter()
    {
        // ChargingDive �ڷ�ƾ ����
        StartCoroutine(ChargingDive());
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

    // �ڷ�ƾ�� ����ؼ� �÷��̾ ���߿��� ���߰� �Ѵ�
    IEnumerator ChargingDive()
    {
        // ��¡ ����
        _isCharging = true;

        // Anim
        Player.Animator.SetBool("IsCharging", true);
        Player.Animator.SetBool("IsDiving", false);

        Player.Rigidbody.gravityScale = 0;
        Player.Rigidbody.velocity = Vector2.zero;

        // ��¡ ��ƼŬ ����
        // TODO : transform�� �θ� ���Ѵ�. �ش� ��ƼŬ�� �θ� �Ʒ��� Instantiate �Ǵ°��� ���Ѵ�
        ParticleSystem chargingEffect = Instantiate(_chargingParticle, transform.position + _chargingParticlePos, Quaternion.identity, transform);
        chargingEffect.Play();

        yield return new WaitForSeconds(_chargingDelay);

        // ��¡ ���� & ���̺� ����
        chargingEffect.Stop();
        Destroy(chargingEffect.gameObject);

        // ������⸦ ���� Rigidbody ����
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
