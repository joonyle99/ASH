using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class FloatingActionAreaDataSender : ActionAreaDataSender
{
    [Header("Floating ActionArea Data Sender")]
    [Space]

    private NavMeshSurface _navMeshSurface;

    // main
    private BoxCollider2D _patrolArea;
    private BoxCollider2D _chaseArea;

    private Bounds _respawnBounds;      // monster will respawn in this bound (patrol area)

    public void Awake()
    {
        _navMeshSurface = GetComponent<NavMeshSurface>();

        _patrolArea = transform.GetChild(0).GetComponent<BoxCollider2D>();
        _chaseArea = transform.GetChild(1).GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Patrol Area / Chase Area의 boxCollider2D를 추출한다.
    /// </summary>
    /// <param name="patrolArea"></param>
    /// <param name="chaseArea"></param>
    public override void ExtractActionAreaInfo(out BoxCollider2D patrolArea, out BoxCollider2D chaseArea)
    {
        patrolArea = _patrolArea;
        chaseArea = _chaseArea;
    }
    /// <summary>
    /// Receiver에게 전달하기 위한 행동 반경 데이터를 업데이트한다
    /// </summary>
    public override void UpdateActionAreaData()
    {
        _respawnBounds = _patrolArea.bounds;

        // receiver에게 
        receiver.SetFloatingActionAreaData(_patrolArea.transform.position, _chaseArea.transform.position,
            _patrolArea.transform.localScale, _chaseArea.transform.localScale, _respawnBounds);

        BakeNewNavMesh();
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
    public void BakeNewNavMesh()
    {
        // _navMeshSurface.BuildNavMeshAsync();
        _navMeshSurface.RemoveData();
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
