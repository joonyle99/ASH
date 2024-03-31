using UnityEngine;

/// <summary>
/// 공중 몬스터가 대상이 추격 범위 안에 들어왔는지 하는 판단하고 타겟을 설정하는 클래스
/// </summary>
public class FloatingChaseEvaluator : Evaluator
{
    public Vector3 TargetPosition { get; private set; }

    public override void Awake()
    {
        base.Awake();

        // 타겟 감지 시 자동으로 발생하는 기능 추가
        customEvaluationEvent -= SetTarget;
        customEvaluationEvent += SetTarget;
    }

    public override Collider2D IsTargetWithinRange()
    {
        return base.IsTargetWithinRange();
    }

    /// <summary>
    /// NavMesh의 타겟을 설정한다
    /// </summary>
    /// <param name="targetPoint"></param>
    private void SetTarget(Vector3 targetPoint)
    {
        // 추격 타겟을 설정한다
        TargetPosition = targetPoint;
    }
}
