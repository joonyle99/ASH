using UnityEngine;

public class IdleState : PlayerState
{
    [Header("Idle Setting")]

    [Space]

    [SerializeField] float _belowForce = 25f;       // 아래로 가해주는 힘

    Vector2 _groundNormal;                          // 땅의 법선벡터
    Vector3 _groundHitPoint;                        // 땅의 Hit Point

    protected override void OnEnter()
    {
        // 땅의 법선벡터
        _groundNormal = Player.GroundHit.normal;

        // 땅의 Hit Point
        _groundHitPoint = Player.GroundHit.point;
    }

    protected override void OnUpdate()
    {
        // Run State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0)
        {
            ChangeState<RunState>();
            return;
        }

        // 플레이어와 땅 사이의 각도
        // float _angle = Vector3.Angle(_groundNormal, Player.PlayerLookDir);

        // 기울어진 땅에서 미끄럼 방지
        // if (Mathf.Abs(90f - _angle) > 5f)

        // 이동을 멈추면 미끄럼 방지
        // 기울어진 땅에서 효과적
        // 근데 그냥 마찰력을 키우면 되는거 아니야..?
        // 그렇게 하면 기울어진 땅을 오르지를 못하네 ~
        Player.Rigidbody.AddForce(-_groundNormal * _belowForce);
    }

    protected override void OnExit()
    {

    }

    private void OnDrawGizmosSelected()
    {
        // 땅의 법선벡터 그리기
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_groundHitPoint, _groundHitPoint + new Vector3(_groundNormal.x, _groundNormal.y, 0f));
    }
}