using UnityEngine;

public class FloatingChaseEvaluator : Evaluator
{
    public Vector3 TargetPosition { get; private set; }

    private void Awake()
    {
        customEvaluationEvent -= SetTarget;
        customEvaluationEvent += SetTarget;
    }

    public override Collider2D IsTargetWithinRangePlus()
    {
        return base.IsTargetWithinRangePlus();
    }

    private void SetTarget(Vector3 targetPoint)
    {
        // �߰� Ÿ���� �����Ѵ�
        TargetPosition = targetPoint;
    }
}
