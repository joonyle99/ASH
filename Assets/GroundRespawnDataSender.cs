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
    /// ground respawn data sender��
    /// patrol point A / patrol point B�� boxCollider2D��
    /// action area info�� ������ �ִ�
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

        // new�� Ŭ���� �Ǵ� ����ü�� ��ü�� �����ϰ� �޸𸮿� �Ҵ��ϱ� ���� ���ȴ�
        // struct�� value type�̱� ������ class�� heap�� �Ҵ�Ǵ� �Ͱ� �޸� stack�� �Ҵ�ȴ�.
        _respawnLine = new Line(_pointA, _pointB);

        // �ν��Ͻ��� �ʱ� ������(��)�� �����Ѵ�
        receiver.SetGroundRespawnData(_patrolPointA.transform.position, _patrolPointB.transform.position, _respawnLine);
    }

    private void OnDrawGizmos()
    {
        if (!_patrolPointA || !_patrolPointB) return;

        // �� Patrol Point ���̿� ���� �ߴ´�
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_respawnLine.pointA, _respawnLine.pointB);
    }
}
