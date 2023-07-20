using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] PlayerBehaviour _player;
    [SerializeField] Transform _basicAttackHitbox;
    [SerializeField] float _attackCountRefreshTime;

    int _basicAttackCount = 0;
    float _timeAfterLastBasicAttack = 0f;

    public bool IsBasicAttacking { get; private set; }

    private void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
    }

    public void CastBasicAttack()
    {
        _basicAttackHitbox.gameObject.SetActive(true);

        _timeAfterLastBasicAttack = Time.time;
        _basicAttackCount++;

        _player.Animator.SetTrigger("BasicAttack");
        _player.Animator.SetInteger("BasicAttackCount", _basicAttackCount);

        if (_basicAttackCount >= 6)
            _basicAttackCount = 0;

        IsBasicAttacking = true;
    }

    public void CastShootingAttack()
    {
        _player.ChangeState<ShootingState>();
    }

    private void Update()
    {
        // 1초 후 다시 처음으로
        if (Time.time > _timeAfterLastBasicAttack + _attackCountRefreshTime)
            _basicAttackCount = 0;
    }

    public void AnimEvent_FinishBaseAttackAnim()
    {
        IsBasicAttacking = false;

        _basicAttackHitbox.gameObject.SetActive(false);
    }
}
