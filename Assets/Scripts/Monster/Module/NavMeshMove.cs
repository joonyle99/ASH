using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

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

    public void MoveToDestination()
    {
        _agent.SetDestination(_destPosition);
    }

    public void SetStopAgent(bool isStop)
    {
        _agent.isStopped = isStop;
    }
}