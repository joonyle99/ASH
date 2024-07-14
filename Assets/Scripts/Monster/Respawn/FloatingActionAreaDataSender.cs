using UnityEngine;

public sealed class FloatingActionAreaDataSender : ActionAreaDataSender
{
    [Header("Floating ActionArea Data Sender")]
    [Space]

    [SerializeField] private NavMeshRuntimeBaker _navMeshBuilder;

    private BoxCollider2D _patrolArea;
    private BoxCollider2D _chaseArea;

    protected override void Awake()
    {
        _patrolArea = transform.GetChild(0).GetComponent<BoxCollider2D>();
        _chaseArea = transform.GetChild(1).GetComponent<BoxCollider2D>();

        base.Awake();
    }

    public override void ExtractActionAreaInfo(out BoxCollider2D patrolArea, out BoxCollider2D chaseArea)
    {
        patrolArea = _patrolArea;
        chaseArea = _chaseArea;
    }
    public override void SetActionAreaPosition(Vector3 position1, Vector3 position2)
    {
        _patrolArea.transform.position = position1;
        _chaseArea.transform.position = position2;
    }
    public override void SetActionAreaScale(Vector3 scale1, Vector3 scale2)
    {
        _patrolArea.transform.localScale = scale1;
        _chaseArea.transform.localScale = scale2;
    }

    public override void UpdateActionAreaData()
    {
        // 리스폰 구역 설정
        var _respawnBounds = _patrolArea.bounds;

        Debug.DrawLine(_respawnBounds.center - Vector3.right * _respawnBounds.size.x / 2f, _respawnBounds.center + Vector3.right * _respawnBounds.size.x / 2f, Color.yellow, 3f);
        Debug.DrawLine(_respawnBounds.center - Vector3.up * _respawnBounds.size.y / 2f, _respawnBounds.center + Vector3.up * _respawnBounds.size.y / 2f, Color.yellow, 3f);

        // receiver에게 데이터 전달
        receiver.SetFloatingActionAreaData(_patrolArea.transform.position, _chaseArea.transform.position,
            _patrolArea.transform.localScale, _chaseArea.transform.localScale, _respawnBounds);

        _navMeshBuilder.BakeNewNavMesh();
    }
}
