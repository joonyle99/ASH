using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingState : PlayerState
{
    [SerializeField] Transform _basicAttackHitbox;

    [SerializeField] float _shootingCoolTime = 2f;
    [SerializeField] float _shootingDelay = 2f;

    [SerializeField] GameObject _fireBullet;
    [SerializeField] ParticleSystem _ShootingDelayEffect;

    float _timeAfterLastShootingAttack;

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
        // Player.Animator.SetTrigger("End Shooting");
    }

    private IEnumerator ShootingFire()
    {
        // create particle system
        ParticleSystem effect = Instantiate(_ShootingDelayEffect, transform.position + new Vector3(0f, 0.5f), Quaternion.identity, transform);
        effect.Play();

        // 1.5초의 딜레이
        yield return new WaitForSeconds(_shootingDelay);

        // delete particle system
        effect.Stop();
        Destroy(effect.gameObject);

        _basicAttackHitbox.gameObject.SetActive(true);

        // 파이어볼 생성
        GameObject bullet = Instantiate(_fireBullet, _basicAttackHitbox.transform.position + new Vector3(0.4f * Player.RecentDir, 0.8f), transform.rotation);
        bullet.transform.localScale *= Player.RecentDir;

        // Idle State
        ChangeState<IdleState>();

        yield return null;
    }

}
