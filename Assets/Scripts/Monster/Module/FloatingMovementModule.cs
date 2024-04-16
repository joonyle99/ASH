using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Nav Mesh Agent를 이용한 특수한 이동 모듈
/// Monster Movement Module와 함께 사용하지 않는다
/// </summary>
public class FloatingMovementModule : MonoBehaviour
{
    private NavMeshAgent _agent;
    public NavMeshAgent Agent
    {
        get => _agent;
        set => _agent = value;
    }

    private Vector3 destPosition;
    private float speed;
    private float acceleration;
    private Vector3 velocity;
    private bool isStop;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    void Update()
    {
        this.destPosition = _agent.destination;
        this.speed = _agent.speed;
        this.acceleration = _agent.acceleration;
        this.velocity = _agent.velocity;
        this.isStop = _agent.isStopped;
    }

    public void MoveToDestination(Vector3 destPosition)
    {
        _agent.SetDestination(destPosition);
    }
    public void SetPosition(Vector3 position)
    {
        _agent.Warp(position);
        // _agent.nextPosition = position;
        // transform.position = position;
    }
    public void SetPauseUpdatePosition(bool isPause)
    {
        _agent.updatePosition = !isPause;
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
    public void SetStopAgent(bool isStop, bool isZeroVelocity)
    {
        if (isZeroVelocity) _agent.velocity = Vector3.zero;
        _agent.isStopped = isStop;
    }
}