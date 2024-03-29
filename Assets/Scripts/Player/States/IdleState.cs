using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class IdleState : PlayerState
{
    [Header("Idle Settings")]
    [Space]

    [Tooltip("아래로 가해주는 힘")]
    [SerializeField] float _belowForce = 150f;

    [Tooltip("이 각도를 초과한 경사에선 서있지 못함")]
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

        // 기울어진 땅에서 미끄럼 방지
        if (groundAngle < _slopeThreshold)
            Player.Rigidbody.AddForce(-groundNormal * _belowForce);

        return true;
    }

    protected override bool OnExit()
    {
        return true;
    }
}