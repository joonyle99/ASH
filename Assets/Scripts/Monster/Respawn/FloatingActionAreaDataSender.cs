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
    /// Patrol Area / Chase Area�� boxCollider2D�� �����Ѵ�.
    /// </summary>
    /// <param name="patrolArea"></param>
    /// <param name="chaseArea"></param>
    public override void ExtractActionAreaInfo(out BoxCollider2D patrolArea, out BoxCollider2D chaseArea)
    {
        patrolArea = _patrolArea;
        chaseArea = _chaseArea;
    }
    /// <summary>
    /// Receiver���� �����ϱ� ���� �ൿ �ݰ� �����͸� ������Ʈ�Ѵ�
    /// </summary>
    public override void UpdateActionAreaData()
    {
        _respawnBounds = _patrolArea.bounds;

        // receiver���� 
        receiver.SetFloatingActionAreaData(_patrolArea.transform.position, _chaseArea.transform.position,
            _patrolArea.transform.localScale, _chaseArea.transform.localScale, _respawnBounds);

        BakeNewNavMesh();
    }

    /// <summary>
    /// ���ο� NavMeshSurface�� NavMesh Data�� ������Ʈ�Ѵ�
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

        // Patrol Area�� ������ ǥ���Ѵ�
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_respawnBounds.center, _respawnBounds.size);
    }
}
