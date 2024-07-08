using UnityEngine;

/// <summary>
/// Patrol 상태에서 방향 전환이 필요한 상황을 판단하는 클래스
/// </summary>
public class GroundPatrolEvaluator : Evaluator
{
    [Header("─────── Ground Patrol Evaluator ───────")]
    [Space]

    [SerializeField] private Transform _patrolPoints;
    [SerializeField] private bool _isOutOfPatrolRange;

    private Collider2D _leftPoint;
    private Collider2D _rightPoint;

    public override void Awake()
    {
        base.Awake();

        _leftPoint = _patrolPoints.GetChild(0).GetComponent<Collider2D>();
        _rightPoint = _patrolPoints.GetChild(1).GetComponent<Collider2D>();
    }
    public override void OnDisable()
    {
        base.OnDisable();

        _isOutOfPatrolRange = false;
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
