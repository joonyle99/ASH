using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallState : PlayerState
{
    // Wall State�� ��ӹ޴� Ŭ�������� ����� ����
    protected Vector2 moveDirection;
    protected Vector2 wallNormal;
    protected Vector3 wallHitPos;
    protected Vector3 crossVector;

    // Length for Gizmos
    float _moveDirLength = 1.5f;
    float _normalDirLength = 1.5f;
    float _crossDirLength = 1.5f;

    protected override void OnEnter()
    {
        // �÷��̾ ���� ������� ���� ���
        if (!Player.WallHit)
            return;

        Player.Animator.SetBool("IsWall", true);

        // ���� ��������
        wallNormal = Player.WallHit.normal;

        // ���� Raycast Hit ���� ��ǥ
        wallHitPos = Player.WallHit.transform.position;

        // ���� �������Ϳ� �÷��̾ �ٶ󺸴� ������ ������ ���Ѵ�
        float dot = Vector2.Dot(wallNormal, Player.PlayerLookDir);

        /*
        crossVector = Vector3.Cross(wallNormal, Vector3.up); // ������ �̿��� ȸ���� ���
        */

        // ���� ���⿡ ���� ĳ���͸� ȸ��
        // transform.Rotate(new Vector3(0, 0, Player.WallHit.collider.transform.rotation.z * 100f));

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

        // ���� ���󺹱�
        // transform.rotation = Quaternion.identity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(moveDirection.x, moveDirection.y, 0) * _moveDirLength);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(wallHitPos, wallHitPos + new Vector3(wallNormal.x, wallNormal.y, 0) * _normalDirLength);

        /*
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, 0, crossVector.z) * _crossDirLength);
        */
    }
}
