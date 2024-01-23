using System.Collections.Generic;
using UnityEngine;

public class GroundPatrolEvaluator : Evaluator
{
    [Header("Ground Patrol Evaluator")]
    [Space]

    [SerializeField] private Transform _patrolPoints;
    [SerializeField] private List<Collider2D> _patrolPointList;
    [SerializeField] private Collider2D _currentTargetPoint;
    [SerializeField] private int _curPointIndex = 0;

    private void Awake()
    {
        foreach (Transform child in _patrolPoints)
        {
            if (child.gameObject.activeSelf)
                _patrolPointList.Add(child.GetComponent<Collider2D>());
        }

        _currentTargetPoint = _patrolPointList[_curPointIndex];
    }

    public override Collider2D IsTargetWithinRange()
    {
        return base.IsTargetWithinRange();
    }

    public bool IsCorrectTargetPoint(Collider2D collider)
    {
        if (collider == _currentTargetPoint)
            return true;

        return false;
    }

    public void SetNextTargetPoint()
    {
        _curPointIndex = (_curPointIndex + 1) % _patrolPointList.Count;
        _currentTargetPoint = _patrolPointList[_curPointIndex];
    }
}
