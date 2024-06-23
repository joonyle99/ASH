using System.Collections;
using UnityEngine;

public class ShootingState : PlayerState
{
    [Header("Shooting Setting")]

    [Space]

    [SerializeField] Transform _shootingTransform;                              // 생성위치
    [SerializeField] ParticleSystem _lightingParticle;                          // 차징 파티클
    [SerializeField] GameObject _bullet;                                        // 총알

    [Range(0f, 5f)] [SerializeField] private float _shootingCoolTime;           // 쿨타임
    [Range(0f, 5f)] [SerializeField] private float _shootingDelay;              // 딜레이

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
    /// 플레이어 파이어볼 발사 함수
    /// </summary>
    /// <returns></returns>
    private IEnumerator Shooting()
    {
        Player.Animator.SetTrigger("Shooting");
        Player.Animator.SetBool("IsShooting", true);

        // 파티클 생성 & 시작
        ParticleSystem chargingEffect = Instantiate(_lightingParticle, transform.position + _particlePos, Quaternion.identity, transform);
        chargingEffect.Play();  // 반복되는 이펙트

        // 발사 딜레이
        yield return new WaitForSeconds(_shootingDelay);

        // 파티클 종료 & 파괴
        chargingEffect.Stop();
        Destroy(chargingEffect.gameObject);

        // Bullet 생성
        GameObject bullet = Instantiate(_bullet, _shootingTransform.transform.position + new Vector3(_bulletPosX * Player.RecentDir, _bulletPosY), transform.rotation);
        Debug.Log(bullet.gameObject.name + "이 생성되었습니다");

        // TODO : 나중에 플레이어가 회전을 한 상태에서 발사하면 y축의 반전도 생각해야해서 모든 scale을 반전
        // 플레이어가 발사하는 방향에 따라 Bullet의 방향도 바뀐다
        Vector3 scale = bullet.transform.localScale;
        scale.x *= Player.RecentDir;
        bullet.transform.localScale = scale;

        // Idle State
        ChangeState<IdleState>();

        yield return null;
    }
}