using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallState : PlayerState
{
    // Wall State를 상속받는 클래스에서 사용할 변수
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
        // 플레이어가 벽을 잡고있지 않은 경우
        if (!Player.WallHit)
            return;

        Player.Animator.SetBool("IsWall", true);

        // 벽의 법선벡터
        wallNormal = Player.WallHit.normal;

        // 벽의 Raycast Hit 지점 좌표
        wallHitPos = Player.WallHit.transform.position;

        // 벽의 법선벡터와 플레이어가 바라보는 방향의 내적을 구한다
        float dot = Vector2.Dot(wallNormal, Player.PlayerLookDir);

        /*
        crossVector = Vector3.Cross(wallNormal, Vector3.up); // 외적을 이용한 회전축 계산
        */

        // 벽의 기울기에 따라 캐릭터를 회전
        // transform.Rotate(new Vector3(0, 0, Player.WallHit.collider.transform.rotation.z * 100f));

        // 내적이 0보다 크면 예각
        if (dot > 0)
        {
            // 벽의 법선벡터와 수직한 벡터는 플레이어의 이동방향 (Normalize)
            // Perpendicular()로 구한 벡터는 시계 반대 방향으로 항상 90도 회전한 벡터
            // https://docs.unity3d.com/ScriptReference/Vector2.Perpendicular.html
            moveDirection = Vector2.Perpendicular(Player.WallHit.normal).normalized;
        }
        // 작으면 둔각
        else
        {
            // 벽의 법선벡터에 수직인 벡터를 반대 방향으로 만들어줘야 한다 (플레이어와 벽의 위치에 따른 이동방향을 변화)
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

        // 기울기 원상복구
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
