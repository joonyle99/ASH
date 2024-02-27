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

    protected override void OnEnter()
    {

    }

    protected override void OnUpdate()
    {
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0)
        {
            ChangeState<RunState>();
            return;
        }

    }

    protected override void OnFixedUpdate()
    {
        Vector2 _groundNormal = Player.GroundHit.normal;
        float groundAngle = Mathf.Abs(Mathf.Atan2(_groundNormal.y, _groundNormal.x) * Mathf.Rad2Deg - 90);

        // ������ ������ �̲��� ����
        if (groundAngle < _slopeThreshold)
        {
            Player.Rigidbody.AddForce(-_groundNormal * _belowForce);
        }
    }

    protected override void OnExit()
    {

    }
}