using UnityEngine;
using UnityEngine.AI;

public class NavMeshMoveModule : MonoBehaviour
{
    private NavMeshAgent _agent;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    public void MoveToDestination(Vector3 destPosition)
    {
        _agent.SetDestination(destPosition);
    }
    public void SetStopAgent(bool isStop, bool isZeroVelocity)
    {
        if (isZeroVelocity) _agent.velocity = Vector3.zero;
        _agent.isStopped = isStop;
    }
}