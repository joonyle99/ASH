using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointPatrolEvaluator : MonoBehaviour
{
    [Header("WayPoint Patrol Evaluator")]
    [Space]

    [SerializeField] private Transform _wayPointBox;
    [SerializeField] private float _targetWaitTime = 2f;
    [SerializeField] private bool _isWaiting;
    public bool IsWaiting
    {
        get { return _isWaiting; }
        set { _isWaiting = value; }
    }

    private List<Transform> _wayPoints;
    private Transform _curTargetPosition;
    private Transform _nextTargetPosition;
    private int _curWayPointIndex;
    private Vector3 _moveDir;
    public Vector3 MoveDir
    {
        get { return _moveDir; }
        set { _moveDir = value; }
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
    public bool ArrivedAtDestProcess()
    {
        // 목적지에 도착여부 확인
        if (Vector3.Distance(_curTargetPosition.position,
                transform.position) < 0.01f)
        {
            StartCoroutine(WaitingTimer());
            return true;
        }

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

    private void OnDrawGizmosSelected()
    {
        // 이동 방향
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + _moveDir * 2f);
    }
}
