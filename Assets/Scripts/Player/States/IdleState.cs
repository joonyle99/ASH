using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class IdleState : PlayerState
{
    [Header("Idle Settings")]
    [SerializeField] float _belowForce = 150;         // 아래로 가해주는 힘

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

        // 기울어진 땅에서 미끄럼 방지
        if (groundAngle < Player.SlopeThreshold)
        {
            Player.Rigidbody.AddForce(-_groundNormal * _belowForce);
        }
    }

    protected override void OnExit()
    {

    }
}