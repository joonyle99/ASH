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
        customEvaluationEvent -= ChangeDir;
        customEvaluationEvent += ChangeDir;
    }

    public override Collider2D IsTargetWithinRangePlus()
    {
        return base.IsTargetWithinRangePlus();
    }

    private void ChangeDir(Vector3 targetPoint)
    {
        // 공격 전 최종적으로 방향을 바꾼다.
        int chaseDir = Math.Sign(targetPoint.x - transform.position.x);

        if (_monster.MoveType == MonsterDefine.MoveType.GroundWalking || _monster.MoveType == MonsterDefine.MoveType.GroundJumpping)
            _monster.StartSetRecentDirAfterGrounded(chaseDir);
        else
            _monster.SetRecentDir(chaseDir);
    }
}
