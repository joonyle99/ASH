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
        // ���� ��ġ
        wallHitPos = Player.WallHit.transform.position;

        // ���� Hit ��ġ
        wallHitPointPos = Player.WallHit.point;

        // ���� ��������
        wallNormalVec = Player.WallHit.normal;

        // ���� �븻 ������ ������ ���� ���
        if (wallHitPointPos.x > Player.transform.position.x)
            wallPerPendVec = (-1) * Vector2.Perpendicular(wallNormalVec).normalized;
        else
            wallPerPendVec = Vector2.Perpendicular(wallNormalVec).normalized;
    }

    protected override void OnUpdate()
    {

    }
    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {

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
