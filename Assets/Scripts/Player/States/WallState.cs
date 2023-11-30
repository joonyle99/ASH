using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallState : PlayerState
{
    [Header("Wall State")]
    [Space]

    // Wall State�� ��ӹ޴� Ŭ�������� ����� ����
    protected Vector2 wallPerPendVec;
    protected Vector2 wallNormalVec;
    protected Vector3 wallHitPointPos;
    protected Vector3 wallHitPos;

    // Length for Gizmos
    float _perpendVecLength = 3.5f;
    float _normalDirLength = 1.5f;

    protected override void OnEnter()
    {
        // �÷��̾ ���� ������� ���� ���
        if (!Player.WallHit)
            return;

        Player.Animator.SetBool("IsWall", true);

        // ���� ��������
        wallNormalVec = Player.WallHit.normal;

        // ���� ��ġ
        wallHitPos = Player.WallHit.transform.position;

        // ���� Hit ��ġ
        wallHitPointPos = Player.WallHit.point;

        // ���� �븻 ������ ������ ���� ���
        if (wallHitPointPos.x > Player.transform.position.x)
            wallPerPendVec = (-1) * Vector2.Perpendicular(wallNormalVec).normalized;
        else
            wallPerPendVec = Vector2.Perpendicular(wallNormalVec).normalized;

        Debug.Log("WallState Enter");

        // Player.transform.position = new Vector3(wallHitPos.x, Player.transform.position.y);
    }

    protected override void OnUpdate()
    {

    }
    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {
        Player.Animator.SetBool("IsWall", false);
    }

    private void OnDrawGizmosSelected()
    {
        // �÷��̾ �̵��ϴ� ����
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(wallHitPos, wallHitPos + new Vector3(wallPerPendVec.x, wallPerPendVec.y, 0) * _perpendVecLength);

        // ���� ��������
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(wallHitPos, wallHitPos + new Vector3(wallNormalVec.x, wallNormalVec.y, 0) * _normalDirLength);
    }
}
