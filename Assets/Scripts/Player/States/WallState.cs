using UnityEngine;

/// <summary>
/// wall ���� (grab, climb, ��) �� ��� Ŭ����
/// </summary>
public abstract class WallState : PlayerState
{
    [Header("Wall State")]
    [Space]

    // Wall State�� ��ӹ޴� Ŭ�������� ����� ����
    protected Vector3 wallHitPos;
    protected Vector3 wallHitPointPos;
    protected Vector2 wallNormalVec;
    protected Vector2 wallPerPendVec;

    protected override bool OnEnter()
    {
        Player.Animator.SetTrigger("Wall");
        Player.Animator.SetBool("IsWall", true);

        if (!Player.ClimbHit)
        {
            Debug.LogError($"ClimbHit is Null. Changes to Idle State");
            ChangeState<IdleState>();
            return false;
        }

        // ���� ��ġ
        wallHitPos = Player.ClimbHit.transform.position;

        // ���� Hit ��ġ
        wallHitPointPos = Player.ClimbHit.point;

        // ���� ��������
        wallNormalVec = Player.ClimbHit.normal;

        // ���� �븻 ������ ������ ���� ���
        if (wallHitPointPos.x > Player.transform.position.x) wallPerPendVec = (-1) * Vector2.Perpendicular(wallNormalVec).normalized;
        else wallPerPendVec = Vector2.Perpendicular(wallNormalVec).normalized;

        return true;
    }

    protected override bool OnUpdate()
    {

        return true;
    }
    protected override bool OnFixedUpdate()
    {

        return true;
    }

    protected override bool OnExit()
    {
        Player.Animator.SetBool("IsWall", false);

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        // �÷��̾ �̵��ϴ� ����
        // DrawArrow.ForGizmo(wallHitPos, wallPerPendVec, Color.yellow);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(wallHitPos, wallHitPos + new Vector3(wallPerPendVec.x, wallPerPendVec.y, 0) * 3.5f);

        // ���� ��������
        // DrawArrow.ForGizmo(wallHitPos, wallNormalVec, Color.yellow);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(wallHitPos, wallHitPos + new Vector3(wallNormalVec.x, wallNormalVec.y, 0) * 1.5f);
    }
}
