using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// NavMesh Patrol 모듈
/// Monster에 붙혀서 사용
/// </summary>
public class NavMeshMove : MonoBehaviour
{
    [Header("NavMesh Move")]
    [Space]

    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Vector3 _destPosition;

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

    public void SetDestination(Vector3 vec)
    {
        _destPosition = vec;
    }

    public void MoveToTarget()
    {
        _agent.SetDestination(_destPosition);
        // _agent.Move();
    }

    public void SetStopAgent(bool isStop)
    {
        _agent.isStopped = isStop;
    }
}