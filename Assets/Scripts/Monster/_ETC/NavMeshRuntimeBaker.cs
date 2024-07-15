using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using NavMeshPlus.Components;

public class NavMeshRuntimeBaker : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private NavMeshSurface _navMeshSurface;

    /// <summary>
    /// bake navMesh at runtime code
    /// </summary>
    public void BakeNewNavMesh()
    {
        // _navMeshSurface.RemoveData();
        // _navMeshSurface.BuildNavMesh();
        if (!_agent.enabled) _agent.enabled = true;
        // StartCoroutine(BakeNavMeshCoroutine());
    }

    /// <summary>
    /// 비동기 방식으로 NavMesh를 생성하는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator BakeNavMeshCoroutine()
    {
        // Remove existing NavMesh data
        _navMeshSurface.RemoveData();
        Debug.Log("Removed existing NavMesh data");

        // Build new NavMesh
        AsyncOperation asyncOperation = _navMeshSurface.BuildNavMeshAsync();
        while (!asyncOperation.isDone)
        {
            Debug.Log("gogogogogo");
            yield return null;
        }
        Debug.Log("NavMesh building completed");

        // Check if NavMesh was created successfully
        if (NavMesh.CalculateTriangulation().vertices.Length > 0)
        {
            Debug.Log("NavMesh created successfully");

            // Ensure the agent is on a valid NavMesh position
            NavMeshHit hit;
            if (NavMesh.SamplePosition(_agent.transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                _agent.transform.position = hit.position;
                Debug.Log("Agent position adjusted to NavMesh");
            }
            else
            {
                Debug.LogWarning("Failed to find a nearby NavMesh position for the agent");
            }

            // Enable the agent
            if (!_agent.enabled)
            {
                _agent.enabled = true;
                Debug.Log("NavMeshAgent enabled");
            }
        }
        else
        {
            Debug.LogError("Failed to create NavMesh");
        }
    }
}
