using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ս� �ս� ���ִ� Idle
/// </summary>
public class Floating_Idle : M_IdleState
{
    public Transform wayPoint;
    public float moveSpeed = 5f;    // ���� �̵� �ӵ�
    public float time = 0f;

    protected override void OnEnter()
    {
        base.OnEnter();

    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        // ���͸� ���� �������� �̵���ŵ�ϴ�.
        Transform targetWaypoint = wayPoint;
        Vector3 moveDirection = (targetWaypoint.position - transform.position).normalized;

        time += Time.deltaTime;

        if (time < 3f)
        {
            time = 0f;
            Monster.Rigidbody.AddForce(moveDirection * moveSpeed);
        }


        /*
        if (currentWaypointIndex < wayPoint.Length)
        {

            // ���Ͱ� ���� ������ ���� �����ߴ��� Ȯ���ϰ� ���� �������� �̵��մϴ�.
            if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
        else
        {
            // ��� ��θ� ���Ҵٸ� �������� ����ϴ�. (��θ� �ݺ��Ϸ��� currentWaypointIndex�� �缳�����ָ� �˴ϴ�)
            // ���� ���, currentWaypointIndex = 0; �� �����ϸ� ó������ �ٽ� �����̰� �˴ϴ�.
            moveSpeed = 0f;
        }
        */
    }

    protected override void OnExit()
    {
        base.OnExit();
    }
}
