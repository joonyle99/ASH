using System.Collections;
using UnityEngine;

public class DesolateDiveState : PlayerState
{
    [SerializeField] Transform _explosionPoint;         // ���� ��ġ
    [SerializeField] LayerMask _enemyLayers;            // �� ���̾�
    [SerializeField] Collider2D[] _targetEnemys;        // �� �ݶ��̴�
    [SerializeField] ParticleSystem _boomParticle;      // ���� ��ƼŬ
    [SerializeField] ParticleSystem _chargingParticle;  // ��¡ ��ƼŬ

    [SerializeField] float _diveSpeed = 15.0f;          // �������� �ӵ�
    [SerializeField] float _fastDiveSpeed = 10.0f;      // ���� �������� �ӵ�
    [SerializeField] float _explosionSizeX = 5.0f;      // ���� �ʺ�
    [SerializeField] float _explosionSizeY = 1.0f;      // ���� ����
    [SerializeField] int _explosionDamage = 40;         // ���� ������
    [SerializeField] float _knockBackPower = 10f;       // �˹� �Ŀ�

    [SerializeField] bool _isCharging = false;          // ��¡ ����
    [SerializeField] bool _isDiving = false;            // ���̺� ����

    protected override void OnEnter()
    {
        // ChargingDive �ڷ�ƾ ����
        StartCoroutine(ChargingDive());
    }

    protected override void OnUpdate()
    {
        Player.Animator.SetBool("IsCharging", _isCharging);
        Player.Animator.SetBool("IsDiving", _isDiving);

        if (!_isDiving)
            return;

        // ���ӵ� ����1
        Player.Rigidbody.velocity -= Vector2.down * _fastDiveSpeed * Physics2D.gravity.y * Time.deltaTime;

        // ������Ⱑ ������ => ������ & �˹�
        if (Player.IsGrounded)
        {
            // Boom Particle
            Instantiate(_boomParticle, transform.position + new Vector3(0f, 0.5f), Quaternion.identity);

            // ������� ���� ���� �� �Ǻ�
            _targetEnemys = Physics2D.OverlapBoxAll(transform.position, new Vector2(_explosionSizeX, _explosionSizeY), 0, _enemyLayers);

            // �� ����� ���� ��ȸ
            foreach (Collider2D enemy in _targetEnemys)
            {
                float dir = Mathf.Sign(enemy.transform.position.x - transform.position.x);                  // �÷��̾ ���� �ٶ󺸴� ����
                Vector2 knockBackVector = new Vector2(_knockBackPower * dir, _knockBackPower / 2f);       // �˹� ����

                // ���� �������̸�
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

    // �ڷ�ƾ�� ����ؼ� �÷��̾ ���߿��� ���߰� �Ѵ�
    IEnumerator ChargingDive()
    {
        // Start Charging

        _isCharging = true;

        // ������� �ִϸ��̼� ������
        // Player.Animator.SetBool("Dive", true);

        // ���߿� ���ִ� ���·� ����
        Player.Rigidbody.gravityScale = 0;
        Player.Rigidbody.velocity = Vector2.zero;

        // ��¡ ��ƼŬ ����
        ParticleSystem effect = Instantiate(_chargingParticle, transform.position + new Vector3(0f, 0.5f), Quaternion.identity, transform);
        effect.Play();

        yield return new WaitForSeconds(2.0f);

        // End Charging / Start Dive

        // ��¡ ��ƼŬ ����
        effect.Stop();
        Destroy(effect.gameObject);

        // ������⸦ ���� Rigidbody ����
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
