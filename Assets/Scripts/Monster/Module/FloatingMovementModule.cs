using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Nav Mesh Agent�� �̿��� Ư���� �̵� ���
/// Monster Movement Module�� �Բ� ������� �ʴ´�
/// </summary>
public class FloatingMovementModule : MonoBehaviour
{
    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    public void SetPosition(Vector3 position)
    {
        _agent.Warp(position);
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
            Debug.LogWarning($"������Ʈ�� NavMesh ���� ����");
            return;
        }

        _agent.isStopped = isStop;
    }
    public void MoveToDestination(Vector3 destPosition)
    {
        _agent.SetDestination(destPosition);
    }
}