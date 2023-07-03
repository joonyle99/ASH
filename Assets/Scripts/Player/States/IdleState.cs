using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IdleState : PlayerState
{
    protected Vector2 donwDir;
    protected Vector2 groundNormal;
    protected Vector3 tempGroundHitPoint;

    protected override void OnEnter()
    {
        //Debug.Log("Idle Enter");

        if (!Player.GroundHit)
            return;

        // 땅의 법선벡터
        groundNormal = Player.GroundHit.normal;

        // 땅의 raycast hit 위치값
        tempGroundHitPoint = Player.GroundHit.point;

        Debug.Log(groundNormal);
    }

    protected override void OnUpdate()
    {
        if (Player.RawInputs.Movement.x != 0)
        {
            ChangeState<WalkState>();
            return;
        }

        // Idle 상태라면 벽에서 미끄러지지 않음
        Player.Rigidbody.velocity = new Vector2((-1) * groundNormal.x, (-1) * groundNormal.y) * Physics2D.gravity * Time.deltaTime;
    }

    protected override void OnExit()
    {
        //Debug.Log("Idle Exit");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(donwDir.x, donwDir.y, 0) * 2);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(tempGroundHitPoint, tempGroundHitPoint + new Vector3(groundNormal.x, groundNormal.y, 0) * 1.5f);
    }
}