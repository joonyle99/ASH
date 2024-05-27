using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Nav Mesh Agent를 이용한 특수한 이동 모듈
/// Monster Movement Module와 함께 사용하지 않는다
/// </summary>
public class FloatingMovementModule : MonoBehaviour
{
    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _agent.autoBraking = true;
    }

    public void SetPosition(Vector3 position)
    {
        bool result = _agent.Warp(position);

        if (!result)
        {
            Debug.LogWarning("NavMesh Agent - SetPosition() is failed");
        }
    }
    public void SetSpeed(float speed)
    {
        _agent.speed = speed;
    }
    public void SetAcceleration(float acceleration)
    {
        _agent.acceleration = acceleration;
    }
    public void SetVelocity(Vector3 velocity)
    {
        _agent.velocity = velocity;
    }
    public void SetStopAgent(bool isStop)
    {
        if (!_agent.isOnNavMesh)
        {
            Debug.LogWarning($"NavMesh Agent - isOnNavMesh is failed");
            return;
        }

        _agent.isStopped = isStop;
    }
    public void MoveToDestination(Vector3 destPosition)
    {
        bool result = _agent.SetDestination(destPosition);

        if (!result)
        {
            Debug.LogWarning("NavMesh Agent - SetDestination() is failed");
        }
    }
    public bool CheckArrivedToTarget()
    {
        return _agent.remainingDistance < 0.3f;
    }
}