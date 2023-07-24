using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 둥실 둥실 떠있는 Idle
/// </summary>
public class Floating_Idle : M_IdleState
{
    public Transform wayPoint;
    public float moveSpeed = 5f;    // 몬스터 이동 속도
    public float time = 0f;

    protected override void OnEnter()
    {
        base.OnEnter();

    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        // 몬스터를 다음 지점으로 이동시킵니다.
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

            // 몬스터가 현재 지점에 거의 도달했는지 확인하고 다음 지점으로 이동합니다.
            if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
        else
        {
            // 모든 경로를 돌았다면 움직임을 멈춥니다. (경로를 반복하려면 currentWaypointIndex를 재설정해주면 됩니다)
            // 예를 들어, currentWaypointIndex = 0; 로 설정하면 처음부터 다시 움직이게 됩니다.
            moveSpeed = 0f;
        }
        */
    }

    protected override void OnExit()
    {
        base.OnExit();
    }
}
