using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IdleState : PlayerState
{
    protected Vector2 donwDir;
    protected Vector2 groundNormal;
    protected Vector3 tempGroundHitPoint;

    protected override void OnEnter()
    {
        //Debug.Log("Idle Enter");

        if (!Player.GroundHit)
            return;

        // ���� ��������
        groundNormal = Player.GroundHit.normal;

        // ���� raycast hit ��ġ��
        tempGroundHitPoint = Player.GroundHit.point;

        Debug.Log(groundNormal);
    }

    protected override void OnUpdate()
    {
        if (Player.RawInputs.Movement.x != 0)
        {
            ChangeState<WalkState>();
            return;
        }

        // Idle ���¶�� ������ �̲������� ����
        Player.Rigidbody.velocity = new Vector2((-1) * groundNormal.x, (-1) * groundNormal.y) * Physics2D.gravity * Time.deltaTime;
    }

    protected override void OnExit()
    {
        //Debug.Log("Idle Exit");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(donwDir.x, donwDir.y, 0) * 2);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(tempGroundHitPoint, tempGroundHitPoint + new Vector3(groundNormal.x, groundNormal.y, 0) * 1.5f);
    }
}