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
    [SerializeField] private float _targetDistance = 0.1f;
    [SerializeField] private float _targetWaitTime = 2f;
    [SerializeField] private Vector3 _moveDir;
    [SerializeField] private bool _isWaiting;

    #region Property

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

    #endregion

    void Start()
    {
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
        _moveDir = (_curTargetPosition.position - transform.position).normalized;
    }

    public bool IsArrived()
    {
        if (Vector3.Distance(_curTargetPosition.position,
                transform.position) < _targetDistance)
            return true;
        else
            return false;
    }

    public void ChangeWayPoint()
    {
        _curWayPointIndex++;
        _curTargetPosition = _nextTargetPosition;
        _nextTargetPosition = _wayPoints[(_curWayPointIndex + 1) % _wayPoints.Count];
    }

    public IEnumerator WaitingTimer()
    {
        _isWaiting = true;

        yield return new WaitForSeconds(_targetWaitTime);

        _isWaiting = false;
    }

    private void OnDrawGizmosSelected()
    {
        // 이동 방향
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(this.transform.position, this.transform.position + _moveDir * 2f);
    }
}
