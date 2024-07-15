using UnityEngine;
using NavMeshPlus.Components;
using UnityEngine.AI;

public class NavMeshRuntimeBaker : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private NavMeshSurface _navMeshSurface;

    /// <summary>
    /// bake navMesh at runtime code
    /// </summary>
    public void BakeNewNavMesh()
    {
        // _navMeshSurface.BuildNavMeshAsync();
        _navMeshSurface.RemoveData();
        _navMeshSurface.BuildNavMesh();

        if (!_agent.enabled)
            _agent.enabled = true;
    }
}
