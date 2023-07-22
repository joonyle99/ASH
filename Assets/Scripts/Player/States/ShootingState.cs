using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingState : PlayerState
{
    [Header("Shooting Setting")]

    [Space]

    [SerializeField] Transform _shootingTransform;                              // ������ġ
    [SerializeField] ParticleSystem _lightingParticle;                          // ��¡ ��ƼŬ
    [SerializeField] GameObject _bullet;                                        // �Ѿ�

    [Range(0f, 5f)] [SerializeField] float _shootingCoolTime = 2f;              // ��Ÿ��
    [Range(0f, 5f)] [SerializeField] float _shootingDelay = 2f;                 // ������

    [SerializeField] Vector3 _particlePos = new Vector3(0f, 1f);
    [Range(0f, 5f)] [SerializeField] float _bulletPosX = 0.4f;
    [Range(0f, 5f)] [SerializeField] float _bulletPosY = 0.8f;

    protected override void OnEnter()
    {
        Player.Animator.SetTrigger("Shooting");

        StartCoroutine(Shooting());
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnExit()
    {

    }

    private IEnumerator Shooting()
    {
        // ��ƼŬ ���� & ����
        ParticleSystem chargingEffect = Instantiate(_lightingParticle, transform.position + _particlePos, Quaternion.identity, transform);
        chargingEffect.Play();  // �ݺ��Ǵ� ����Ʈ

        // �߻� ������
        yield return new WaitForSeconds(_shootingDelay);

        // ��ƼŬ ���� & �ı�
        chargingEffect.Stop();
        Destroy(chargingEffect.gameObject);

        /// �߻�

        // Bullet ����
        GameObject bullet = Instantiate(_bullet, _shootingTransform.transform.position + new Vector3(_bulletPosX * Player.RecentDir, _bulletPosY), transform.rotation);

        // �÷��̾ �߻��ϴ� ���⿡ ���� Bullet�� ���⵵ �ٲ��
        Vector3 scale = bullet.transform.localScale;
        scale.x *= Player.RecentDir;
        bullet.transform.localScale = scale;
        // TODO : ���߿� �÷��̾ ȸ���� �� ���¿��� �߻��ϸ� y���� ������ �����ؾ��ؼ� ��� scale�� ����

        // Idle State
        ChangeState<IdleState>();

        yield return null;
    }

}