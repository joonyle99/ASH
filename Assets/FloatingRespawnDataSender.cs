using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.AI;

public class FloatingRespawnDataSender : RespawnDataSender
{
    [Header("FloatingRespawnDataSender")]
    [Space]

    private NavMeshSurface _navMeshSurface;
    private NavMeshData _navMeshData;

    // main
    private BoxCollider2D _patrolArea;
    private BoxCollider2D _chaseArea;

    // by - product
    private Bounds _respawnBounds;

    public void Awake()
    {
        _navMeshSurface = GetComponent<NavMeshSurface>();

        _patrolArea = transform.GetChild(0).GetComponent<BoxCollider2D>();
        _chaseArea = transform.GetChild(1).GetComponent<BoxCollider2D>();
    }
    public void Start()
    {
        UpdateRespawnData();
    }

    /// <summary>
    /// float respawn data sender는
    /// patrol area / chase area의 boxCollider2D를
    /// action area info로 가지고 있다
    /// </summary>
    /// <param name="boxCollider1">patrol area's boxCollider 2D</param>
    /// <param name="boxCollider2">chase area's boxCollider 2D</param>
    public override void ExtractActionAreaInfo(out BoxCollider2D boxCollider1, out BoxCollider2D boxCollider2)
    {
        boxCollider1 = _patrolArea;
        boxCollider2 = _chaseArea;
    }
    /// <summary>
    /// receiver에게 전달하기 위한 리스폰 데이터를 업데이트한다
    /// </summary>
    public override void UpdateRespawnData()
    {
        _navMeshData = _navMeshSurface.navMeshData;
        _respawnBounds = _patrolArea.bounds;            // patrolArea의 트랜스폼을 바꿨지만 fixedUpdate가 실행되지는 않은 상태이기 때문에 bounds가 기본값을 가지고 있다

        // 인스턴스의 초기 데이터(_navMeshData를 제외하고는 모두 값)를 전달한다
        receiver.SetFloatingRespawnData(_navMeshData, _patrolArea.transform.position, _chaseArea.transform.position,
            _patrolArea.transform.localScale, _chaseArea.transform.localScale, _respawnBounds);
    }

    /// <summary>
    /// 새로운 NavMeshSurface에 NavMesh Data를 업데이트한다
    /// </summary>
    /// <param name="navMeshData"></param>
    public void SetNavMeshData(NavMeshData navMeshData)
    {
        _navMeshSurface.UpdateNavMesh(navMeshData);
    }
    /// <summary>
    /// bake navMesh at runtime code
    /// </summary>
    public void BakeNavMesh()
    {
        // _navMeshSurface.BuildNavMeshAsync();
        _navMeshSurface.BuildNavMesh();
    }

    private void OnDrawGizmos()
    {
        if (!_patrolArea || !_chaseArea) return;

        // Patrol Area의 영역을 표시한다
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_respawnBounds.center, _respawnBounds.size);
    }
}
