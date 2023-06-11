using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WallState : PlayerState
{
    protected Vector2 moveDirection;
    protected Vector2 wallNormal;

    protected override void OnEnter()
    {
        if (!Player.WallHit)
            return;

        // ���� �÷��̾� ���� �����ʿ� ������, ���� ���� ���͸� �ݴ��
        if (Player.WallHit.transform.position.x > Player.transform.position.x)
            wallNormal = Player.WallHit.normal;
        else
            wallNormal = (-1) * Player.WallHit.normal;
    }

    protected override void OnUpdate()
    {
        float dot = Vector2.Dot(wallNormal, Player.PlayerLookDir);

        // ���� �÷��̾��� ������ ���� ���� �Ǵ�
        if (dot > 0)
            moveDirection = Vector2.Perpendicular(Player.WallHit.normal).normalized;
        else
            moveDirection = (-1) * Vector2.Perpendicular(Player.WallHit.normal).normalized;

    }

    protected override void OnExit()
    {

    }
}
