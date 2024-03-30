using UnityEngine;

public class FloatingChaseEvaluator : Evaluator
{
    public Vector3 TargetPosition { get; private set; }

    private void Awake()
    {
        customEvaluationEvent -= SetTarget;
        customEvaluationEvent += SetTarget;
    }

    public override Collider2D IsTargetWithinRange()
    {
        return base.IsTargetWithinRange();
    }

    private void SetTarget(Vector3 targetPoint)
    {
        // 추격 타겟을 설정한다
        TargetPosition = targetPoint;
    }
}
