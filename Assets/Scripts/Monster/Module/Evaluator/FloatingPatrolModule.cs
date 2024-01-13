using UnityEngine;

public class FloatingPatrolModule : MonoBehaviour
{
    [Header("Floating Patrol Module")]
    [Space]

    [SerializeField] private BoxCollider2D _patrolArea;
    public Vector3 TargetPosition { get; private set; }

    // Test Code
    [SerializeField] private GameObject _checkPrefab;
    private GameObject _patrolTargetPoint;

    void Start()
    {
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

        Bounds patrolBounds = _patrolArea.bounds;
        TargetPosition = new Vector3(Random.Range(patrolBounds.min.x, patrolBounds.max.x),
            Random.Range(patrolBounds.min.y, patrolBounds.max.y));

        // Create Debug Object
        _patrolTargetPoint = Instantiate(_checkPrefab, TargetPosition, Quaternion.identity, transform.parent);
        _patrolTargetPoint.name = "Patrol Target Point";
    }
}
