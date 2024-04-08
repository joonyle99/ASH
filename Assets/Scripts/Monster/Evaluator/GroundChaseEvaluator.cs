using System;
using UnityEngine;

/// <summary>
/// 지상 몬스터가 대상이 추격 범위 안에 들어왔는지 하는 판단하고 방향을 전환하는 클래스
/// </summary>
public class GroundChaseEvaluator : Evaluator
{
    [field: Header("Ground Chase Evaluator")]
    [field: Space]
    [field: SerializeField]
    public int ChaseDir
    {
        get;
        private set;
    } = 1;
    [field: SerializeField]
    public float MaxChaseDistance
    {
        get;
        private set;
    } = 3;
    [field: SerializeField]
    public bool IsChasing
    {
        get;
        private set;
    }
    [field: SerializeField]
    public bool IsTooClose
    {
        get;
        set;
    }

    public override void Awake()
    {
        base.Awake();

        evaluationEvent -= SetChaseDir;
        evaluationEvent += SetChaseDir;
    }

    public override Collider2D IsTargetWithinRange()
    {
        var hasChaseTarget = base.IsTargetWithinRange();

        IsChasing = hasChaseTarget;
        return hasChaseTarget;
    }

    public void SetChaseDir(Vector3 targetPoint)
    {
        ChaseDir = Math.Sign(targetPoint.x - transform.position.x);
    }
}
