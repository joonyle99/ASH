using System;
using UnityEngine;

/// <summary>
/// 공격 판정기
/// </summary>
public class AttackEvaluator : Evaluator
{
    private MonsterBehavior _monster;

    private void Awake()
    {
        _monster = GetComponent<MonsterBehavior>();
    }

    public override bool IsTargetWithinRange()
    {
        if (IsDuringCoolTime || !IsUsable)
            return false;

        // check target within range
        Collider2D targetCollider = Physics2D.OverlapBox(_checkCollider.transform.position, _checkCollider.bounds.size, 0f, _targetLayer);
        if (targetCollider)
        {
            // check player
            PlayerBehaviour player = targetCollider.GetComponent<PlayerBehaviour>();
            if (player && !player.IsDead)
            {
                // 공격 전 최종적으로 방향을 바꾼다.
                int chaseDir = Math.Sign(player.transform.position.x - transform.position.x);
                _monster.SetRecentDir(chaseDir);

                return true;
            }
        }

        return false;
    }
}
