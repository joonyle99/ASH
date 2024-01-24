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
        customEvaluation -= ChangeDir;
        customEvaluation += ChangeDir;
    }

    public override Collider2D IsTargetWithinRange()
    {
        return base.IsTargetWithinRange();
    }

    private void ChangeDir(Vector3 targetPoint)
    {
        // 공격 전 최종적으로 방향을 바꾼다.
        int chaseDir = Math.Sign(targetPoint.x - transform.position.x);
        _monster.SetRecentDir(chaseDir);
    }
}
