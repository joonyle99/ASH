using UnityEngine;

/// <summary>
/// 대상이 경계 범위 안에 들어왔는지 판단하는 클래스
/// </summary>
public class CautionEvaluator : Evaluator
{
    public override Collider2D IsTargetWithinRange()
    {
        return base.IsTargetWithinRange();
    }
}