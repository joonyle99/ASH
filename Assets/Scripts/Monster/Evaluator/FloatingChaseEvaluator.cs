using UnityEngine;

public class FloatingChaseEvaluator : Evaluator
{
    public Vector3 TargetPoint { get; private set; }

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
        // �߰� Ÿ���� �����Ѵ�
        TargetPoint = targetPoint;
    }
}
