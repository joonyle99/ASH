using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Nav Mesh Agent를 이용한 특수한 이동 모듈
/// Monster Movement Module와 함께 사용하지 않는다
/// </summary>
public class NavMeshMovementModule : MonoBehaviour
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
    public void SetSpeed(float speed)
    {
        _agent.speed = speed;
    }
    public void SetAcceleration(float acceleration)
    {
        _agent.acceleration = acceleration;
    }
    public void SetStopAgent(bool isStop, bool isZeroVelocity)
    {
        if (isZeroVelocity) _agent.velocity = Vector3.zero;
        _agent.isStopped = isStop;
    }
}