using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IdleState : PlayerState
{
    [Header("Idle Setting")]
    Vector2 _groundNormal;
    Vector3 _tempGroundHitPoint;

    protected override void OnEnter()
    {
        //Debug.Log("Idle Enter");

        // Exception
        if (!Player.GroundHit)
            return;

        // 땅의 법선벡터
        _groundNormal = Player.GroundHit.normal;

        // 땅의 raycast hit 위치값
        _tempGroundHitPoint = Player.GroundHit.point;
    }

    protected override void OnUpdate()
    {
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0)
        {
            ChangeState<WalkState>();
            return;
        }

        // 벽에서 미끄러지지 않음
        Player.Rigidbody.velocity = new Vector2((-1) * _groundNormal.x, (-1) * _groundNormal.y) * Time.deltaTime;
    }

    protected override void OnExit()
    {
        //Debug.Log("Idle Exit");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_tempGroundHitPoint, _tempGroundHitPoint + new Vector3(_groundNormal.x, _groundNormal.y, 0f));
    }
}