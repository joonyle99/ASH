using UnityEngine;

/// <summary>
/// wall 상태 (grab, climb, 등) 의 기반 클래스
/// </summary>
public abstract class WallState : PlayerState
{
    [Header("Wall State")]
    [Space]

    // Wall State를 상속받는 클래스에서 사용할 변수
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

        // 벽의 위치
        wallHitPos = Player.ClimbHit.transform.position;

        // 벽의 Hit 위치
        wallHitPointPos = Player.ClimbHit.point;

        // 벽의 법선벡터
        wallNormalVec = Player.ClimbHit.normal;

        // 벽의 노말 벡터의 수직한 벡터 계산
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
        // 플레이어가 이동하는 방향
        // DrawArrow.ForGizmo(wallHitPos, wallPerPendVec, Color.yellow);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(wallHitPos, wallHitPos + new Vector3(wallPerPendVec.x, wallPerPendVec.y, 0) * 3.5f);

        // 벽의 법선벡터
        // DrawArrow.ForGizmo(wallHitPos, wallNormalVec, Color.yellow);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(wallHitPos, wallHitPos + new Vector3(wallNormalVec.x, wallNormalVec.y, 0) * 1.5f);
    }
}
