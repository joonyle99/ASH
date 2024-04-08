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

        evaluationEvent -= ChangeTarget;
        evaluationEvent += ChangeTarget;
    }

    /// <summary>
    /// NavMesh의 타겟을 설정한다
    /// </summary>
    /// <param name="targetPoint"></param>
    private void ChangeTarget(Vector3 targetPoint)
    {
        // 추격 타겟을 설정한다
        TargetPosition = targetPoint;
    }
}
