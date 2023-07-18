using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    PlayerBehaviour _playerBehaviour;
    Animator _animator;
    [SerializeField] Transform _basicAttackHitbox;
    [SerializeField] float _attackCountRefreshTime;

    private int _basicAttackCount = 0;
    private float _timeAfterLastBasicAttack = 0f;

    public bool IsBasicAttacking { get; private set; }

    private void Awake()
    {
        _playerBehaviour = GetComponent<PlayerBehaviour>();
        _animator = GetComponent<Animator>();
    }

    public void CastBasicAttack()
    {
        _basicAttackHitbox.gameObject.SetActive(true);

        _timeAfterLastBasicAttack = 0f;
        //_animator.SetInteger("BasicAttackCount", _basicAttackCount);
        //_animator.SetTrigger("BasicAttack");

        _basicAttackCount++;
        if (_basicAttackCount > 2)
            _basicAttackCount = 0;

        IsBasicAttacking = true;
    }

    public void CastShootingAttack()
    {
        _playerBehaviour.ChangeState<ShootingState>();
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
}
