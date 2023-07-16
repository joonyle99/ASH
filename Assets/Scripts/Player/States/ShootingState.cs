using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable All

public class ShootingState : PlayerState
{
    [SerializeField] Transform _fireballTransform;          // 파이어볼 생성위치

    [SerializeField] float _fireballCoolTime = 2f;          // 파이어볼 쿨타임
    [SerializeField] float _fireballDelay = 2f;             // 파이어볼 딜레이

    [SerializeField] GameObject _fireBullet;                // 파이어볼 오브젝트
    [SerializeField] ParticleSystem _lightingParticle;      // 차징 파티클

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
        // 차징 파티클 생성
        ParticleSystem effect = Instantiate(_lightingParticle, transform.position + new Vector3(0f, 0.5f), Quaternion.identity, transform);
        effect.Play();

        // N초 만큼 딜레이 발생
        yield return new WaitForSeconds(_fireballDelay);

        // delete particle system
        effect.Stop();
        Destroy(effect.gameObject);

        // 파이어볼 생성
        GameObject bullet = Instantiate(_fireBullet, _fireballTransform.transform.position + new Vector3(0.4f * Player.RecentDir, 0.8f), transform.rotation);
        bullet.transform.localScale *= Player.RecentDir;

        // Idle State
        ChangeState<IdleState>();

        yield return null;
    }

}