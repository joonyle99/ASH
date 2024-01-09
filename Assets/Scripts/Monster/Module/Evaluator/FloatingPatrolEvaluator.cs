using UnityEngine;

public class FloatingPatrolEvaluator : MonoBehaviour
{
    [Header("Floating Patrol Evaluator")]
    [Space]

    [SerializeField] private BoxCollider2D _patrolArea;
    private Bounds _patrolBounds;

    public Vector3 TargetPosition { get; private set; }

    // Test Code
    [SerializeField] private GameObject _checkPrefab;
    private GameObject _patrolTargetPoint;

    void Awake()
    {
        _patrolBounds = _patrolArea.bounds;

        SetTargetPos();
    }

    public void UpdatePatrolPoint()
    {
        if (Vector3.Distance(transform.position, TargetPosition) < 1f)
            SetTargetPos();
    }

    public void SetTargetPos()
    {
        // Delete Debug Object
        if (_patrolTargetPoint)
            Destroy(_patrolTargetPoint);

        TargetPosition = new Vector3(Random.Range(_patrolBounds.min.x, _patrolBounds.max.x),
            Random.Range(_patrolBounds.min.y, _patrolBounds.max.y));

        // Create Debug Object
        _patrolTargetPoint = Instantiate(_checkPrefab, TargetPosition, Quaternion.identity, transform.parent);
        _patrolTargetPoint.name = "Patrol Target Point";
    }

    private void OnDrawGizmosSelected()
    {
        // 순회 범위
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_patrolArea.transform.position, _patrolBounds.size);
    }
}
