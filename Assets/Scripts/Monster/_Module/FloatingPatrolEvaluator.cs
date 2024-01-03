using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPatrolEvaluator : MonoBehaviour
{
    [Header("Patrol Evaluator")]
    [Space]

    [SerializeField] private BoxCollider2D patrolArea;
    private Bounds _patrolBounds;

    [SerializeField] private Vector3 _targetPosition;
    public Vector3 TargetPosition { get { return _targetPosition; } }

    [SerializeField] private float targetTime = 3f;
    [SerializeField] private float elapsedTime;

    [SerializeField] private GameObject checkPrefab;
    [SerializeField] private GameObject targetPoint;

    void Awake()
    {
        _patrolBounds = patrolArea.bounds;

        // init
        SetTargetPos();
        targetPoint = Instantiate(checkPrefab, _targetPosition, Quaternion.identity);
    }

    public void UpdatePatrolPoint()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= targetTime || Vector3.Distance(transform.position, _targetPosition) < 1f)
        {
            elapsedTime = 0f;

            // Delete Debug Object
            if (targetPoint)
                Destroy(targetPoint);

            // Set Destination
            SetTargetPos();

            // Create Debug Object
            targetPoint = Instantiate(checkPrefab, _targetPosition, Quaternion.identity);
        }
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
        Gizmos.DrawWireCube(patrolArea.transform.position, _patrolBounds.size);
    }
}
