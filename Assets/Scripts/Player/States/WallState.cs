using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WallState : PlayerState
{
    protected Vector2 moveDirection;
    protected Vector2 wallNormal;
    protected Vector3 tempWallHit;

    protected override void OnEnter()
    {
        if (!Player.WallHit)
            return;

        Player.Animator.SetBool("Wall", true);
        Player.Animator.SetBool("Jump", false);

        // 벽의 법선벡터
        wallNormal = Player.WallHit.normal;

        // 벽의 raycast hit vector 값
        tempWallHit = Player.WallHit.transform.position;

        // 벽의 법선벡터와 플레이어가 바라보는 방향의 내적을 구한다
        float dot = Vector2.Dot(wallNormal, Player.PlayerLookDir);

        // 내적이 0보다 크면 예각, 작으면 둔각
        if (dot > 0)
        {
            //Debug.Log("예각");

            moveDirection = Vector2.Perpendicular(Player.WallHit.normal).normalized;
        }
        else
        {
            //Debug.Log("둔각");

            if (Player.WallHit.point.x > Player.transform.position.x)
                moveDirection = (-1) * Vector2.Perpendicular(Player.WallHit.normal).normalized;
            else
                moveDirection = Vector2.Perpendicular(Player.WallHit.normal).normalized;
        }
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnExit()
    {
        Player.Animator.SetBool("Wall", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(moveDirection.x, moveDirection.y, 0) * 2);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(tempWallHit, tempWallHit + new Vector3(wallNormal.x, wallNormal.y, 0) * 2);
    }
}
