using UnityEngine;

// [ExecuteInEditMode]
public class GroundRespawnDataSender : RespawnDataSender
{
    [Header("GroundRespawnDataSender")]
    [Space]

    // main
    private BoxCollider2D _patrolPointA;
    private BoxCollider2D _patrolPointB;

    // by - product
    private Bounds _boundsOfPointA;
    private Bounds _boundsOfPointB;

    private Vector3 _pointA;
    private Vector3 _pointB;

    private Line _respawnLine;

    public void Awake()
    {
        _patrolPointA = transform.GetChild(0).GetComponent<BoxCollider2D>();
        _patrolPointB = transform.GetChild(1).GetComponent<BoxCollider2D>();
    }
    public void Start()
    {
        UpdateRespawnData();
    }

    /// <summary>
    /// ground respawn data sender는
    /// patrol point A / patrol point B의 boxCollider2D를
    /// action area info로 가지고 있다
    /// </summary>
    /// <param name="boxCollider1">patrol point A's boxCollider2D</param>
    /// <param name="boxCollider2">patrol point B's boxCollider2D</param>
    public override void ExtractActionAreaInfo(out BoxCollider2D boxCollider1, out BoxCollider2D boxCollider2)
    {
        boxCollider1 = _patrolPointA;
        boxCollider2 = _patrolPointB;
    }
    public override void UpdateRespawnData()
    {
        _boundsOfPointA = _patrolPointA.bounds;
        _boundsOfPointB = _patrolPointB.bounds;

        _pointA = _boundsOfPointA.center;
        _pointB = _boundsOfPointB.center;

        // new는 클래스 또는 구조체의 객체를 생성하고 메모리에 할당하기 위해 사용된다
        // struct는 value type이기 때문에 class가 heap에 할당되는 것과 달리 stack에 할당된다.
        _respawnLine = new Line(_pointA, _pointB);

        // 인스턴스의 초기 데이터(값)를 전달한다
        receiver.SetGroundRespawnData(_patrolPointA.transform.position, _patrolPointB.transform.position, _respawnLine);
    }

    private void OnDrawGizmos()
    {
        if (!_patrolPointA || !_patrolPointB) return;

        // 두 Patrol Point 사이에 선을 긋는다
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_respawnLine.pointA, _respawnLine.pointB);
    }
}
