using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WayPoint Patrol ���
/// Monster�� ������ ���
/// Way Point�� �湮�Ѵ�.
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
        // Way Point�� List�� �߰�
        for (int i = 0; i < _wayPointBox.childCount; ++i)
        {
            Transform wayPoint = _wayPointBox.GetChild(i);
            if (wayPoint.gameObject.activeSelf)
                _wayPoints.Add(wayPoint);
        }

        // �ʱ� ������ ����
        _curTargetPosition = _wayPoints[_curWayPointIndex];
        _nextTargetPosition = _wayPoints[(_curWayPointIndex + 1) % _wayPoints.Count];
    }

    public void SetMoveDir()
    {
        // �̵� ���� ����
        _moveDir = (_curTargetPosition.position - transform.position).normalized;
    }

    public bool IsArrived()
    {
        // �������� �������� Ȯ��
        if (Vector3.Distance(_curTargetPosition.position,
                transform.position) < _targetDistance)
            return true;
        else
            return false;
    }

    public void ChangeWayPoint()
    {
        // ������ ����
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
        // �̵� ����
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(this.transform.position, this.transform.position + _moveDir * 2f);
    }
}
