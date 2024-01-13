using UnityEngine;
using UnityEngine.AI;

public class NavMeshMoveModule : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Vector3 _destPosition;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    public void SetDestination(Transform trans)
    {
        _destPosition = trans.position;
    }
    public void SetDestination(Vector3 pos)
    {
        _destPosition = pos;
    }
    public void MoveToDestination()
    {
        _agent.SetDestination(_destPosition);
    }
    public void SetStopAgent(bool isStop)
    {
        _agent.isStopped = isStop;
    }
}