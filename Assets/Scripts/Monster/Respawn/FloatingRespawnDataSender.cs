using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.AI;

public class FloatingRespawnDataSender : RespawnDataSender
{
    [Header("Floating RespawnData Sender")]
    [Space]

    private NavMeshSurface _navMeshSurface;
    private NavMeshData _navMeshData;

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
    /// float respawn data sender��
    /// patrol area / chase area�� boxCollider2D��
    /// action area info�� ������ �ִµ�, �̸� �����Ѵ�.
    /// </summary>
    /// <param name="boxCollider1">patrol area's boxCollider 2D</param>
    /// <param name="boxCollider2">chase area's boxCollider 2D</param>
    public override void ExtractActionAreaInfo(out BoxCollider2D boxCollider1, out BoxCollider2D boxCollider2)
    {
        boxCollider1 = _patrolArea;
        boxCollider2 = _chaseArea;
    }
    /// <summary>
    /// receiver���� �����ϱ� ���� ������ �����͸� ������Ʈ�Ѵ�
    /// </summary>
    public override void UpdateRespawnData()
    {
        _navMeshData = _navMeshSurface.navMeshData;     // baked action area
        _respawnBounds = _patrolArea.bounds;            // patrolArea�� Ʈ�������� �ٲ����� fixedUpdate�� ��������� ���� �����̱� ������ bounds�� �⺻���� ������ �ִ�

        // �ν��Ͻ��� �ʱ� ������(_navMeshData�� �����ϰ�� ��� ��)�� �����Ѵ�
        receiver.SetFloatingRespawnData(_navMeshData, _patrolArea.transform.position, _chaseArea.transform.position,
            _patrolArea.transform.localScale, _chaseArea.transform.localScale, _respawnBounds);
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
    public void BakeNavMesh()
    {
        // _navMeshSurface.BuildNavMeshAsync();
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
