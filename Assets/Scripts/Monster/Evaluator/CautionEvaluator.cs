using UnityEngine;

/// <summary>
/// 경계 판독기
/// </summary>
public class CautionEvaluator : Evaluator
{
    public override Collider2D IsTargetWithinRangePlus()
    {
        return base.IsTargetWithinRangePlus();
    }
}