using System;
using UnityEngine;

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
    [SerializeField] private bool _isChasing;
    public bool IsChasing
    {
        get => _isChasing;
        private set => _isChasing = value;
    }

    public Transform TargetTrans { get; private set; }

    private void Awake()
    {
        customEvaluation -= SetTargetTrans;
        customEvaluation += SetTargetTrans;
        customEvaluation -= SetChaseDir;
        customEvaluation += SetChaseDir;
    }

    public override Collider2D IsTargetWithinRange()
    {
        var hasChaseTarget = base.IsTargetWithinRange();

        if (hasChaseTarget) IsChasing = true;
        else IsChasing = false;

        return hasChaseTarget;
    }

    public void SetTargetTrans(Transform trans)
    {
        // 추격 타겟을 설정한다.
        TargetTrans = trans;
    }
    public void SetChaseDir(Transform trans)
    {
        // 추격 방향을 설정한다.
        _chaseDir = Math.Sign(trans.position.x - transform.position.x);
    }
}
