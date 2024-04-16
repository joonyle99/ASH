using System;
using UnityEngine;

/// <summary>
/// 대상이 공격 범위 안에 들어왔는지 판단하고 방향을 전환하는 클래스
/// </summary>
public class AttackEvaluator : Evaluator
{
    public override void Awake()
    {
        base.Awake();

        EvaluationEvent -= ChangeDir;
        EvaluationEvent += ChangeDir;
    }

    private void ChangeDir(Vector3 targetPoint)
    {
        // 공격 전 최종적으로 방향을 바꾼다.
        int chaseDir = Math.Sign(targetPoint.x - transform.position.x);

        if (monster.monsterData.MoveType == MonsterDefine.MoveType.Ground)
        {
            // monster.SetRecentDir(chaseDir);
            monster.StartSetRecentDirAfterGrounded(chaseDir);
        }
        else
            monster.SetRecentDir(chaseDir);
    }
}
