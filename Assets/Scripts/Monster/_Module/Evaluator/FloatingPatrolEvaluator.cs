using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPatrolEvaluator : MonoBehaviour
{
    [Header("Floating Patrol Evaluator")]
    [Space]

    [SerializeField] private BoxCollider2D _patrolArea;
    private Bounds _patrolBounds;

    [SerializeField] private Vector3 _targetPosition;
    public Vector3 TargetPosition { get { return _targetPosition; } }

    [SerializeField] private float _targetTime = 7f;
    [SerializeField] private float _elapsedTime;

    // Test Code
    [SerializeField] private GameObject _checkPrefab;
    [SerializeField] private GameObject _patrolTargetPoint;

    void Awake()
    {
        _patrolBounds = _patrolArea.bounds;

        // init
        Init();
    }

    public void UpdatePatrolPoint()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime > _targetTime || Vector3.Distance(transform.position, _targetPosition) < 1f)
        {
            _elapsedTime = 0f;

            // Delete Debug Object
            if (_patrolTargetPoint)
                Destroy(_patrolTargetPoint);

            // Set Destination
            SetTargetPos();

            // Create Debug Object
            _patrolTargetPoint = Instantiate(_patrolTargetPoint, _targetPosition, Quaternion.identity, transform.parent);
            _patrolTargetPoint.name = "Patrol Target Point";
        }
    }

    public void Init()
    {
        SetTargetPos();
        _patrolTargetPoint = Instantiate(_checkPrefab, _targetPosition, Quaternion.identity);
    }

    public void SetTargetPos()
    {
        _targetPosition = new Vector3(Random.Range(_patrolBounds.min.x, _patrolBounds.max.x),
            Random.Range(_patrolBounds.min.y, _patrolBounds.max.y));
    }

    private void OnDrawGizmosSelected()
    {
        // 순회 범위
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_patrolArea.transform.position, _patrolBounds.size);
    }
}
