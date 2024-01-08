using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WayPoint Patrol 모듈
/// Monster에 붙혀서 사용
/// Way Point를 방문한다.
/// </summary>
public class WayPointPatrolEvaluator : MonoBehaviour
{
    [Header("WayPoint Patrol")]
    [Space]

    [SerializeField] private Transform _wayPointBox;
    [SerializeField] private List<Transform> _wayPoints;
    [SerializeField] private Transform _curTargetPosition;
    [SerializeField] private Transform _nextTargetPosition;
    [SerializeField] private int _curWayPointIndex = 0;
    [SerializeField] private float _targetDistance = 0.1f;
    [SerializeField] private float _targetWaitTime = 2f;
    [SerializeField] private Vector3 _moveDir;
    [SerializeField] private bool _isWaiting;

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

    void Awake()
    {
        // Way Point를 List에 추가
        for (int i = 0; i < _wayPointBox.childCount; ++i)
        {
            Transform wayPoint = _wayPointBox.GetChild(i);
            if (wayPoint.gameObject.activeSelf)
                _wayPoints.Add(wayPoint);
        }

        // 초기 목적지 설정
        _curTargetPosition = _wayPoints[_curWayPointIndex];
        _nextTargetPosition = _wayPoints[(_curWayPointIndex + 1) % _wayPoints.Count];
    }

    public void SetMoveDir()
    {
        // 이동 방향 설정
        _moveDir = (_curTargetPosition.position - transform.position).normalized;
    }

    public bool IsArrived()
    {
        // 목적지에 도착여부 확인
        if (Vector3.Distance(_curTargetPosition.position,
                transform.position) < _targetDistance)
            return true;
        else
            return false;
    }

    public void ChangeWayPoint()
    {
        // 목적지 변경
        _curWayPointIndex++;
        _curTargetPosition = _nextTargetPosition;
        _nextTargetPosition = _wayPoints[(_curWayPointIndex + 1) % _wayPoints.Count];
    }

    private IEnumerator WaitingTimer()
    {
        _isWaiting = true;
        yield return new WaitForSeconds(_targetWaitTime);
        _isWaiting = false;
    }

    public void StartWaitingTimer()
    {
        StartCoroutine(WaitingTimer());
    }

    private void OnDrawGizmosSelected()
    {
        // 이동 방향
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(this.transform.position, this.transform.position + _moveDir * 2f);
    }
}
