using System;
using UnityEngine;

/// <summary>
/// ���� ���Ͱ� ����� �߰� ���� �ȿ� ���Դ��� �ϴ� �Ǵ��ϰ� ������ ��ȯ�ϴ� Ŭ����
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
