using System.Collections;
using UnityEngine;

public class DiveState : PlayerState
{
    [Header("Dive Pre Setting")]

    [Space]

    [SerializeField] LayerMask _enemyLayers;                                            // �� ���̾�
    [SerializeField] ParticleSystem _boomParticle;                                      // ���� ��ƼŬ
    [SerializeField] ParticleSystem _chargingParticle;                                  // ��¡ ��ƼŬ
    [SerializeField] Transform _explosionPoint;                                         // ���� ��ġ

    [Header("Dive Setting")]

    [Space]

    [Range(0f, 30f)] [SerializeField] float _diveSpeed = 15.0f;                         // �������� �ӵ�
    [Range(0f, 30f)] [SerializeField] float _fastDiveSpeed = 10.0f;                     // �������� ���ӵ�
    [Range(0f, 100f)][SerializeField] int _explosionDamage = 40;                        // ���� ������
    [Range(0f, 50f)][SerializeField] float _knockBackPower = 10f;                       // �˹� �Ŀ�
    [Range(0f, 5f)][SerializeField] float _chargingDelay = 2.0f;                        // ��¡ ������
    [SerializeField] Vector3 _explosionSize = new Vector3(5.0f, 1.0f);             // ���� ����
    [SerializeField] Vector3 _chargingParticlePos = new Vector3(0f, 0.5f);         // ��¡ ��ƼŬ ���� ��ġ ����

    Collider2D[] _targetEnemys; // �� �ݶ��̴�
    ParticleSystem _chargingEffect;    // Charging ����Ʈ �ν��Ͻ�

    bool _isCharging = false;          // ��¡ ����
    bool _isDiving = false;            // ���̺� ����

    protected override void OnEnter()
    {
        StartCoroutine(ExcuteDive());
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

            // ������� ���� ���� �� Ž��
            _targetEnemys = Physics2D.OverlapBoxAll(_explosionPoint.position, _explosionSize, 0, _enemyLayers);

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

            // Boom Particle
            // TODO : �ڵ����� �����ȴ�??
            Instantiate(_boomParticle, _explosionPoint.position, Quaternion.identity);

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

        // ��¡ ��ƼŬ ����
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
