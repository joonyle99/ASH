using System.Collections.Generic;
using UnityEngine;

public class GroundPatrolEvaluator : Evaluator
{
    [Header("Ground Patrol Evaluator")]
    [Space]

    [SerializeField] private Transform _patrolPoints;
    [SerializeField] private bool _isOutOfPatrolRange;

    private Collider2D _leftPoint;
    private Collider2D _rightPoint;

    private void Awake()
    {
        _leftPoint = _patrolPoints.GetChild(0).GetComponent<Collider2D>();
        _rightPoint = _patrolPoints.GetChild(1).GetComponent<Collider2D>();
    }

    public override Collider2D IsTargetWithinRange()
    {
        return base.IsTargetWithinRange();
    }

    public bool IsOutOfPatrolRange()
    {
        _isOutOfPatrolRange = IsLeftOfLeftPoint() || IsRightOfRightPoint();
        return _isOutOfPatrolRange;
    }
    public bool IsLeftOfLeftPoint()
    {
        return _leftPoint.transform.position.x > this.transform.position.x;
    }
    public bool IsRightOfRightPoint()
    {
        return _rightPoint.transform.position.x < this.transform.position.x;
    }
}
