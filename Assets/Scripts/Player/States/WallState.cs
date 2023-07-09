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

        // ���� ��������
        wallNormal = Player.WallHit.normal;

        // ���� raycast hit vector ��
        tempWallHit = Player.WallHit.transform.position;

        // ���� �������Ϳ� �÷��̾ �ٶ󺸴� ������ ������ ���Ѵ�
        float dot = Vector2.Dot(wallNormal, Player.PlayerLookDir);

        // ������ 0���� ũ�� ����, ������ �а�
        if (dot > 0)
        {
            //Debug.Log("����");

            moveDirection = Vector2.Perpendicular(Player.WallHit.normal).normalized;
        }
        else
        {
            //Debug.Log("�а�");

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
