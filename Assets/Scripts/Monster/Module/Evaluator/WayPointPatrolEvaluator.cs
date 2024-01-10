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
    public bool ArrivedAtDestProcess()
    {
        // �������� �������� Ȯ��
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

    private void OnDrawGizmosSelected()
    {
        // �̵� ����
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + _moveDir * 2f);
    }
}
