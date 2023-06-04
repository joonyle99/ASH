using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    PlayerBehaviour _playerBehaviour;

    int _basicAttackCount = 0 ;

    [SerializeField] float _attackCountRefreshTime;
    [SerializeField] float _shootingCoolTime;
    [SerializeField] float _shootingDelay;
    [SerializeField] Transform _basicAttackHitbox;
    [SerializeField] GameObject _fireBullet;
    [SerializeField] ParticleSystem _ShootingDelayEffect;

    float _timeAfterLastBasicAttack;
    float _timeAfterLastShootingAttack;

    public bool IsBasicAttacking { get; private set; }
    public bool IsShootingAttacking { get; private set; }

    [SerializeField] Animator _animator;
    private void Awake()
    {
        _playerBehaviour = GetComponent<PlayerBehaviour>();
    }

    public void CastBasicAttack()
    {
        _basicAttackHitbox.gameObject.SetActive(true);

        _timeAfterLastBasicAttack = 0f;
        _animator.SetInteger("BasicAttackCount", _basicAttackCount);
        _animator.SetTrigger("BasicAttack");

        _basicAttackCount++;
        if (_basicAttackCount > 2)
            _basicAttackCount = 0;

        IsBasicAttacking = true;
    }

    public void CastShootingAttack()
    {
        if (!IsShootingAttacking)
        {
            StartCoroutine(ShootingFire());
        }
    }

    private void Update()
    {
        if (_timeAfterLastBasicAttack < _attackCountRefreshTime)
        {
            _timeAfterLastBasicAttack += Time.deltaTime;
            if (_timeAfterLastBasicAttack > _attackCountRefreshTime)
                _basicAttackCount = 0;
        }
    }
    public void AnimEvent_FinishBaseAttackAnim()
    {
        IsBasicAttacking = false;

        _basicAttackHitbox.gameObject.SetActive(false);
    }

    private IEnumerator ShootingFire()
    {
        IsShootingAttacking = true;

        // create particle system
        ParticleSystem effect = Instantiate(_ShootingDelayEffect, transform.position + new Vector3(0f, 0.5f), Quaternion.identity, transform);
        effect.Play();

        // 1.5초의 딜레이
        // 이 동안은 움직일 수 없어야 한다.
        yield return new WaitForSeconds(1.5f);

        // delete particle system
        effect.Stop();
        Destroy(effect.gameObject);

        _basicAttackHitbox.gameObject.SetActive(true);

        //_timeAfterLastBasicAttack = 0f;
        //_animator.SetInteger("BasicAttackCount", _basicAttackCount);
        //_animator.SetTrigger("BasicAttack");

        GameObject bullet = Instantiate(_fireBullet, _basicAttackHitbox.transform.position + new Vector3(0.4f * _playerBehaviour.RecentDir, 0.8f), transform.rotation);
        bullet.transform.localScale *= _playerBehaviour.RecentDir;

        IsShootingAttacking = false;

        yield return null;
    }
}
