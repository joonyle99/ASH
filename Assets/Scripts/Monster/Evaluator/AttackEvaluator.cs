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

        evaluationEvent -= ChangeDir;
        evaluationEvent += ChangeDir;
    }

    private void ChangeDir(Vector3 targetPoint)
    {
        // 공격 전 최종적으로 방향을 바꾼다.
        int chaseDir = Math.Sign(targetPoint.x - transform.position.x);

        if (monsterBehavior.monsterData.MoveType == MonsterDefine.MoveType.GroundWalking ||
            monsterBehavior.monsterData.MoveType == MonsterDefine.MoveType.GroundJumpping)
        {
            monsterBehavior.StartSetRecentDirAfterGrounded(chaseDir);
        }
        else
        {
            monsterBehavior.SetRecentDir(chaseDir);
        }
    }
}
