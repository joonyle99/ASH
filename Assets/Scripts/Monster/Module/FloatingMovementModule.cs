using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Nav Mesh Agent를 이용한 특수한 이동 모듈
/// Monster Movement Module와 함께 사용하지 않는다
/// </summary>
public class FloatingMovementModule : MonoBehaviour
{
    private NavMeshAgent _agent;
    public NavMeshAgent Agent => _agent;
    public bool IsOnNavMesh => _agent.isOnNavMesh;

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
        //Debug.Log($"target velocity: {velocity}");

        _agent.velocity = velocity;
    }
    public void SetStopAgent(bool isStop)
    {
        //Debug.Log($"isStop : {isStop}");

        _agent.isStopped = isStop;
    }
    public void SetPosition(Vector3 position)
    {
        if (!IsOnNavMesh) return;

        bool result = _agent.Warp(position);

        if (!result)
        {
            Debug.LogWarning("NavMesh Agent - SetPosition() is failed");
        }
    }
    public void MoveToDestination(Vector3 destPosition)
    {
        if (!IsOnNavMesh) return;

        bool result = _agent.SetDestination(destPosition);

        if (!result)
        {
            Debug.LogWarning("NavMesh Agent - SetDestination() is failed");
        }
    }
    public bool CheckArrivedToTarget()
    {
        if (!IsOnNavMesh) return false;

        return _agent.remainingDistance < 0.3f;
    }
}