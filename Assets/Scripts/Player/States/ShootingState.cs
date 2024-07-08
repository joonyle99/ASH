using System.Collections;
using UnityEngine;

public class ShootingState : PlayerState
{
    [Header("Shooting Setting")]

    [Space]

    [SerializeField] Transform _shootingTransform;                              // ������ġ
    [SerializeField] ParticleSystem _lightingParticle;                          // ��¡ ��ƼŬ
    [SerializeField] GameObject _bullet;                                        // �Ѿ�

    [Range(0f, 5f)] [SerializeField] private float _shootingCoolTime;           // ��Ÿ��
    [Range(0f, 5f)] [SerializeField] private float _shootingDelay;              // ������

    [SerializeField] private Vector3 _particlePos;
    [Range(0f, 5f)] [SerializeField] private float _bulletPosX;
    [Range(0f, 5f)] [SerializeField] private float _bulletPosY;

    protected override bool OnEnter()
    {
        StartCoroutine(Shooting());

        return true;
    }

    protected override bool OnUpdate()
    {

        return true;
    }
    protected override bool OnFixedUpdate()
    {

        return true;
    }

    protected override bool OnExit()
    {
        Player.Animator.SetBool("IsShooting", false);

        return true;
    }

    /// <summary>
    /// �÷��̾� ���̾ �߻� �Լ�
    /// </summary>
    /// <returns></returns>
    private IEnumerator Shooting()
    {
        Player.Animator.SetTrigger("Shooting");
        Player.Animator.SetBool("IsShooting", true);

        // ��ƼŬ ���� & ����
        ParticleSystem chargingEffect = Instantiate(_lightingParticle, transform.position + _particlePos, Quaternion.identity, transform);
        chargingEffect.Play();  // �ݺ��Ǵ� ����Ʈ

        // �߻� ������
        yield return new WaitForSeconds(_shootingDelay);

        // ��ƼŬ ���� & �ı�
        chargingEffect.Stop();
        Destroy(chargingEffect.gameObject);

        // Bullet ����
        GameObject bullet = Instantiate(_bullet, _shootingTransform.transform.position + new Vector3(_bulletPosX * Player.RecentDir, _bulletPosY), transform.rotation);
        Debug.Log(bullet.gameObject.name + "�� �����Ǿ����ϴ�");

        // TODO : ���߿� �÷��̾ ȸ���� �� ���¿��� �߻��ϸ� y���� ������ �����ؾ��ؼ� ��� scale�� ����
        // �÷��̾ �߻��ϴ� ���⿡ ���� Bullet�� ���⵵ �ٲ��
        Vector3 scale = bullet.transform.localScale;
        scale.x *= Player.RecentDir;
        bullet.transform.localScale = scale;

        // Idle State
        ChangeState<IdleState>();

        yield return null;
    }
}