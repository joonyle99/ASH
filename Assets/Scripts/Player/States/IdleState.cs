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
        if (groundAngle < _slopeThreshold)
        {
            Player.Rigidbody.AddForce(-_groundNormal * _belowForce);
        }
    }

    protected override void OnExit()
    {

    }
}