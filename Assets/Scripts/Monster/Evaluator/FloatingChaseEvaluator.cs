using UnityEngine;

public class FloatingChaseEvaluator : Evaluator
{
    public Transform TargetTrans { get; private set; }

    private void Awake()
    {
        customEvaluation -= SetTarget;
        customEvaluation += SetTarget;
    }

    public override Collider2D IsTargetWithinRange()
    {
        return base.IsTargetWithinRange();
    }

    private void SetTarget(Transform trans)
    {
        // 추격 타겟을 설정한다
        TargetTrans = trans;
    }
}
