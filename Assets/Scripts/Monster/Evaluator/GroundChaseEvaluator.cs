using System;
using UnityEngine;

/// <summary>
/// ���� ���Ͱ� ����� �߰� ���� �ȿ� ���Դ��� �ϴ� �Ǵ��ϰ� ������ ��ȯ�ϴ� Ŭ����
/// </summary>
public class GroundChaseEvaluator : Evaluator
{
    [Header("Ground Chase Evaluator")]
    [Space]

    [SerializeField] private int _chaseDir = 1;
    public int ChaseDir
    {
        get => _chaseDir;
        private set => _chaseDir = value;
    }
    [SerializeField] private float _maxChaseDistance = 3f;
    public float MaxChaseDistance
    {
        get => _maxChaseDistance;
        private set => _maxChaseDistance = value;
    }

    [SerializeField] private bool _isChasing;
    public bool IsChasing
    {
        get => _isChasing;
        private set => _isChasing = value;
    }
    [SerializeField] private bool _isTooClose;
    public bool IsTooClose
    {
        get => _isTooClose;
        set => _isTooClose = value;
    }

    public override void Awake()
    {
        base.Awake();

        // Ÿ�� ���� �� �ڵ����� �߻��ϴ� ��� �߰�
        customEvaluationEvent -= SetChaseDir;
        customEvaluationEvent += SetChaseDir;
    }

    public override Collider2D IsTargetWithinRange()
    {
        var hasChaseTarget = base.IsTargetWithinRange();

        if (hasChaseTarget) IsChasing = true;
        else IsChasing = false;

        return hasChaseTarget;
    }

    public void SetChaseDir(Vector3 targetPoint)
    {
        _chaseDir = Math.Sign(targetPoint.x - transform.position.x);
    }
}
