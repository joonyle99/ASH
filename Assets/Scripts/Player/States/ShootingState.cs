using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable All

public class ShootingState : PlayerState
{
    [SerializeField] Transform _fireballTransform;          // ���̾ ������ġ

    [SerializeField] float _fireballCoolTime = 2f;          // ���̾ ��Ÿ��
    [SerializeField] float _fireballDelay = 2f;             // ���̾ ������

    [SerializeField] GameObject _fireBullet;                // ���̾ ������Ʈ
    [SerializeField] ParticleSystem _lightingParticle;      // ��¡ ��ƼŬ

    protected override void OnEnter()
    {
        Player.Animator.SetTrigger("Shooting");

        StartCoroutine(ShootingFire());
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnExit()
    {

    }

    private IEnumerator ShootingFire()
    {
        // ��¡ ��ƼŬ ����
        ParticleSystem effect = Instantiate(_lightingParticle, transform.position + new Vector3(0f, 0.5f), Quaternion.identity, transform);
        effect.Play();

        // N�� ��ŭ ������ �߻�
        yield return new WaitForSeconds(_fireballDelay);

        // delete particle system
        effect.Stop();
        Destroy(effect.gameObject);

        // ���̾ ����
        GameObject bullet = Instantiate(_fireBullet, _fireballTransform.transform.position + new Vector3(0.4f * Player.RecentDir, 0.8f), transform.rotation);
        bullet.transform.localScale *= Player.RecentDir;

        // Idle State
        ChangeState<IdleState>();

        yield return null;
    }

}