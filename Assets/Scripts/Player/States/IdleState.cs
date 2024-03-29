using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class IdleState : PlayerState
{
    [Header("Idle Settings")]
    [Space]

    [Tooltip("�Ʒ��� �����ִ� ��")]
    [SerializeField] float _belowForce = 150f;

    [Tooltip("�� ������ �ʰ��� ��翡�� ������ ����")]
    [SerializeField] float _slopeThreshold = 70f;

    protected override bool OnEnter()
    {


        return true;
    }

    protected override bool OnUpdate()
    {
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0)
        {
            ChangeState<RunState>();
            return true;
        }

        return true;
    }

    protected override bool OnFixedUpdate()
    {
        Vector2 groundNormal = Player.GroundHit.normal;
        float groundAngle = Mathf.Abs(Mathf.Atan2(groundNormal.y, groundNormal.x) * Mathf.Rad2Deg - 90f);

        // ������ ������ �̲��� ����
        if (groundAngle < _slopeThreshold)
            Player.Rigidbody.AddForce(-groundNormal * _belowForce);

        return true;
    }

    protected override bool OnExit()
    {
        return true;
    }
}