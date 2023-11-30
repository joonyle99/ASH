using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallState : PlayerState
{
    [Header("Wall State")]
    [Space]

    // Wall State를 상속받는 클래스에서 사용할 변수
    protected Vector2 wallPerPendVec;
    protected Vector2 wallNormalVec;
    protected Vector3 wallHitPointPos;
    protected Vector3 wallHitPos;

    // Length for Gizmos
    float _perpendVecLength = 3.5f;
    float _normalDirLength = 1.5f;

    protected override void OnEnter()
    {
        // 플레이어가 벽을 잡고있지 않은 경우
        if (!Player.WallHit)
            return;

        Player.Animator.SetBool("IsWall", true);

        // 벽의 법선벡터
        wallNormalVec = Player.WallHit.normal;

        // 벽의 위치
        wallHitPos = Player.WallHit.transform.position;

        // 벽의 Hit 위치
        wallHitPointPos = Player.WallHit.point;

        // 벽의 노말 벡터의 수직한 벡터 계산
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
        // 플레이어가 이동하는 방향
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(wallHitPos, wallHitPos + new Vector3(wallPerPendVec.x, wallPerPendVec.y, 0) * _perpendVecLength);

        // 벽의 법선벡터
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(wallHitPos, wallHitPos + new Vector3(wallNormalVec.x, wallNormalVec.y, 0) * _normalDirLength);
    }
}
