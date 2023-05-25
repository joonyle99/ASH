using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    PlayerBehaviour _playerBehaviour;

    int _basicAttackCount = 0 ;

    [SerializeField] float _attackCountRefreshTime;
    float _timeAfterLastBasicAttack;
    public bool IsBasicAttacking { get; private set; }

    [SerializeField] Animator _animator;
    private void Awake()
    {
        _playerBehaviour = GetComponent<PlayerBehaviour>();
    }
    public void CastBasicAttack()
    {
        _timeAfterLastBasicAttack = 0f;
        _animator.SetInteger("BasicAttackCount", _basicAttackCount);
        _animator.SetTrigger("BasicAttack");

        _basicAttackCount++;
        if (_basicAttackCount > 2)
            _basicAttackCount = 0;

        IsBasicAttacking = true;
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
    }

}
