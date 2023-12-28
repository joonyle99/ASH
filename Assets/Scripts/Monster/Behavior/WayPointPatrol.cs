using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointPatrol : MonoBehaviour
{
    [Header("WayPoint Patrol")]
    [Space]

    [SerializeField] private Transform _wayPointBox;
    [SerializeField] private List<Transform> _wayPoints;
    [SerializeField] private Transform _curTargetPosition;
    [SerializeField] private Transform _nextTargetPosition;
    [SerializeField] private int _curWayPointIndex = 0;
    [SerializeField] private float _distanceWithTarget = 0.1f;
    [SerializeField] private float _targetWaitTime = 2f;
    [SerializeField] private float _elapsedWaitTime;
    [SerializeField] private Vector3 _moveDir;
    [SerializeField] private bool _isWaiting;

    public Transform CurTargetPosition
    {
        get { return _curTargetPosition; }
        set { _curTargetPosition = value; }
    }

    public float DistanceWithTarget
    {
        get { return _distanceWithTarget; }
        set { _distanceWithTarget = value; }
    }

    public Vector3 MoveDir
    {
        get { return _moveDir; }
        set { _moveDir = value; }
    }

    public bool IsWaiting
    {
        get { return _isWaiting; }
        set { _isWaiting = value; }
    }

    void Start()
    {
        for (int i = 0; i < _wayPointBox.childCount; ++i)
            _wayPoints.Add(_wayPointBox.GetChild(i));

        // 초기 목적지 설정
        _curTargetPosition = _wayPoints[_curWayPointIndex];
        _nextTargetPosition = _wayPoints[(_curWayPointIndex + 1) % _wayPoints.Count];
    }

    void Update()
    {
        // 대기 상태
        if (_isWaiting)
        {
            _elapsedWaitTime += Time.deltaTime;

            if (_elapsedWaitTime > _targetWaitTime)
            {
                _elapsedWaitTime = 0f;
                _isWaiting = false;
            }
        }
    }

    public void ChangeWayPoint()
    {
        _curWayPointIndex++;
        _curTargetPosition = _nextTargetPosition;
        _nextTargetPosition = _wayPoints[(_curWayPointIndex + 1) % _wayPoints.Count];
    }

    private void OnDrawGizmosSelected()
    {
        // 이동 방향
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(this.transform.position, this.transform.position + _moveDir * 2f);
    }
}
