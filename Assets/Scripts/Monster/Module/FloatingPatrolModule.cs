using UnityEngine;

public class FloatingPatrolModule : MonoBehaviour
{
    [Header("Floating Patrol Module")]
    [Space]

    [SerializeField] private BoxCollider2D _patrolArea;
    public Vector3 TargetPosition { get; private set; }

    void Awake()
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
        Bounds patrolBounds = _patrolArea.bounds;
        TargetPosition = new Vector3(Random.Range(patrolBounds.min.x, patrolBounds.max.x),
            Random.Range(patrolBounds.min.y, patrolBounds.max.y));
    }
}
