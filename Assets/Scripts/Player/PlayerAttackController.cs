using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    PlayerBehaviour _playerBehaviour;

    int _basicAttackCount;

    [SerializeField] Animator _animator;
    private void Awake()
    {
        _playerBehaviour = GetComponent<PlayerBehaviour>();
    }
    public void CastBasicAttack()
    {
        _playerBehaviour.ChangeState<GroundBasicAttackState>();
        _animator.SetInteger("BasicAttackCount", _basicAttackCount);
        _animator.SetTrigger("BasicAttack");

        _basicAttackCount++;
        if (_basicAttackCount > 1)
            _basicAttackCount = 0;
    }


    public void AnimEvent_FinishBaseAttackAnim()
    {
        // IdleState로 변경
        _playerBehaviour.ChangeState<IdleState>();
    }

    public void RefreshAttackCount()
    {
        _basicAttackCount = 0;
    }
}
