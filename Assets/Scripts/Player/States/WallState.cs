using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallState : PlayerState
{
    protected Vector2 moveDirection;    // Wall State�� ��ӹ޴� Ŭ�������� ����� ����
    protected Vector2 wallNormal;
    protected Vector3 WallHitPos;

    private float _moveDirLength = 1.5f;
    private float _normalDirLength = 1.5f;

    protected override void OnEnter()
    {
        // �÷��̾ ���� ������� ���� ���
        if (!Player.WallHit)
            return;

        Player.Animator.SetBool("IsWall", true);

        // ���� ��������
        wallNormal = Player.WallHit.normal;

        // ���� Raycast Hit ���� ��ǥ
        WallHitPos = Player.WallHit.transform.position;

        // ���� �������Ϳ� �÷��̾ �ٶ󺸴� ������ ������ ���Ѵ�
        float dot = Vector2.Dot(wallNormal, Player.PlayerLookDir);

        // ������ 0���� ũ�� ����
        if (dot > 0)
        {
            // ���� �������Ϳ� ������ ���ʹ� �÷��̾��� �̵����� (Normalize)
            // Perpendicular()�� ���� ���ʹ� �ð� �ݴ� �������� �׻� 90�� ȸ���� ����
            // https://docs.unity3d.com/ScriptReference/Vector2.Perpendicular.html
            moveDirection = Vector2.Perpendicular(Player.WallHit.normal).normalized;
        }
        // ������ �а�
        else
        {
            // ���� �������Ϳ� ������ ���͸� �ݴ� �������� �������� �Ѵ� (�÷��̾�� ���� ��ġ�� ���� �̵������� ��ȭ)
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
        Player.Animator.SetBool("IsWall", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(moveDirection.x, moveDirection.y, 0) * _moveDirLength);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(WallHitPos, WallHitPos + new Vector3(wallNormal.x, wallNormal.y, 0) * _normalDirLength);
    }
}
