using UnityEngine;
using static MonsterBehavior;

[ExecuteInEditMode]
public class MonsterPatrolPoints : MonoBehaviour
{
    [SerializeField]
    private MonsterBehavior _monster;

    private BoxCollider2D _patrolPointA;
    private BoxCollider2D _patrolPointB;

    private Bounds _boundsOfPointA;
    private Bounds _boundsOfPointB;

    private Vector3 _pointA;
    private Vector3 _pointB;

    public void Awake()
    {
        _patrolPointA = transform.GetChild(0).GetComponent<BoxCollider2D>();
        _patrolPointB = transform.GetChild(1).GetComponent<BoxCollider2D>();
    }

    public void Start()
    {
        _boundsOfPointA = _patrolPointA.bounds;
        _boundsOfPointB = _patrolPointB.bounds;

        _pointA = new Vector3(_boundsOfPointA.center.x,
            _boundsOfPointA.center.y - _boundsOfPointA.extents.y / 2f, _boundsOfPointA.center.z);
        _pointB = new Vector3(_boundsOfPointB.center.x,
            _boundsOfPointB.center.y - _boundsOfPointB.extents.y / 2f, _boundsOfPointB.center.z);

        _monster.SetPatrolPoints(_pointA, _pointB);
    }

    private void OnDrawGizmos()
    {
        // 두 Patrol Point 사이에 선을 긋는다
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_pointA, _pointB);
    }
}
